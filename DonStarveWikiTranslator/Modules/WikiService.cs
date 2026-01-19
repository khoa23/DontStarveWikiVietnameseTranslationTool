using DonStarveWikiTranslator.Data;
using DonStarveWikiTranslator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DonStarveWikiTranslator.Modules
{
    public class WikiService : IDisposable
    {
        private readonly WikiApiClient _enClient;
        private readonly WikiApiClient _viClient;
        private readonly WikiRepository _repository;
        private readonly int _cacheExpirationHours;

        public WikiService()
        {
            // Read URLs from AppConfig
            _enClient = new WikiApiClient(AppConfig.EnglishApiUrl, AppConfig.EnglishWikiUrl);
            _viClient = new WikiApiClient(AppConfig.VietnameseApiUrl, AppConfig.VietnameseWikiUrl);
            _repository = new WikiRepository();
            _cacheExpirationHours = AppConfig.CacheExpirationHours;
        }

        /// <summary>
        /// Get articles that exist in English but not in Vietnamese
        /// Checks cache first, then fetches from API if needed
        /// </summary>
        public async Task<List<string>> GetMissingArticles(int limit = 50, bool forceRefresh = false, CancellationToken ct = default)
        {
            Logger.Log($"Fetching missing articles (limit: {limit}, forceRefresh: {forceRefresh})...");

            if (!forceRefresh)
            {
                // Try to get from cache first
                var cached = _repository.GetMissingArticles();
                if (cached.Any() && !cached.First().IsStale(_cacheExpirationHours))
                {
                    Logger.Log($"Returning {cached.Count} missing articles from cache");
                    return cached.Select(a => a.Title).ToList();
                }
            }

            // Fetch from API
            var missingPages = await _enClient.GetPagesWithoutLangLink("vi", limit);
            
            // Save to database
            await SyncArticlesToDatabase(missingPages, ct);
            
            Logger.Log($"Found {missingPages.Count} missing articles from API");
            return missingPages;
        }

        /// <summary>
        /// Get articles that exist in both languages but Vietnamese version is outdated
        /// Checks cache first, then fetches from API if needed
        /// </summary>
        public async Task<List<OutdatedArticle>> GetOutdatedArticles(int limit = 50, bool forceRefresh = false, CancellationToken ct = default)
        {
            Logger.Log($"Fetching outdated articles (limit: {limit}, forceRefresh: {forceRefresh})...");

            if (!forceRefresh)
            {
                // Try to get from cache first
                var cached = _repository.GetOutdatedArticles();
                if (cached.Any() && !cached.First().IsStale(_cacheExpirationHours))
                {
                    Logger.Log($"Returning {cached.Count} outdated articles from cache");
                    return cached.Select(a => new OutdatedArticle
                    {
                        Title = a.Title,
                        VietnameseTitle = a.VietnameseTitle,
                        EnglishLastUpdate = a.EnglishLastUpdate ?? DateTime.MinValue,
                        VietnameseLastUpdate = a.VietnameseLastUpdate ?? DateTime.MinValue,
                        DaysBehind = a.DaysBehind
                    }).ToList();
                }
            }

            var outdatedArticles = new List<OutdatedArticle>();

            // Get a list of English pages
            var enPages = await _enClient.GetAllPages(limit);
            Logger.Log($"Checking {enPages.Count} English articles in batches...");

            const int BatchSize = 25;
            for (int i = 0; i < enPages.Count; i += BatchSize)
            {
                ct.ThrowIfCancellationRequested();
                var batch = enPages.Skip(i).Take(BatchSize).ToList();
                
                try
                {
                    // Batch fetch info for English articles first
                    var enInfos = await _enClient.GetPagesInfo(batch);

                    // Collect Vietnamese titles from English info to fetch from Vietnamese wiki
                    var viTitlesToFetch = enInfos.Values
                        .Where(info => info != null && !string.IsNullOrEmpty(info.VietnameseTitle))
                        .Select(info => info.VietnameseTitle)
                        .ToList();

                    // Map English title to its Vietnamese title for lookup
                    var titleMap = enInfos.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.VietnameseTitle ?? kvp.Key,
                        StringComparer.OrdinalIgnoreCase
                    );

                    // Fetch Vietnamese info using the correct Vietnamese titles (or English if missing langlink)
                    var viInfoResults = await _viClient.GetPagesInfo(titleMap.Values.Distinct().ToList());

                    foreach (var enTitle in batch)
                    {
                        enInfos.TryGetValue(enTitle, out var enInfo);
                        
                        // Get the mapped Vietnamese title to lookup in viInfoResults
                        var viSearchTitle = titleMap[enTitle];
                        viInfoResults.TryGetValue(viSearchTitle, out var viInfo);

                        if (enInfo == null) continue;

                        if (viInfo == null)
                        {
                            await SaveArticleToDatabase(enTitle, enInfo, null, TranslationStatus.Missing);
                            continue;
                        }

                        // Compare timestamps
                        if (!string.IsNullOrEmpty(enInfo.LastRevisionTimestamp) &&
                            !string.IsNullOrEmpty(viInfo.LastRevisionTimestamp))
                        {
                            var enTime = DateTime.Parse(enInfo.LastRevisionTimestamp);
                            var viTime = DateTime.Parse(viInfo.LastRevisionTimestamp);

                            if (enTime > viTime)
                            {
                                outdatedArticles.Add(new OutdatedArticle
                                {
                                    Title = enTitle,
                                    VietnameseTitle = viInfo.Title, // The title from vi wiki
                                    EnglishLastUpdate = enTime,
                                    VietnameseLastUpdate = viTime,
                                    DaysBehind = (enTime - viTime).Days
                                });

                                await SaveArticleToDatabase(enTitle, enInfo, viInfo, TranslationStatus.Outdated);
                            }
                            else
                            {
                                await SaveArticleToDatabase(enTitle, enInfo, viInfo, TranslationStatus.UpToDate);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error batch checking articles: {ex.ToString()}");
                }

                if (i + BatchSize < enPages.Count)
                    await Task.Delay(500); // Respect rate limit
            }

            Logger.Log($"Found {outdatedArticles.Count} outdated articles total");
            return outdatedArticles.OrderByDescending(a => a.DaysBehind).ToList();
        }

        /// <summary>
        /// Get page content from English wiki
        /// </summary>
        public async Task<string> GetEnglishPageContent(string title)
        {
            return await _enClient.GetPageContent(title);
        }

        /// <summary>
        /// Get page content from Vietnamese wiki
        /// </summary>
        public async Task<string> GetVietnamesePageContent(string title)
        {
            return await _viClient.GetPageContent(title);
        }

        /// <summary>
        /// Sync specific article to database
        /// </summary>
        public async Task SyncArticleToDatabase(string title)
        {
            Logger.Log($"Syncing article '{title}' to database...");

            var enInfo = await _enClient.GetPageInfo(title);
            
            // Use the Vietnamese title from English wiki langlinks if available
            var viSearchTitle = enInfo?.VietnameseTitle ?? title;
            var viInfo = await _viClient.GetPageInfo(viSearchTitle);

            TranslationStatus status = TranslationStatus.UpToDate;
            
            if (viInfo == null)
            {
                status = TranslationStatus.Missing;
            }
            else if (enInfo != null && viInfo != null)
            {
                var enTime = DateTime.Parse(enInfo.LastRevisionTimestamp ?? DateTime.MinValue.ToString());
                var viTime = DateTime.Parse(viInfo.LastRevisionTimestamp ?? DateTime.MinValue.ToString());
                
                if (enTime > viTime)
                    status = TranslationStatus.Outdated;
            }

            await SaveArticleToDatabase(title, enInfo, viInfo, status);
        }

        /// <summary>
        /// Sync multiple articles to database
        /// </summary>
        private async Task SyncArticlesToDatabase(List<string> titles, CancellationToken ct = default)
        {
            const int BatchSize = 25;
            for (int i = 0; i < titles.Count; i += BatchSize)
            {
                ct.ThrowIfCancellationRequested();
                var batch = titles.Skip(i).Take(BatchSize).ToList();
                var articles = new List<WikiArticle>();

                try
                {
                    // Batch fetch info for English articles first
                    var enInfos = await _enClient.GetPagesInfo(batch);

                    // Map English title to its Vietnamese title for lookup
                    var titleMap = enInfos.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.VietnameseTitle ?? kvp.Key,
                        StringComparer.OrdinalIgnoreCase
                    );

                    // Fetch Vietnamese info using the correct Vietnamese titles (or English if missing langlink)
                    var viInfoResults = await _viClient.GetPagesInfo(titleMap.Values.Distinct().ToList());

                    foreach (var title in batch)
                    {
                        enInfos.TryGetValue(title, out var enInfo);

                        // Get the mapped Vietnamese title to lookup in viInfoResults
                        var viSearchTitle = titleMap[title];
                        viInfoResults.TryGetValue(viSearchTitle, out var viInfo);

                        if (enInfo == null)
                        {
                            Logger.Log($"Warning: Could not find English info for '{title}' during batch sync.");
                        }

                        TranslationStatus status = viInfo == null 
                            ? TranslationStatus.Missing 
                            : TranslationStatus.UpToDate;

                        // Check for outdated if both exist
                        if (enInfo != null && viInfo != null)
                        {
                            var enTime = DateTime.Parse(enInfo.LastRevisionTimestamp ?? DateTime.MinValue.ToString());
                            var viTime = DateTime.Parse(viInfo.LastRevisionTimestamp ?? DateTime.MinValue.ToString());
                            if (enTime > viTime) status = TranslationStatus.Outdated;
                        }

                        var article = CreateArticleFromInfo(title, enInfo, viInfo, status);
                        articles.Add(article);
                    }

                    if (articles.Any())
                    {
                        _repository.SaveOrUpdateArticles(articles);
                        Logger.Log($"Saved batch of {articles.Count} articles to database");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error syncing batch: {ex.ToString()}");
                }

                if (i + BatchSize < titles.Count)
                    await Task.Delay(500); // Respect rate limit
            }
        }

        /// <summary>
        /// Save article information to database
        /// </summary>
        private async Task SaveArticleToDatabase(string title, PageInfo enInfo, PageInfo viInfo, TranslationStatus status)
        {
            var article = CreateArticleFromInfo(title, enInfo, viInfo, status);
            _repository.SaveOrUpdateArticle(article);
        }

        /// <summary>
        /// Create WikiArticle entity from API page info
        /// </summary>
        private WikiArticle CreateArticleFromInfo(string title, PageInfo enInfo, PageInfo viInfo, TranslationStatus status)
        {
            return new WikiArticle
            {
                Title = title,
                VietnameseTitle = enInfo?.VietnameseTitle,
                EnglishUrl = enInfo?.FullUrl,
                VietnameseUrl = viInfo?.FullUrl,
                EnglishContent = enInfo?.Content,
                VietnameseContent = viInfo?.Content,
                EnglishLastUpdate = enInfo != null && !string.IsNullOrEmpty(enInfo.LastRevisionTimestamp)
                    ? DateTime.Parse(enInfo.LastRevisionTimestamp)
                    : (DateTime?)null,
                VietnameseLastUpdate = viInfo != null && !string.IsNullOrEmpty(viInfo.LastRevisionTimestamp)
                    ? DateTime.Parse(viInfo.LastRevisionTimestamp)
                    : (DateTime?)null,
                LastSyncDate = DateTime.Now,
                Status = status
            };
        }

        /// <summary>
        /// Get article from cache only (no API call)
        /// </summary>
        public WikiArticle GetArticleFromCache(string title)
        {
            return _repository.GetArticleByTitle(title);
        }

        /// <summary>
        /// Get database statistics
        /// </summary>
        public (int Total, int Missing, int Outdated, int UpToDate) GetDatabaseStats()
        {
            return (
                _repository.GetTotalCount(),
                _repository.GetCountByStatus(TranslationStatus.Missing),
                _repository.GetCountByStatus(TranslationStatus.Outdated),
                _repository.GetCountByStatus(TranslationStatus.UpToDate)
            );
        }

        /// <summary>
        /// Clear all cached data
        /// </summary>
        public void ClearCache()
        {
            _repository.ClearCache();
            Logger.Log("Database cache cleared");
        }

        public void Dispose()
        {
            _repository?.Dispose();
        }
    }

    public class OutdatedArticle
    {
        public string Title { get; set; }
        public string VietnameseTitle { get; set; }
        public DateTime EnglishLastUpdate { get; set; }
        public DateTime VietnameseLastUpdate { get; set; }
        public int DaysBehind { get; set; }
    }
}

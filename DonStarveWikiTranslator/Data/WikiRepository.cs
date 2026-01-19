using DonStarveWikiTranslator.Models;
using DonStarveWikiTranslator.Modules;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DonStarveWikiTranslator.Data
{
    /// <summary>
    /// Repository for managing WikiArticle database operations
    /// </summary>
    public class WikiRepository : IDisposable
    {
        private WikiContext _context;

        public WikiRepository()
        {
            _context = new WikiContext();
        }

        private void ResetContext()
        {
            try
            {
                _context?.Dispose();
                _context = new WikiContext();
                Logger.Log("Database context has been reset due to a previous error.");
            }
            catch (Exception ex)
            {
                Logger.Log($"Critical error resetting database context: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all articles from database
        /// </summary>
        public List<WikiArticle> GetAllArticles()
        {
            return _context.WikiArticles.ToList();
        }

        /// <summary>
        /// Get article by exact title match
        /// </summary>
        public WikiArticle GetArticleByTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return null;
            
            string trimmedTitle = title.Trim();

            // 1. Check local context first (not yet saved to DB)
            var local = _context.WikiArticles.Local
                .FirstOrDefault(a => string.Equals(a.Title?.Trim(), trimmedTitle, StringComparison.OrdinalIgnoreCase));
            if (local != null) return local;

            // 2. Check Database
            return _context.WikiArticles
                .FirstOrDefault(a => a.Title.Trim() == trimmedTitle);
        }

        /// <summary>
        /// Get articles that don't have Vietnamese version
        /// </summary>
        public List<WikiArticle> GetMissingArticles()
        {
            return _context.WikiArticles
                .Where(a => a.Status == TranslationStatus.Missing)
                .OrderBy(a => a.Title)
                .ToList();
        }

        /// <summary>
        /// Get articles where Vietnamese version is outdated
        /// </summary>
        public List<WikiArticle> GetOutdatedArticles()
        {
            return _context.WikiArticles
                .Where(a => a.Status == TranslationStatus.Outdated)
                .ToList()
                .OrderByDescending(a => a.DaysBehind)
                .ToList();
        }

        /// <summary>
        /// Get articles where cache is stale (older than expiration time)
        /// </summary>
        public List<WikiArticle> GetStaleArticles(int expirationHours)
        {
            var threshold = DateTime.Now.AddHours(-expirationHours);
            return _context.WikiArticles
                .Where(a => a.LastSyncDate < threshold)
                .ToList();
        }

        /// <summary>
        /// Save or update an article
        /// </summary>
        public void SaveOrUpdateArticle(WikiArticle article)
        {
            if (article == null || string.IsNullOrWhiteSpace(article.Title)) return;

            var existing = GetArticleByTitle(article.Title);
            
            if (existing != null)
            {
                // Update existing
                existing.VietnameseTitle = article.VietnameseTitle;
                existing.EnglishUrl = article.EnglishUrl;
                existing.VietnameseUrl = article.VietnameseUrl;
                existing.EnglishContent = article.EnglishContent;
                existing.VietnameseContent = article.VietnameseContent;
                existing.EnglishLastUpdate = article.EnglishLastUpdate;
                existing.VietnameseLastUpdate = article.VietnameseLastUpdate;
                existing.LastSyncDate = article.LastSyncDate;
                existing.Status = article.Status;
            }
            else
            {
                // Ensure title is trimmed
                article.Title = article.Title.Trim();
                _context.WikiArticles.Add(article);
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Log($"SQL Save Error (Single): {ex.ToString()}");
                ResetContext();
                throw; // Rethrow to let caller know
            }
        }

        /// <summary>
        /// Save or update multiple articles
        /// </summary>
        public void SaveOrUpdateArticles(IEnumerable<WikiArticle> articles)
        {
            if (articles == null) return;

            var articleList = articles.ToList();
            foreach (var article in articleList)
            {
                if (article == null || string.IsNullOrWhiteSpace(article.Title)) continue;

                var existing = GetArticleByTitle(article.Title);
                
                if (existing != null)
                {
                    // Update existing
                    existing.VietnameseTitle = article.VietnameseTitle;
                    existing.EnglishUrl = article.EnglishUrl;
                    existing.VietnameseUrl = article.VietnameseUrl;
                    existing.EnglishContent = article.EnglishContent;
                    existing.VietnameseContent = article.VietnameseContent;
                    existing.EnglishLastUpdate = article.EnglishLastUpdate;
                    existing.VietnameseLastUpdate = article.VietnameseLastUpdate;
                    existing.LastSyncDate = article.LastSyncDate;
                    existing.Status = article.Status;
                }
                else
                {
                    article.Title = article.Title.Trim();
                    _context.WikiArticles.Add(article);
                }
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Log($"Batch save failed, entering salvage mode (individual saves). Error: {ex.Message}");
                ResetContext();
                
                // Salvage mode: Try to save each article individually
                foreach (var article in articleList)
                {
                    try
                    {
                        SaveOrUpdateArticle(article);
                    }
                    catch (Exception individualEx)
                    {
                        Logger.Log($"Failed to salvage article '{article.Title}': {individualEx.Message}");
                        // Continue to next article
                    }
                }
            }
        }

        /// <summary>
        /// Delete an article by title
        /// </summary>
        public void DeleteArticle(string title)
        {
            var article = GetArticleByTitle(title);
            if (article != null)
            {
                _context.WikiArticles.Remove(article);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Clear all cached data
        /// </summary>
        public void ClearCache()
        {
            _context.WikiArticles.RemoveRange(_context.WikiArticles);
            _context.SaveChanges();
        }

        /// <summary>
        /// Get total article count
        /// </summary>
        public int GetTotalCount()
        {
            return _context.WikiArticles.Count();
        }

        /// <summary>
        /// Get count by status
        /// </summary>
        public int GetCountByStatus(TranslationStatus status)
        {
            return _context.WikiArticles.Count(a => a.Status == status);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DonStarveWikiTranslator.Modules
{
    public class WikiApiClient
    {
        private readonly HttpClient _http;
        private readonly string _apiUrl;
        private readonly string _baseWikiUrl;
        private string _csrfToken = "";

        public WikiApiClient(string apiUrl, string baseWikiUrl)
        {
            _apiUrl = apiUrl;
            _baseWikiUrl = baseWikiUrl;
            _http = new HttpClient(new HttpClientHandler { CookieContainer = new CookieContainer() });
            
            // MediaWiki APIs require a User-Agent header to avoid 403 Forbidden errors
            _http.DefaultRequestHeaders.Add("User-Agent", "DonStarveWikiTranslatorTool/1.0 (https://github.com/vinh-hodan/DonStarveWikiTranslator; contact@example.com)");
        }

        public async Task<string> GetPageContent(string title)
        {
            var url = $"{_apiUrl}?action=query&prop=revisions&rvprop=content&titles={title}&format=json";
            var json = JObject.Parse(await _http.GetStringAsync(url));
            var pages = json["query"]["pages"].First.First["revisions"][0]["*"];
            return pages.ToString();
        }

        public async Task Login(string username, string password)
        {
            // B1: Lấy token đăng nhập
            var tokenResp = JObject.Parse(await _http.GetStringAsync($"{_apiUrl}?action=query&meta=tokens&type=login&format=json"));
            string token = tokenResp["query"]["tokens"]["logintoken"].ToString();

            // B2: Gửi yêu cầu đăng nhập
            var form = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string,string>("action","login"),
            new KeyValuePair<string,string>("lgname",username),
            new KeyValuePair<string,string>("lgpassword",password),
            new KeyValuePair<string,string>("lgtoken",token),
            new KeyValuePair<string,string>("format","json")
        });
            await _http.PostAsync(_apiUrl, form);
        }

        public async Task GetCsrfToken()
        {
            var tokenResp = JObject.Parse(await _http.GetStringAsync($"{_apiUrl}?action=query&meta=tokens&format=json"));
            _csrfToken = tokenResp["query"]["tokens"]["csrftoken"].ToString();
        }

        public async Task PostPage(string title, string text)
        {
            if (string.IsNullOrEmpty(_csrfToken)) await GetCsrfToken();
            var form = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string,string>("action","edit"),
            new KeyValuePair<string,string>("title","vi:" + title),
            new KeyValuePair<string,string>("text",text),
            new KeyValuePair<string,string>("token",_csrfToken),
            new KeyValuePair<string,string>("format","json"),
            new KeyValuePair<string,string>("summary","Auto-translated via WinForm tool")
        });
            await _http.PostAsync(_apiUrl, form);
        }

        public async Task<JArray> GetRecentChanges()
        {
            var url = $"{_apiUrl}?action=query&list=recentchanges&rcprop=title|timestamp&rclimit=50&format=json";
            var json = JObject.Parse(await _http.GetStringAsync(url));
            return (JArray)json["query"]["recentchanges"];
        }

        /// <summary>
        /// Get all pages from the wiki (namespace 0 = main articles)
        /// </summary>
        public async Task<List<string>> GetAllPages(int limit = 500)
        {
            var pages = new List<string>();
            string continueToken = null;
            int fetched = 0;

            do
            {
                var batchLimit = Math.Min(500, limit - fetched);
                var url = $"{_apiUrl}?action=query&list=allpages&aplimit={batchLimit}&apnamespace=0&format=json";
                if (!string.IsNullOrEmpty(continueToken))
                    url += $"&apcontinue={continueToken}";

                var json = JObject.Parse(await _http.GetStringAsync(url));
                var allPages = (JArray)json["query"]?["allpages"];
                
                if (allPages != null)
                {
                    foreach (var page in allPages)
                    {
                        pages.Add(page["title"].ToString());
                    }
                    fetched += allPages.Count;
                }

                continueToken = json["continue"]?["apcontinue"]?.ToString();
            } while (continueToken != null && fetched < limit);

            return pages;
        }

        /// <summary>
        /// Get page info including last revision timestamp
        /// </summary>
        public async Task<PageInfo> GetPageInfo(string title)
        {
            // Include rvprop=content and rvslots=main to get the actual page text
            // Added prop=langlinks to get Vietnamese title from English wiki
            var url = $"{_apiUrl}?action=query&prop=info|revisions|langlinks&lllang=vi&rvprop=timestamp|content&rvslots=main&titles={Uri.EscapeDataString(title)}&format=json";
            var json = JObject.Parse(await _http.GetStringAsync(url));
            var page = json["query"]["pages"].First.First;

            if (page["missing"] != null)
                return null;

            // Get content from revisions
            string content = page["revisions"]?[0]?["slots"]?["main"]?["*"]?.ToString() 
                           ?? page["revisions"]?[0]?["*"]?.ToString();

            // Extract Vietnamese title from langlinks if available
            string viTitle = null;
            var langlinks = page["langlinks"] as JArray;
            if (langlinks != null && langlinks.Count > 0)
            {
                viTitle = langlinks[0]["*"]?.ToString();
            }

            return new PageInfo
            {
                Title = page["title"].ToString(),
                VietnameseTitle = viTitle,
                FullUrl = $"{_baseWikiUrl.TrimEnd('/')}/{Uri.EscapeDataString(page["title"].ToString()).Replace("%20", "_")}",
                PageId = page["pageid"].ToString(),
                LastRevisionTimestamp = page["revisions"]?[0]?["timestamp"]?.ToString(),
                Touched = page["touched"]?.ToString(),
                Content = content
            };
        }

        /// <summary>
        /// Get page info for multiple titles at once (batching)
        /// </summary>
        public async Task<Dictionary<string, PageInfo>> GetPagesInfo(List<string> titles)
        {
            if (titles == null || !titles.Any()) return new Dictionary<string, PageInfo>();

            var result = new Dictionary<string, PageInfo>(StringComparer.OrdinalIgnoreCase);
            
            // MediaWiki batch limit is 50 for normal users
            const int BatchLimit = 50;

            for (int i = 0; i < titles.Count; i += BatchLimit)
            {
                var batch = titles.Skip(i).Take(BatchLimit).ToList();
                var titlesStr = string.Join("|", batch.Select(Uri.EscapeDataString));
                // Include rvprop=content and rvslots=main to get the actual page text
                // Added prop=langlinks to get Vietnamese titles from English wiki
                var url = $"{_apiUrl}?action=query&prop=info|revisions|langlinks&lllang=vi&rvprop=timestamp|content&rvslots=main&titles={titlesStr}&format=json";

                var response = await _http.GetStringAsync(url);
                var json = JObject.Parse(response);
                var pages = json["query"]?["pages"] as JObject;

                if (pages != null)
                {
                    foreach (var property in pages.Properties())
                    {
                        var page = property.Value;
                        var title = page["title"]?.ToString();
                        
                        if (title != null)
                        {
                            if (page["missing"] != null)
                            {
                                // Store original search title as null in result
                                result[title] = null;
                            }
                            else
                            {
                                // Get content from revisions (try multiple paths for compatibility)
                                string content = null;
                                var revisions = page["revisions"];
                                if (revisions != null && revisions.HasValues)
                                {
                                    var firstRev = revisions[0];
                                    content = firstRev["slots"]?["main"]?["*"]?.ToString() 
                                           ?? firstRev["*"]?.ToString();
                                }

                                if (string.IsNullOrEmpty(content))
                                {
                                    Logger.Log($"Warning: No content found for page '{title}'");
                                }

                                // Extract Vietnamese title from langlinks if available
                                string viTitle = null;
                                var langlinks = page["langlinks"] as JArray;
                                if (langlinks != null && langlinks.Count > 0)
                                {
                                    viTitle = langlinks[0]["*"]?.ToString();
                                }

                                result[title] = new PageInfo
                                {
                                    Title = title,
                                    VietnameseTitle = viTitle,
                                    FullUrl = $"{_baseWikiUrl.TrimEnd('/')}/{Uri.EscapeDataString(title).Replace("%20", "_")}",
                                    PageId = page["pageid"]?.ToString(),
                                    LastRevisionTimestamp = revisions?[0]?["timestamp"]?.ToString(),
                                    Touched = page["touched"]?.ToString(),
                                    Content = content
                                };
                            }
                        }
                    }
                }
                
                // Small delay between batches if there are more
                if (i + BatchLimit < titles.Count)
                    await Task.Delay(200);
            }

            return result;
        }

        /// <summary>
        /// Get pages that don't have a language link to the specified language
        /// </summary>
        public async Task<List<string>> GetPagesWithoutLangLink(string targetLang = "vi", int limit = 50)
        {
            var missingPages = new List<string>();
            string continueToken = null;
            int fetched = 0;

            do
            {
                var batchLimit = Math.Min(50, limit - fetched);
                var url = $"{_apiUrl}?action=query&list=allpages&aplimit={batchLimit}&apnamespace=0&format=json";
                if (!string.IsNullOrEmpty(continueToken))
                    url += $"&apcontinue={continueToken}";

                var json = JObject.Parse(await _http.GetStringAsync(url));
                var allPages = json["query"]?["allpages"] as JArray;

                if (allPages == null || !allPages.HasValues)
                    break;

                // Get titles for this batch
                var titles = allPages.Select(p => p["title"].ToString()).ToList();
                
                // Check language links for these titles
                var langUrl = $"{_apiUrl}?action=query&prop=langlinks&lllang={targetLang}&titles={string.Join("|", titles.Select(Uri.EscapeDataString))}&format=json";
                var langJson = JObject.Parse(await _http.GetStringAsync(langUrl));
                var pagesWithLang = langJson["query"]["pages"];

                foreach (var page in pagesWithLang)
                {
                    var pageObj = page.First;
                    var title = pageObj["title"].ToString();
                    
                    // If langlinks is missing or empty, this page doesn't have a Vietnamese version
                    if (pageObj["langlinks"] == null || !pageObj["langlinks"].Any())
                    {
                        missingPages.Add(title);
                    }
                }

                fetched += allPages.Count;
                continueToken = json["continue"]?["apcontinue"]?.ToString();

            } while (continueToken != null && fetched < limit);

            return missingPages;
        }
    }

    public class PageInfo
    {
        public string Title { get; set; }
        public string VietnameseTitle { get; set; }
        public string FullUrl { get; set; }
        public string PageId { get; set; }
        public string LastRevisionTimestamp { get; set; }
        public string Touched { get; set; }
        public string Content { get; set; }
    }
}

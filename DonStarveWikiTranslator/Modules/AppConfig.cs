using System;
using System.Configuration;

namespace DonStarveWikiTranslator.Modules
{
    /// <summary>
    /// Manages application configuration settings
    /// </summary>
    public static class AppConfig
    {
        /// <summary>
        /// English wiki base URL
        /// </summary>
        public static string EnglishWikiUrl => 
            ConfigurationManager.AppSettings["EnglishWikiUrl"] 
            ?? "https://dontstarve.wiki.gg/wiki";

        /// <summary>
        /// Vietnamese wiki base URL
        /// </summary>
        public static string VietnameseWikiUrl => 
            ConfigurationManager.AppSettings["VietnameseWikiUrl"] 
            ?? "https://dontstarve.wiki.gg/vi/wiki";

        /// <summary>
        /// English wiki API endpoint
        /// </summary>
        public static string EnglishApiUrl => 
            ConfigurationManager.AppSettings["EnglishApiUrl"] 
            ?? "https://dontstarve.wiki.gg/api.php";

        /// <summary>
        /// Vietnamese wiki API endpoint
        /// </summary>
        public static string VietnameseApiUrl => 
            ConfigurationManager.AppSettings["VietnameseApiUrl"] 
            ?? "https://dontstarve.wiki.gg/vi/api.php";

        /// <summary>
        /// How many hours before cached data is considered stale
        /// </summary>
        public static int CacheExpirationHours
        {
            get
            {
                var value = ConfigurationManager.AppSettings["CacheExpirationHours"];
                return int.TryParse(value, out int hours) ? hours : 24;
            }
        }

        /// <summary>
        /// Database connection string
        /// </summary>
        public static string ConnectionString => 
            ConfigurationManager.ConnectionStrings["WikiDatabase"]?.ConnectionString 
            ?? "Data Source=WikiTranslator.db;Version=3;";

        /// <summary>
        /// Validate that all required configuration values are present
        /// </summary>
        public static void Validate()
        {
            if (string.IsNullOrEmpty(EnglishApiUrl))
                throw new ConfigurationErrorsException("EnglishApiUrl is not configured");
            
            if (string.IsNullOrEmpty(VietnameseApiUrl))
                throw new ConfigurationErrorsException("VietnameseApiUrl is not configured");
            
            if (string.IsNullOrEmpty(ConnectionString))
                throw new ConfigurationErrorsException("Database connection string is not configured");
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonStarveWikiTranslator.Models
{
    /// <summary>
    /// Represents a wiki article with both English and Vietnamese content
    /// </summary>
    public class WikiArticle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        [Index(IsUnique = true)]
        public string Title { get; set; }

        /// <summary>
        /// Vietnamese title of the article (useful for cross-wiki links)
        /// </summary>
        [MaxLength(500)]
        public string VietnameseTitle { get; set; }

        /// <summary>
        /// Direct URL to the English wiki article
        /// </summary>
        [MaxLength(1000)]
        public string EnglishUrl { get; set; }

        /// <summary>
        /// Direct URL to the Vietnamese wiki article
        /// </summary>
        [MaxLength(1000)]
        public string VietnameseUrl { get; set; }

        /// <summary>
        /// Full English wiki markup content
        /// </summary>
        public string EnglishContent { get; set; }

        /// <summary>
        /// Full Vietnamese wiki markup content (null if doesn't exist)
        /// </summary>
        public string VietnameseContent { get; set; }

        /// <summary>
        /// Last update timestamp from English wiki
        /// </summary>
        public DateTime? EnglishLastUpdate { get; set; }

        /// <summary>
        /// Last update timestamp from Vietnamese wiki
        /// </summary>
        public DateTime? VietnameseLastUpdate { get; set; }

        /// <summary>
        /// When this data was last synced from the API
        /// </summary>
        public DateTime LastSyncDate { get; set; }

        /// <summary>
        /// Current translation status
        /// </summary>
        public TranslationStatus Status { get; set; }

        /// <summary>
        /// Calculated property: days Vietnamese version is behind English
        /// </summary>
        [NotMapped]
        public int DaysBehind
        {
            get
            {
                if (EnglishLastUpdate.HasValue && VietnameseLastUpdate.HasValue)
                {
                    return (EnglishLastUpdate.Value - VietnameseLastUpdate.Value).Days;
                }
                return 0;
            }
        }

        /// <summary>
        /// Check if cached data is stale based on expiration hours
        /// </summary>
        public bool IsStale(int expirationHours = 24)
        {
            return (DateTime.Now - LastSyncDate).TotalHours >= expirationHours;
        }
    }
}

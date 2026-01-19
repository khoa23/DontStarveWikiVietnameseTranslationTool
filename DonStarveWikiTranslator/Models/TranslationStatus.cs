namespace DonStarveWikiTranslator.Models
{
    /// <summary>
    /// Status of article translation
    /// </summary>
    public enum TranslationStatus
    {
        /// <summary>
        /// Article does not exist in Vietnamese wiki
        /// </summary>
        Missing = 0,

        /// <summary>
        /// Vietnamese version exists but is older than English version
        /// </summary>
        Outdated = 1,

        /// <summary>
        /// Vietnamese version is up to date with English version
        /// </summary>
        UpToDate = 2,

        /// <summary>
        /// Translation is currently in progress
        /// </summary>
        InProgress = 3
    }
}

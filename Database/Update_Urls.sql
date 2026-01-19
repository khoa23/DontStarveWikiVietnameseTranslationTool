-- =============================================
-- Migration Script: Add URL columns
-- Don't Starve Wiki Translator
-- =============================================

USE WikiTranslator;
GO

-- Add EnglishUrl
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[WikiArticles]') 
    AND name = N'EnglishUrl'
)
BEGIN
    PRINT 'Adding EnglishUrl column...';
    ALTER TABLE [dbo].[WikiArticles] ADD [EnglishUrl] NVARCHAR(1000) NULL;
END

-- Add VietnameseUrl
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[WikiArticles]') 
    AND name = N'VietnameseUrl'
)
BEGIN
    PRINT 'Adding VietnameseUrl column...';
    ALTER TABLE [dbo].[WikiArticles] ADD [VietnameseUrl] NVARCHAR(1000) NULL;
END

PRINT 'Migration completed.';
GO

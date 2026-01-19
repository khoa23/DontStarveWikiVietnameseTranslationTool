-- =============================================
-- Migration Script: Add VietnameseTitle column
-- Don't Starve Wiki Translator
-- =============================================

USE WikiTranslator;
GO

-- Check if column exists before adding
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[WikiArticles]') 
    AND name = N'VietnameseTitle'
)
BEGIN
    PRINT 'Adding VietnameseTitle column to WikiArticles table...';
    
    ALTER TABLE [dbo].[WikiArticles] 
    ADD [VietnameseTitle] NVARCHAR(500) NULL;
    
    PRINT 'Column added successfully.';
END
ELSE
BEGIN
    PRINT 'Column VietnameseTitle already exists.';
END
GO

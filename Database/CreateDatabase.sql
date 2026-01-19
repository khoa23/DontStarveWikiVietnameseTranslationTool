-- =============================================
-- SQL Server Database Creation Script
-- Don't Starve Wiki Translator
-- =============================================

-- Create Database
USE master;
GO

-- Drop database if exists (for testing purposes)
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'WikiTranslator')
BEGIN
    ALTER DATABASE WikiTranslator SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE WikiTranslator;
END
GO

-- Create new database
CREATE DATABASE WikiTranslator
ON PRIMARY 
(
    NAME = N'WikiTranslator_Data',
    FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\WikiTranslator.mdf',
    SIZE = 10MB,
    MAXSIZE = UNLIMITED,
    FILEGROWTH = 5MB
)
LOG ON 
(
    NAME = N'WikiTranslator_Log',
    FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\WikiTranslator_log.ldf',
    SIZE = 5MB,
    MAXSIZE = 100MB,
    FILEGROWTH = 5MB
);
GO

-- Use the new database
USE WikiTranslator;
GO

-- =============================================
-- Create Tables
-- =============================================

-- Create WikiArticles table
CREATE TABLE [dbo].[WikiArticles]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(500) NOT NULL,
    [VietnameseTitle] NVARCHAR(500) NULL,
    [EnglishUrl] NVARCHAR(1000) NULL,
    [VietnameseUrl] NVARCHAR(1000) NULL,
    [EnglishContent] NVARCHAR(MAX) NULL,
    [VietnameseContent] NVARCHAR(MAX) NULL,
    [EnglishLastUpdate] DATETIME2 NULL,
    [VietnameseLastUpdate] DATETIME2 NULL,
    [LastSyncDate] DATETIME2 NOT NULL,
    [Status] INT NOT NULL DEFAULT(0),
    
    CONSTRAINT [PK_WikiArticles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_WikiArticles_Title] UNIQUE NONCLUSTERED ([Title] ASC)
);
GO

-- Create indexes for better performance
CREATE NONCLUSTERED INDEX [IX_WikiArticles_Status] 
ON [dbo].[WikiArticles]([Status] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_WikiArticles_LastSyncDate] 
ON [dbo].[WikiArticles]([LastSyncDate] DESC);
GO

-- =============================================
-- Sample data (optional - remove if not needed)
-- =============================================

-- Insert sample article
INSERT INTO [dbo].[WikiArticles] 
    ([Title], [EnglishContent], [VietnameseContent], [EnglishLastUpdate], [VietnameseLastUpdate], [LastSyncDate], [Status])
VALUES 
    (N'Sample Article', 
     N'This is a sample English content.', 
     NULL, 
     GETDATE(), 
     NULL, 
     GETDATE(), 
     0); -- Missing status
GO

-- =============================================
-- Display database info
-- =============================================

PRINT '============================================='
PRINT 'Database created successfully!'
PRINT '============================================='
PRINT 'Database Name: WikiTranslator'
PRINT 'Server: .\SQLEXPRESS'
PRINT 'Tables created:'
PRINT '  - WikiArticles'
PRINT '============================================='

-- Show table structure
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'WikiArticles'
ORDER BY ORDINAL_POSITION;
GO

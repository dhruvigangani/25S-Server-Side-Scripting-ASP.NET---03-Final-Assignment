-- SQL Server Database Setup Script for ShiftSchedularApplication
-- Run this script in SQL Server Management Studio or Azure Data Studio

-- Create the database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ShiftSchedularApplication')
BEGIN
    CREATE DATABASE ShiftSchedularApplication;
END
GO

USE ShiftSchedularApplication;
GO

-- Note: The Entity Framework migrations will create the actual tables
-- This script just ensures the database exists

-- Verify the database was created
SELECT name, size, size * 8 / 1024 AS 'Size (MB)' 
FROM sys.database_files;
GO

PRINT 'Database ShiftSchedularApplication is ready for Entity Framework migrations.';
PRINT 'Run: dotnet ef database update';
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'OnlineClothingStore')
BEGIN
    CREATE DATABASE OnlineClothingStore;
END;
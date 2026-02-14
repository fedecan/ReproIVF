USE [ReproIVF];
GO

IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Users](
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Username] NVARCHAR(450) NOT NULL,
        [PasswordHash] NVARCHAR(MAX) NOT NULL,
        [PasswordSalt] NVARCHAR(MAX) NOT NULL,
        [Role] NVARCHAR(MAX) NOT NULL
    );

    CREATE UNIQUE INDEX [IX_Users_Username] ON [dbo].[Users]([Username]);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'admin')
BEGIN
    INSERT INTO dbo.Users (Username, PasswordHash, PasswordSalt, Role)
    VALUES (
        'admin',
        'HPK9A2zspQ2aFMr3TFhlOwIvcg5iH8D7Sa8FyVmg6ts=',
        '0ss0OSmVN3xN8f+wKv2I1g==',
        'Admin'
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'client')
BEGIN
    INSERT INTO dbo.Users (Username, PasswordHash, PasswordSalt, Role)
    VALUES (
        'client',
        '9Ia0FJ7MmfWGL7wWqta9t3Lck2B+o/gGWV8w+JS62Pc=',
        'MwkeFCubWwYZPvxdVNeZqA==',
        'Client'
    );
END
GO

IF OBJECT_ID('dbo.__EFMigrationsHistory', 'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1
    FROM dbo.__EFMigrationsHistory
    WHERE MigrationId = '20260214015106_AddUsersAuth'
)
BEGIN
    INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20260214015106_AddUsersAuth', '8.0.2');
END
GO

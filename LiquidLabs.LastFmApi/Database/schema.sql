-- LastFm Albums Cache - SQL Server schema

IF OBJECT_ID('dbo.Albums', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Albums;
END
GO

CREATE TABLE dbo.Albums
(
    Id              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Albums PRIMARY KEY,

    Artist          NVARCHAR(200) NOT NULL,
    Name            NVARCHAR(300) NOT NULL,

    Mbid            NVARCHAR(50)  NULL,
    Url             NVARCHAR(500) NULL,

    Listeners       INT NULL,
    Playcount       INT NULL,
    Rank            INT NULL,

    ReleaseDate     NVARCHAR(100) NULL,

    ImageSmall      NVARCHAR(500) NULL,
    ImageMedium     NVARCHAR(500) NULL,
    ImageLarge      NVARCHAR(500) NULL,

    IsDetailFetched BIT NOT NULL CONSTRAINT DF_Albums_IsDetailFetched DEFAULT (0),

    -- timestamps
    FetchedAtUtc    DATETIME2(0) NOT NULL CONSTRAINT DF_Albums_FetchedAtUtc DEFAULT (SYSUTCDATETIME()),
    UpdatedAtUtc    DATETIME2(0) NOT NULL CONSTRAINT DF_Albums_UpdatedAtUtc DEFAULT (SYSUTCDATETIME())
);
GO

CREATE UNIQUE INDEX UX_Albums_Artist_Name
ON dbo.Albums (Artist, Name);
GO

CREATE INDEX IX_Albums_Artist_UpdatedAtUtc
ON dbo.Albums (Artist, UpdatedAtUtc DESC);
GO

CREATE INDEX IX_Albums_Artist_Name
ON dbo.Albums (Artist, Name);
GO

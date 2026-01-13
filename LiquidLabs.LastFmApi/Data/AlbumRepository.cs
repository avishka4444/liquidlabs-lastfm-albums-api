using System.Data;
using Microsoft.Data.SqlClient;
using LiquidLabs.LastFmApi.Models;

namespace LiquidLabs.LastFmApi.Data;

public sealed class AlbumRepository : IAlbumRepository
{
    private readonly string _connectionString;

    public AlbumRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection missing");
    }

    public async Task<IReadOnlyList<Album>> GetByArtistAsync(string artist)
    {
        var result = new List<Album>();

        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT Id, Artist, Name, Mbid, Url, Listeners, Playcount, Rank, ReleaseDate,
       ImageSmall, ImageMedium, ImageLarge, IsDetailFetched, FetchedAtUtc, UpdatedAtUtc
FROM dbo.Albums
WHERE Artist = @Artist
ORDER BY Rank ASC, Name ASC;
";
        cmd.Parameters.Add(new SqlParameter("@Artist", SqlDbType.NVarChar, 200) { Value = artist });

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new Album
            {
                Id = reader.GetInt32(0),
                Artist = reader.GetString(1),
                Name = reader.GetString(2),
                Mbid = reader.IsDBNull(3) ? null : reader.GetString(3),
                Url = reader.IsDBNull(4) ? null : reader.GetString(4),
                Listeners = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                Playcount = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                Rank = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                ReleaseDate = reader.IsDBNull(8) ? null : reader.GetString(8),
                ImageSmall = reader.IsDBNull(9) ? null : reader.GetString(9),
                ImageMedium = reader.IsDBNull(10) ? null : reader.GetString(10),
                ImageLarge = reader.IsDBNull(11) ? null : reader.GetString(11),
                IsDetailFetched = reader.GetBoolean(12),
                FetchedAtUtc = reader.GetDateTime(13),
                UpdatedAtUtc = reader.GetDateTime(14)
            });
        }

        return result;
    }

    public async Task<Album?> GetByIdAsync(int id)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT Id, Artist, Name, Mbid, Url, Listeners, Playcount, Rank, ReleaseDate,
       ImageSmall, ImageMedium, ImageLarge, IsDetailFetched, FetchedAtUtc, UpdatedAtUtc
FROM dbo.Albums
WHERE Id = @Id;
";
        cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        return new Album
        {
            Id = reader.GetInt32(0),
            Artist = reader.GetString(1),
            Name = reader.GetString(2),
            Mbid = reader.IsDBNull(3) ? null : reader.GetString(3),
            Url = reader.IsDBNull(4) ? null : reader.GetString(4),
            Listeners = reader.IsDBNull(5) ? null : reader.GetInt32(5),
            Playcount = reader.IsDBNull(6) ? null : reader.GetInt32(6),
            Rank = reader.IsDBNull(7) ? null : reader.GetInt32(7),
            ReleaseDate = reader.IsDBNull(8) ? null : reader.GetString(8),
            ImageSmall = reader.IsDBNull(9) ? null : reader.GetString(9),
            ImageMedium = reader.IsDBNull(10) ? null : reader.GetString(10),
            ImageLarge = reader.IsDBNull(11) ? null : reader.GetString(11),
            IsDetailFetched = reader.GetBoolean(12),
            FetchedAtUtc = reader.GetDateTime(13),
            UpdatedAtUtc = reader.GetDateTime(14)
        };
    }

    public async Task UpsertTopAlbumsAsync(string artist, IReadOnlyList<Album> albums)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var tx = await conn.BeginTransactionAsync();

        try
        {
            foreach (var a in albums)
            {
                var cmd = conn.CreateCommand();
                cmd.Transaction = (SqlTransaction)tx;

                cmd.CommandText = @"
MERGE dbo.Albums AS target
USING (SELECT @Artist AS Artist, @Name AS Name) AS source
ON target.Artist = source.Artist AND target.Name = source.Name
WHEN MATCHED THEN
    UPDATE SET
        Mbid = @Mbid,
        Url = @Url,
        Listeners = @Listeners,
        Playcount = @Playcount,
        Rank = @Rank,
        ImageSmall = @ImageSmall,
        ImageMedium = @ImageMedium,
        ImageLarge = @ImageLarge,
        UpdatedAtUtc = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT (Artist, Name, Mbid, Url, Listeners, Playcount, Rank, ImageSmall, ImageMedium, ImageLarge, IsDetailFetched, FetchedAtUtc, UpdatedAtUtc)
    VALUES (@Artist, @Name, @Mbid, @Url, @Listeners, @Playcount, @Rank, @ImageSmall, @ImageMedium, @ImageLarge, 0, SYSUTCDATETIME(), SYSUTCDATETIME());
";

                cmd.Parameters.AddWithValue("@Artist", artist);
                cmd.Parameters.AddWithValue("@Name", a.Name);
                cmd.Parameters.AddWithValue("@Mbid", (object?)a.Mbid ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Url", (object?)a.Url ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Listeners", (object?)a.Listeners ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Playcount", (object?)a.Playcount ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Rank", (object?)a.Rank ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ImageSmall", (object?)a.ImageSmall ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ImageMedium", (object?)a.ImageMedium ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ImageLarge", (object?)a.ImageLarge ?? DBNull.Value);

                await cmd.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}

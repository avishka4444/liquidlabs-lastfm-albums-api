using LiquidLabs.LastFmApi.Data;
using LiquidLabs.LastFmApi.LastFm;
using LiquidLabs.LastFmApi.Models;

namespace LiquidLabs.LastFmApi.Services;

public sealed class AlbumService : IAlbumService
{
    private readonly IAlbumRepository _repo;
    private readonly ILastFmClient _lastFm;

    private const string HardcodedArtist = "The Beatles";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(30);

    public AlbumService(IAlbumRepository repo, ILastFmClient lastFm)
    {
        _repo = repo;
        _lastFm = lastFm;
    }

    public async Task<IReadOnlyList<Album>> GetTopAlbumsAsync()
    {
        // Try the cache
        var cached = await _repo.GetByArtistAsync(HardcodedArtist);

        if (cached.Count > 0)
        {
            var lastUpdated = cached.Max(x => x.UpdatedAtUtc);
            var isFresh = DateTime.UtcNow - lastUpdated <= CacheTtl;

            if (isFresh)
                return cached;
        }

        // Fetch from Last.fm
        var top = await _lastFm.GetTopAlbumsAsync(HardcodedArtist);

        // Map
        var albums = MapTopAlbumsToDomain(HardcodedArtist, top);

        // Upsert into DB
        await _repo.UpsertTopAlbumsAsync(HardcodedArtist, albums);

        // Return from DB
        return await _repo.GetByArtistAsync(HardcodedArtist);
    }

    public async Task<Album?> GetAlbumByIdAsync(int id)
    {
        // Get album using ID
        return await _repo.GetByIdAsync(id);
    }

    private static List<Album> MapTopAlbumsToDomain(string artist, TopAlbumsResponse resp)
    {
        var list = new List<Album>();

        var albums = resp.Topalbums?.Album ?? [];
        foreach (var a in albums)
        {
            var (small, medium, large) = PickImages(a.Image);

            list.Add(new Album
            {
                Artist = artist,
                Name = a.Name,
                Mbid = string.IsNullOrWhiteSpace(a.Mbid) ? null : a.Mbid,
                Url = a.Url,

                Listeners = a.Listeners,
                Playcount = a.Playcount,
                Rank = a.Attr?.Rank,

                ImageSmall = small,
                ImageMedium = medium,
                ImageLarge = large,
            });
        }

        return list;
    }

    private static (string? small, string? medium, string? large) PickImages(List<ImageDto> images)
    {
        string? Get(string size) =>
            images.FirstOrDefault(x => string.Equals(x.Size, size, StringComparison.OrdinalIgnoreCase))?.Text;

        return (Get("small"), Get("medium"), Get("large"));
    }
}

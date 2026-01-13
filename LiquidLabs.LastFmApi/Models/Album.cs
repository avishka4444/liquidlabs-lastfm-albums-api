namespace LiquidLabs.LastFmApi.Models;

public sealed class Album
{
    public int Id { get; set; }

    public string Artist { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Mbid { get; set; }
    public string? Url { get; set; }

    // Statistics
    public int? Listeners { get; set; }
    public int? Playcount { get; set; }

    // Ranking
    public int? Rank { get; set; }

    // Metadata
    public string? ReleaseDate { get; set; }

    // Images
    public string? ImageSmall { get; set; }
    public string? ImageMedium { get; set; }
    public string? ImageLarge { get; set; }

    // Cache flag
    public bool IsDetailFetched { get; set; }

    // Timestamps
    public DateTime FetchedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}

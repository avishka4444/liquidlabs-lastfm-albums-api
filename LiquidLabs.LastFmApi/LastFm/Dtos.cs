using System.Text.Json.Serialization;

namespace LiquidLabs.LastFmApi.LastFm;

public sealed class TopAlbumsResponse
{
    [JsonPropertyName("topalbums")]
    public TopAlbums? Topalbums { get; init; }
}

public sealed class TopAlbums
{
    [JsonPropertyName("album")]
    public List<AlbumDto> Album { get; init; } = [];
}

public sealed class AlbumDto
{
    [JsonPropertyName("@attr")]
    public RankAttribute? Attr { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    [JsonPropertyName("mbid")]
    public string? Mbid { get; init; }

    [JsonPropertyName("url")]
    public string Url { get; init; } = "";

    // Can be number OR string in Last.fm JSON
    [JsonPropertyName("playcount")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? Playcount { get; init; }

    [JsonPropertyName("listeners")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? Listeners { get; init; }

    [JsonPropertyName("image")]
    public List<ImageDto> Image { get; init; } = [];
}

public sealed class RankAttribute
{
    [JsonPropertyName("rank")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? Rank { get; init; }
}

public sealed class ImageDto
{
    // Last.fm uses "#text" for the URL
    [JsonPropertyName("#text")]
    public string? Text { get; init; }

    [JsonPropertyName("size")]
    public string Size { get; init; } = "";
}

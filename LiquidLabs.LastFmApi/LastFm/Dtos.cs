namespace LiquidLabs.LastFmApi.LastFm;

public sealed class TopAlbumsResponse
{
    public TopAlbums? Topalbums { get; init; }
}

public sealed class TopAlbums
{
    public List<AlbumDto> Album { get; init; } = [];
}

public sealed class AlbumDto
{
    public string Name { get; init; } = "";
    public string? Mbid { get; init; }
    public string Url { get; init; } = "";
    public string? Playcount { get; init; }
    public string? Listeners { get; init; }
    public List<ImageDto> Image { get; init; } = [];
}

public sealed class ImageDto
{
    public string Size { get; init; } = "";
    public string Url { get; init; } = "";
}

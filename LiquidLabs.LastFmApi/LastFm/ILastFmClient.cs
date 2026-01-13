using LiquidLabs.LastFmApi.LastFm;

public interface ILastFmClient
{
    Task<IReadOnlyList<AlbumDto>> GetTopAlbumsAsync(string artist);
}
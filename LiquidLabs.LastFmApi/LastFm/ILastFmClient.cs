namespace LiquidLabs.LastFmApi.LastFm;

public interface ILastFmClient
{
    Task<TopAlbumsResponse> GetTopAlbumsAsync(string artist);
}

using LiquidLabs.LastFmApi.Models;

namespace LiquidLabs.LastFmApi.Services;

public interface IAlbumService
{
    Task<IReadOnlyList<Album>> GetTopAlbumsAsync();
    Task<Album?> GetAlbumByIdAsync(int id);
}

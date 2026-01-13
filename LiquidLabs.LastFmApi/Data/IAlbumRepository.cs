using LiquidLabs.LastFmApi.Models;

namespace LiquidLabs.LastFmApi.Data;

public interface IAlbumRepository
{
    Task UpsertTopAlbumsAsync(string artist, IReadOnlyList<Album> albums);
    Task<IReadOnlyList<Album>> GetByArtistAsync(string artist);
    Task<Album?> GetByIdAsync(int id);
}

using System.Net.Http.Json;
using LiquidLabs.LastFmApi.LastFm;

public sealed class LastFmClient : ILastFmClient
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public LastFmClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["LASTFM_API_KEY"]
            ?? throw new InvalidOperationException("LASTFM_API_KEY not set");
    }

    public async Task<IReadOnlyList<AlbumDto>> GetTopAlbumsAsync(string artist)
    {
        var url =
            $"?method=artist.gettopalbums" +
            $"&artist={Uri.EscapeDataString(artist)}" +
            $"&api_key={_apiKey}" +
            $"&format=json";

        var response = await _http.GetFromJsonAsync<TopAlbumsResponse>(url);

        return response?.Topalbums?.Album ?? [];
    }
}

using System.Text.Json;

namespace LiquidLabs.LastFmApi.LastFm;

public sealed class LastFmClient : ILastFmClient
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public LastFmClient(HttpClient http, IConfiguration config)
    {
        _http = http;

        _apiKey = config["LASTFM_API_KEY"]
            ?? throw new InvalidOperationException("Missing LASTFM_API_KEY (env/appsettings).");
    }

    public Task<TopAlbumsResponse> GetTopAlbumsAsync(string artist)
    {
        var url =
            $"/2.0/?method=artist.gettopalbums&artist={Uri.EscapeDataString(artist)}&api_key={_apiKey}&format=json";

        return GetJsonAsync<TopAlbumsResponse>(url);
    }

    private async Task<T> GetJsonAsync<T>(string relativeUrl)
    {
        using var resp = await _http.GetAsync(relativeUrl);
        resp.EnsureSuccessStatusCode();

        var json = await resp.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<T>(json, JsonOpts);

        if (data is null)
            throw new InvalidOperationException("Failed to deserialize Last.fm response.");

        return data;
    }
}

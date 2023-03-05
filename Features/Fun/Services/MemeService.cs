using GodBot.Features.Fun.Models;

namespace GodBot.Features.Fun.Services;

public class MemeService
{
    private readonly HttpClient _client;
    public MemeService()
    {
        _client = new HttpClient();
    }

    public async Task<Meme?> GetRandomMemeAsync()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"https://api.humorapi.com/memes/random?api-key={Environment.GetEnvironmentVariable("HUMOR_API_KEY")}")
        };

        using var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return Meme.FromJson(await response.Content.ReadAsStringAsync());
    }
}
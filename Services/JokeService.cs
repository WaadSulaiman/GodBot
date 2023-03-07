using GodBot.Models;

namespace GodBot.Services;

public class JokeService
{
    private readonly HttpClient _client;
    public JokeService()
    {
        _client = new HttpClient();
    }

    public async Task<Joke?> GetRandomJokeAsync()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"https://api.humorapi.com/jokes/random?api-key={Environment.GetEnvironmentVariable("HUMOR_API_KEY")}")
        };

        using var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return Joke.FromJson(await response.Content.ReadAsStringAsync());
    }
}
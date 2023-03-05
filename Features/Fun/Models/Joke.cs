using Newtonsoft.Json;

namespace GodBot.Features.Fun.Models;
public class Joke
{
    [JsonProperty("id")]
    public string Id { get; set; } = default!;
    
    [JsonProperty("joke")]
    public string Content { get; set; } = default!;

    public static Joke? FromJson(string json) => JsonConvert.DeserializeObject<Joke>(json);
}
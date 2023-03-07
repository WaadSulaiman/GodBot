using Newtonsoft.Json;

namespace GodBot.Models;
public class Meme
{
    [JsonProperty("id")]
    public string Id { get; set; } = default!;
    
    [JsonProperty("url")]
    public string Url { get; set; } = default!;

    [JsonProperty("type")]
    public string Type { get; set; } = default!;

    public static Meme? FromJson(string json) => JsonConvert.DeserializeObject<Meme>(json);
}
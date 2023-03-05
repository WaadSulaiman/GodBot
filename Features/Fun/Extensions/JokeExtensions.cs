using GodBot.Features.Fun.Models;
using Newtonsoft.Json;

namespace GodBot.Features.Fun.Extensions;
public static class JokeExtensions
{
    public static string ToJson(this Joke self) => JsonConvert.SerializeObject(self, CustomJsonConverter.Settings);
}
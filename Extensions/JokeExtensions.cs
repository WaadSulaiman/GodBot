using GodBot.Models;
using GodBot.Utilities;
using Newtonsoft.Json;

namespace GodBot.Extensions;
public static class JokeExtensions
{
    public static string ToJson(this Joke self) => JsonConvert.SerializeObject(self, CustomJsonConverter.Settings);
}
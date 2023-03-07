using GodBot.Models;
using GodBot.Utilities;
using Newtonsoft.Json;

namespace GodBot.Extensions;
public static class MemeExtensions
{
    public static string ToJson(this Meme self) => JsonConvert.SerializeObject(self, CustomJsonConverter.Settings);
}
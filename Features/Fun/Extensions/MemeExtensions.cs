using GodBot.Features.Fun.Models;
using Newtonsoft.Json;

namespace GodBot.Features.Fun.Extensions;
public static class MemeExtensions
{
    public static string ToJson(this Meme self) => JsonConvert.SerializeObject(self, CustomJsonConverter.Settings);
}
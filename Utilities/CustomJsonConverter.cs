using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GodBot.Utilities;
internal static class CustomJsonConverter
{
    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
    };
}
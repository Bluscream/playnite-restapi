using System.Text.Json.Serialization;
using System.Text.Json;

namespace RestAPI {
    internal static class Extensions {
        internal static string ToJson(this object obj) {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions() { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
        }
    }
}

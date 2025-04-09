using System.Text.Json;

namespace week7Project.Utils;

public sealed class JsonExporter
{
    private static readonly Lazy<JsonExporter> _instance = new(() => new JsonExporter());
    public static JsonExporter Instance => _instance.Value;
    
    private JsonExporter() { }

    public string Export<T>(IEnumerable<T> data, List<string>? properties = null)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        if (properties == null || !properties.Any())
            return JsonSerializer.Serialize(data, options);

        return JsonSerializer.Serialize(
            data.Select(item => FilterProperties(item!, properties)),
            options
        );
    }

    private static Dictionary<string, object?> FilterProperties<T>(T item, IEnumerable<string> properties)
    {
        var dict = new Dictionary<string, object?>();
        foreach (var prop in properties)
        {
            var value = typeof(T).GetProperty(prop)?.GetValue(item);
            dict[prop] = value;
        }
        return dict;
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Linq;

namespace week7Project.Utils
{
    public sealed class Utils
    {
        // Singleton implementation.
        private static readonly Lazy<Utils> lazyInstance = new(() => new Utils());
        public static Utils Instance => lazyInstance.Value;
        private Utils() { }

        /// <summary>
        /// Exports a list of objects to JSON. If selectedColumns is provided (not null and not empty),
        /// only properties with names in the list will be exported.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="data">The enumerable collection of data</param>
        /// <param name="selectedColumns">Optional list of property names to export</param>
        /// <returns>JSON string</returns>
        public string ExportToJson<T>(IEnumerable<T> data, List<string>? selectedColumns = null)
        {
            if (selectedColumns != null && selectedColumns.Any())
            {
                // Prepare a list of dictionaries that hold only the selected properties.
                var filteredData = new List<Dictionary<string, object?>>();
                // Get property infos for type T.
                PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                // Filter properties based on selected columns (case-insensitive match).
                var propertiesToExport = properties
                    .Where(p => selectedColumns.Any(col => string.Equals(col, p.Name, StringComparison.OrdinalIgnoreCase)))
                    .ToArray();

                foreach (var item in data)
                {
                    var dict = new Dictionary<string, object?>();
                    foreach (var prop in propertiesToExport)
                    {
                        dict[prop.Name] = prop.GetValue(item);
                    }
                    filteredData.Add(dict);
                }
                return JsonSerializer.Serialize(filteredData, new JsonSerializerOptions { WriteIndented = true });
            }
            else
            {
                // If no specific columns selected, export the entire objects.
                return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            }
        }
    }
}

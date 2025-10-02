using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API_Lens
{
    internal class ApiResponseFormatter
    {
        public static object TryParseJson(string body)
        {
            try
            {
                using JsonDocument doc = JsonDocument.Parse(body);
                return JsonSerializer.Deserialize<object>(body);
            }
            catch
            {
                return body;
            }
        }

        public static async Task<string> GetFormattedResponseAsync(string url, IEnumerable<ApiParameter> parameters)
        {
            try
            {
                using HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);
                HttpResponseMessage response = await client.GetAsync(url);

                StringBuilder sb = new StringBuilder();

                sb.AppendLine($"Status: {(int)response.StatusCode} {response.ReasonPhrase}");
                sb.AppendLine(new string('-', 50));

                sb.AppendLine("HEADERS:");
                foreach (var header in response.Headers)
                    sb.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
                foreach (var header in response.Content.Headers)
                    sb.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");

                sb.AppendLine(new string('-', 50));

                sb.AppendLine("PARAMETERS:");
                foreach (var param in parameters)
                    sb.AppendLine($"{param.Key}: {param.Value}");

                sb.AppendLine(new string('-', 50));

                string body = await response.Content.ReadAsStringAsync();
                sb.AppendLine("BODY:");
                sb.AppendLine(FormatJsonToReadableText(body, 1));

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private static string FormatJsonToReadableText(string json, int indentLevel)
        {
            try
            {
                using JsonDocument doc = JsonDocument.Parse(json);
                return FormatElement(doc.RootElement, indentLevel);
            }
            catch
            {
                string indent = new string('\t', indentLevel);
                return indent + json;
            }
        }

        private static string FormatElement(JsonElement element, int indentLevel)
        {
            string indent = new string('\t', indentLevel);
            StringBuilder sb = new StringBuilder();

            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var prop in element.EnumerateObject())
                    {
                        string[] ignoredKeys = { "metadata", "internal", "debug" };
                        if (Array.Exists(ignoredKeys, k => k.Equals(prop.Name, StringComparison.OrdinalIgnoreCase)))
                            continue;

                        sb.Append(indent + prop.Name + ": ");
                        if (prop.Value.ValueKind == JsonValueKind.Object || prop.Value.ValueKind == JsonValueKind.Array)
                        {
                            sb.AppendLine();
                            sb.Append(FormatElement(prop.Value, indentLevel + 1));
                        }
                        else
                        {
                            string value = prop.Value.ToString();
                            string[] lines = value.Split('\n');
                            if (lines.Length > 1)
                            {
                                sb.AppendLine(lines[0]);
                                for (int i = 1; i < lines.Length; i++)
                                    sb.AppendLine(indent + "\t" + lines[i]);
                            }
                            else sb.AppendLine(value);
                        }
                    }
                    break;

                case JsonValueKind.Array:
                    int index = 0;
                    foreach (var item in element.EnumerateArray())
                    {
                        sb.AppendLine(indent + $"- Item {index}:");
                        sb.Append(FormatElement(item, indentLevel + 1));
                        index++;
                    }
                    break;

                default:
                    sb.AppendLine(indent + element.ToString());
                    break;
            }

            return sb.ToString();
        }
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;
using ToSic.Cre8magic.Oqtane.Transformer.Models;

[JsonSerializable(typeof(TemplateJson))]
[JsonSourceGenerationOptions(WriteIndented = true)]
public partial class TemplateJsonContext : JsonSerializerContext
{
}
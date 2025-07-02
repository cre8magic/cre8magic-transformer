using System.Text.Json.Serialization;

// ReSharper disable CheckNamespace
namespace ToSic.Cre8magic.Oqtane.TemplateGenerator.Models;

/// <summary>
/// Represents the root of the template-generator.config.json file.
/// </summary>
public class TemplateGeneratorConfig
{
    [JsonPropertyName("sourcePath")]
    public string? SourcePath { get; set; }

    [JsonPropertyName("destinationPath")]
    public string? DestinationPath { get; set; }

    [JsonPropertyName("source")]
    public List<TemplateGeneratorConfigRule> Source { get; set; } = new();

    [JsonPropertyName("rename")]
    public List<TemplateGeneratorConfigRule> Rename { get; set; } = new();

    [JsonPropertyName("process")]
    public List<TemplateGeneratorConfigRule> Process { get; set; } = new();
}
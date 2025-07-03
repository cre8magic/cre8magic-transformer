using System.Text.Json.Serialization;

// ReSharper disable CheckNamespace
namespace ToSic.Cre8magic.Oqtane.Transformer.Models;

/// <summary>
/// Represents the root of the cre8magic-transformer.config.json file.
/// </summary>
public class TransformerConfig
{
    [JsonPropertyName("sourcePath")]
    public string? SourcePath { get; set; }

    [JsonPropertyName("destinationPath")]
    public string? DestinationPath { get; set; }

    [JsonPropertyName("source")]
    public List<TransformerConfigRule> Source { get; set; } = new();

    [JsonPropertyName("rename")]
    public List<TransformerConfigRule> Rename { get; set; } = new();

    [JsonPropertyName("process")]
    public List<TransformerConfigRule> Process { get; set; } = new();
}
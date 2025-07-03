using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using ToSic.Cre8magic.Oqtane.TemplateGenerator.Models;

// ReSharper disable CheckNamespace
namespace ToSic.Cre8magic.Oqtane.TemplateGenerator;

public partial class Program
{
    private static TemplateGeneratorConfig GetConfiguration(string? sourcePath, string? destinationPath, string? configPath)
    {
        configPath = GetConfigPath(sourcePath, configPath);

        if (!File.Exists(configPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(
                $"\nError: Config file not found at '{configPath}'. Please specify with --config or place it in the application/source directory.");
            Console.ResetColor();
            Environment.Exit(1);
        }

        // Load config for possible default paths
        TemplateGeneratorConfig? config = null;
        try
        {
            var configJson = File.ReadAllText(configPath);
            config = JsonSerializer.Deserialize(configJson, TemplateGeneratorConfigJsonContext.Default.TemplateGeneratorConfig);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError: Failed to parse config file: {ex.Message}");
            Console.ResetColor();
            Environment.Exit(1);
        }

        if (config == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nError: Config file is empty or invalid.");
            Console.ResetColor();
            Environment.Exit(1);
        }

        // Validation
        config = PrepareSourceAndDestinationPaths(config, sourcePath, destinationPath);
        ConfigValidation(config);

        return config;
    }

    private static string? GetConfigPath(string? sourcePath, string? configPath)
        => ConvertToFullPath(string.IsNullOrEmpty(configPath)
            ? Path.Combine(sourcePath ?? string.Empty, Constants.TemplateGeneratorConfigJson)
            : configPath);

    private static TemplateGeneratorConfig PrepareSourceAndDestinationPaths(TemplateGeneratorConfig config,
        string? sourcePath,
        string? destinationPath)
    {
        // Use config values if CLI not provided
        config.SourcePath = ConvertToFullPath(sourcePath ?? config.SourcePath);
        config.DestinationPath = ConvertToFullPath(destinationPath ?? config.DestinationPath);

        return config;
    }

    private static void ConfigValidation(TemplateGeneratorConfig config)
    {
        if (string.IsNullOrEmpty(config.SourcePath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nError: Source directory path is required. Please specify with --source or -s.");
            Console.ResetColor();
            Environment.Exit(1);
        }

        if (!Directory.Exists(config.SourcePath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError: Source directory not found at '{config.SourcePath}'");
            Console.ResetColor();
            Environment.Exit(1);
        }

        if (!File.Exists(Path.Combine(config.SourcePath, Constants.TemplateJson)))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError: '{Constants.TemplateJson}' not found at '{config.SourcePath}'");
            Console.ResetColor();
            //Environment.Exit(1);
            // Create a default template.json if it doesn't exist
            CreateDefaultTemplateJson(config);
        }

        if (string.IsNullOrEmpty(config.DestinationPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError: Destination directory is invalid. Please specify with --destination or -d or in config file.");
            Console.ResetColor();
            Environment.Exit(1);
        }
    }

    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Serialize<TValue>(TValue, JsonSerializerOptions)")]
    [RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.Serialize<TValue>(TValue, JsonSerializerOptions)")]
    private static void CreateDefaultTemplateJson(TemplateGeneratorConfig config)
    {
        Console.WriteLine($"Creating a default '{Constants.TemplateJson}' file in '{config.SourcePath}'...");
        var defaultJson = JsonSerializer.Serialize(
            new TemplateJson(),
            TemplateJsonContext.Default.TemplateJson
        );
        File.WriteAllText(Path.Combine(config.SourcePath!, Constants.TemplateJson), defaultJson);
    }
}
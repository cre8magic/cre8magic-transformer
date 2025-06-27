using System.Text.Json;
using ToSic.Cre8magic.Oqtane.TemplateGenerator.Models;

// ReSharper disable CheckNamespace
namespace ToSic.Cre8magic.Oqtane.TemplateGenerator;

public partial class Program
{
    /// <summary>
    /// Resolves the configuration file path based on the provided input or default locations.
    /// </summary>
    /// <remarks>If a non-empty <paramref name="configPath"/> is provided, it is converted to an absolute
    /// path. Otherwise, the method searches for the configuration file in the current working directory first, and then
    /// in the directory of the running executable.</remarks>
    /// <param name="configPath">An optional path to the configuration file. If <see langword="null"/> or empty, the method attempts to locate
    /// the file in the current working directory, falling back to the executable's base directory if not found.</param>
    /// <returns>The resolved full path to the configuration file, or <see langword="null"/> if no valid path could be
    /// determined.</returns>
    private static string? GetConfigPath(string? configPath)
    {
        if (string.IsNullOrEmpty(configPath))
        {
            // 1. Try current working directory
            configPath = Path.Combine(Directory.GetCurrentDirectory(), TemplateGeneratorConfigJson);
            if (!File.Exists(configPath))
            {
                // 2. Fallback to executable directory
                configPath = Path.Combine(AppContext.BaseDirectory, TemplateGeneratorConfigJson);
            }
        }
        else
        {
            configPath = ConvertToFullPath(configPath);
        }

        return configPath;
    }

    /// <summary>
    /// Attempts to load the configuration from the specified file path.
    /// </summary>
    /// <remarks>If the specified file does not exist or cannot be parsed, an error message is written to the
    /// console, and the method returns <see langword="null"/> for the configuration object and <see langword="true"/>
    /// for the error flag.</remarks>
    /// <param name="configPath">The path to the configuration file. Can be <see langword="null"/>.</param>
    /// <returns>A tuple containing the loaded <see cref="TemplateGeneratorConfig"/> object and a <see langword="bool"/>
    /// indicating whether an error occurred. The first item in the tuple is the configuration object, or <see
    /// langword="null"/> if the file could not be loaded or parsed. The second item is <see langword="true"/> if an
    /// error occurred; otherwise, <see langword="false"/>.</returns>
    private static (TemplateGeneratorConfig?, bool Error) GetConfiguration(string? configPath)
    {
        if (!File.Exists(configPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(
                $"\nError: Config file not found at '{configPath}'. Please specify with --config or place it in the application/source directory.");
            Console.ResetColor();
            return (null, true);
        }

        // Load config for possible default paths
        TemplateGeneratorConfig? config = null;
        try
        {
            var configJson = File.ReadAllText(configPath);
            config = JsonSerializer.Deserialize(configJson,
                TemplateGeneratorConfigJsonContext.Default.TemplateGeneratorConfig);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError: Failed to parse config file: {ex.Message}");
            Console.ResetColor();
            return (null, true);
        }

        return (config, false);
    }
}
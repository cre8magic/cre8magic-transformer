using System.Text.Json;
using System.Reflection;
using ToSic.Cre8magic.Oqtane.TemplateGenerator.Models;

// ReSharper disable CheckNamespace
namespace ToSic.Cre8magic.Oqtane.TemplateGenerator;

/// <summary>
/// Main application class.
/// </summary>
public partial class Program
{
    private const string TemplateGeneratorConfigJson = "template-generator.config.json";
    private const string TemplateJson = "template.json";

    /// <summary>
    /// Application entry point. Parses arguments and starts the conversion.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    public static void Main(string[] args)
    {
        var name = Assembly.GetExecutingAssembly().GetName().Name;
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "?";
        Console.WriteLine("================================");
        Console.WriteLine($" {name} v{version}");
        Console.WriteLine("================================");

        // --- Argument Parsing ---
        var sourcePath = GetArgument(args, "--source", "-s");
        var destinationPath = GetArgument(args, "--destination", "-d");
        var configPath = GetArgument(args, "--config", "-c");

        // Default config path if not provided
        configPath = GetConfigPath(configPath);

        var (config, configError) = GetConfiguration(configPath);
        if (configError)
            return;

        (sourcePath, destinationPath) = PrepareSourceAndDestinationPaths(sourcePath, config, configPath, destinationPath);

        // Input Validation
        if (!InputIsValid(sourcePath, destinationPath))
            return;

        // Ensure destinationPath is clean before starting conversion
        CleanDestinationFolder(destinationPath);

        // --- Conversion Process ---
        try
        {
            var converter = new ThemeConverter(sourcePath, destinationPath, configPath);
            converter.Process();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nConversion completed successfully!");
            Console.ResetColor();
            Console.WriteLine($"Template generated at: {destinationPath}");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nAn unexpected error occurred: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Console.ResetColor();
        }

        //Console.WriteLine("\nPress any key to exit...");
        //Console.ReadKey();
    }
}
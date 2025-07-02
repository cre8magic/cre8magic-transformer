// ReSharper disable CheckNamespace
namespace ToSic.Cre8magic.Oqtane.TemplateGenerator;

/// <summary>
/// Main application class.
/// </summary>
public partial class Program
{
    /// <summary>
    /// Application entry point. Parses arguments and starts the conversion.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    public static void Main(string[] args)
    {
        PrintAppInfo();

        // Arguments
        var (sourcePath, destinationPath, configPath) = ArgumentParsing(args);

        // Configuration
        var config = GetConfiguration(sourcePath, destinationPath, configPath);

        // --- Conversion Process ---
        try
        {
            var converter = new ThemeConverter(config);
            converter.Process();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nConversion completed successfully!");
            Console.ResetColor();
            Console.WriteLine($"Template generated at: {config.DestinationPath}");
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
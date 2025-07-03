// ReSharper disable CheckNamespace
using System.Reflection;

namespace ToSic.Cre8magic.Oqtane.Transformer;

public partial class Program
{
    private static (string? sourcePath, string? destinationPath, string? configPath) ArgumentParsing(string[]? args)
    {
        // remove empty strings from args
        args = args?.Where(arg => !string.IsNullOrEmpty(arg)).ToArray();
        
        if (args == null || args.Length == 0 || 
            HasArgument(args, "--help", "-h"))
        {
            PrintHelp();
            Environment.Exit(0);
        }

        // --- Argument Parsing ---
        var sourcePath = GetArgument(args, "--source", "-s");
        var destinationPath = GetArgument(args, "--destination", "-d");
        var configPath = GetArgument(args, "--config", "-c");
        
        ArgumentsValidation(sourcePath, configPath);

        return (sourcePath, destinationPath, configPath);
    }

    private static bool HasArgument(string[] args, string optionName, string shortOptionName)
        => args.Any(arg
            => arg.Equals(optionName, StringComparison.OrdinalIgnoreCase)
               || arg.Equals(shortOptionName, StringComparison.OrdinalIgnoreCase));

    private static string? GetArgument(string[] args, string optionName, string shortOptionName)
    {
        for (var i = 0; i < args.Length - 1; i++)
        {
            if (args[i].Equals(optionName, StringComparison.OrdinalIgnoreCase) ||
                args[i].Equals(shortOptionName, StringComparison.OrdinalIgnoreCase))
            {
                return args[i + 1];
            }
        }

        return null;
    }

    private static void ArgumentsValidation(string? sourcePath, string? configPath)
    {
        if (!string.IsNullOrEmpty(sourcePath) || !string.IsNullOrEmpty(configPath)) return;

        PrintHelp();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\nError: Either source or config paths must be provided via CLI.");
        Console.ResetColor();
        Environment.Exit(1);
    }

    private static void PrintAppInfo()
    {
        var name = Assembly.GetExecutingAssembly().GetName().Name;
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "?";
        Console.WriteLine("================================");
        Console.WriteLine($" {name} v{version}");
        Console.WriteLine("================================");
        Console.WriteLine("\nDocumentation https://go.cre8magic.org/tf");
    }

    /// <summary>
    /// Prints usage instructions to the console.
    /// </summary>
    private static void PrintHelp()
    {
        Console.WriteLine("\nConverts an Oqtane theme into a reusable template.");
        Console.WriteLine("\nUSAGE:");
        Console.WriteLine("  cre8magicTransformer --source <path> [--destination <path>] [--config <path>]");
        Console.WriteLine("\nARGUMENTS:");
        Console.WriteLine("  -s, --source <path>       Path to the source Oqtane theme directory.");
        Console.WriteLine("  -d, --destination <path>  (Optional) Path to the output directory for the template.");
        Console.WriteLine("  -c, --config <path>       (Optional) Path to the JSON configuration file.");
        Console.WriteLine("                            Defaults to 'cre8magic-transformer.config.json' in source directory.");
        Console.WriteLine("  -h, --help                Displays this help information.");
        //Console.WriteLine("  -v, --verbose             Verbose logging.");
    }
}
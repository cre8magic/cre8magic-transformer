// ReSharper disable CheckNamespace
namespace ToSic.Cre8magic.Oqtane.TemplateGenerator;

public partial class Program
{
    /// <summary>
    /// Helper to extract an argument's value from the command line args array.
    /// </summary>
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

    /// <summary>
    /// Prints usage instructions to the console.
    /// </summary>
    private static void PrintUsage()
    {
        Console.WriteLine("\nConverts an Oqtane theme into a reusable template.");
        Console.WriteLine("\nUSAGE:");
        Console.WriteLine("  OqtaneTemplateGenerator --source <path> --destination <path> [--config <path>]");
        Console.WriteLine("\nARGUMENTS:");
        Console.WriteLine("  -s, --source <path>       (Optional) Path to the source Oqtane theme directory.");
        Console.WriteLine("  -d, --destination <path>  (Optional) Path to the output directory for the template.");
        Console.WriteLine("  -c, --config <path>       (Optional) Path to the JSON configuration file.");
        Console.WriteLine(
            "                            Defaults to 'template-generator.config.json' in execution directory.");
    }
}
using ToSic.Cre8magic.Oqtane.TemplateGenerator.Models;

// ReSharper disable CheckNamespace
namespace ToSic.Cre8magic.Oqtane.TemplateGenerator;

public partial class Program
{
    /// <summary>
    /// Prepares and resolves the source and destination paths based on the provided inputs and configuration.
    /// </summary>
    /// <remarks>This method ensures that both the source and destination paths are fully resolved and
    /// absolute. If the source  path is not provided, it defaults to the value in the configuration or the directory
    /// containing the configuration  file. If the destination path is not provided, it defaults to the value in the
    /// configuration. If a template folder  is specified in the configuration, it is appended to the destination
    /// path.</remarks>
    /// <param name="sourcePath">The source path provided via the command-line interface. If null or empty, the method attempts to resolve it 
    /// using the <paramref name="config"/> or the directory containing <paramref name="configPath"/>.</param>
    /// <param name="config">The configuration object containing default values for the source and destination paths, as well as other 
    /// optional settings such as the template folder.</param>
    /// <param name="configPath">The file path to the configuration file. Used as a fallback to determine the source path if  <paramref
    /// name="sourcePath"/> is not provided.</param>
    /// <param name="destinationPath">The destination path provided via the command-line interface. If null or empty, the method attempts to resolve
    /// it  using the <paramref name="config"/>.</param>
    /// <returns>A tuple containing the resolved source path and destination path. The source path is the fully qualified path 
    /// to the source directory, and the destination path is the fully qualified path to the destination directory, 
    /// optionally appended with the template folder if specified in the <paramref name="config"/>.</returns>
    private static (string? sourcePath, string? destinationPath) PrepareSourceAndDestinationPaths(string? sourcePath,
        TemplateGeneratorConfig? config, string? configPath, string? destinationPath)
    {
        // Use config values if CLI not provided
        if (string.IsNullOrEmpty(sourcePath))
            sourcePath = ConvertToFullPath(config?.SourcePath)
                         ?? Directory.GetParent(configPath)?.FullName; // fallback to folder with config
        else
            sourcePath = ConvertToFullPath(sourcePath);


        destinationPath =
            ConvertToFullPath(string.IsNullOrEmpty(destinationPath) ? config?.DestinationPath : destinationPath);

        // Append template folder to destinationPath if present in config
        if (!string.IsNullOrWhiteSpace(config?.Template))
            destinationPath = Path.Combine(destinationPath ?? string.Empty, config.Template);
        return (sourcePath, destinationPath);
    }

    /// <summary>
    /// Validates the input paths for the source and destination directories.
    /// </summary>
    /// <remarks>This method checks whether the source and destination paths are provided and valid.  It also
    /// verifies that the source directory exists and contains the required template file. If validation fails, error
    /// messages are displayed to the console, and the method returns <see langword="true"/>.</remarks>
    /// <param name="sourcePath">The path to the source directory. This parameter cannot be null or empty.</param>
    /// <param name="destinationPath">The path to the destination directory. This parameter cannot be null or empty.</param>
    /// <returns><see langword="true"/> if the validation fails due to missing or invalid paths;  otherwise, <see
    /// langword="false"/> if the input paths are valid.</returns>
    private static bool InputIsValid(string? sourcePath, string? destinationPath)
    {
        if (string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(destinationPath))
        {
            PrintUsage();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nError: Source and destination paths must be provided via CLI or config file.");
            Console.ResetColor();
            return false;
        }

        if (!Directory.Exists(sourcePath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError: Source directory not found at '{sourcePath}'");
            Console.ResetColor();
            return false;
        }

        if (!File.Exists(Path.Combine(sourcePath, TemplateJson)))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError: '{TemplateJson}' not found at '{sourcePath}'");
            Console.ResetColor();
            return false;
        }

        return true;
    }

    /// <summary>
    /// Converts the specified relative or absolute path to its full path representation.
    /// </summary>
    /// <remarks>This method uses <see cref="Path.GetFullPath(string)"/> to resolve the full path.  Ensure
    /// that the input path is valid and accessible to avoid exceptions.</remarks>
    /// <param name="path">The path to convert. Can be a relative or absolute path. If <see langword="null"/> or empty, an empty string is
    /// returned.</param>
    /// <returns>The full path representation of the specified <paramref name="path"/>, or <see langword="null"/> if <paramref
    /// name="path"/> is <see langword="null"/> or empty.</returns>
    private static string? ConvertToFullPath(string? path)
        => string.IsNullOrEmpty(path) ? null : Path.GetFullPath(path);

    /// <summary>
    /// Deletes the specified destination folder and all its contents, if it exists.
    /// </summary>
    /// <remarks>If the folder exists, it will be deleted along with all its contents. If an error occurs 
    /// during deletion, an error message will be written to the console, and the operation will not retry.</remarks>
    /// <param name="destinationPath">The path to the destination folder to be cleaned. Can be <see langword="null"/> or empty,  in which case no
    /// action is taken.</param>
    private static void CleanDestinationFolder(string? destinationPath)
    {
        if (Directory.Exists(destinationPath))
        {
            try
            {
                Directory.Delete(destinationPath, true);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError: Could not clean destination directory: {ex.Message}");
                Console.ResetColor();
                //return;
            }
        }
    }
}
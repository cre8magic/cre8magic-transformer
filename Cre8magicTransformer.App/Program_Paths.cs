// ReSharper disable CheckNamespace
namespace ToSic.Cre8magic.Oqtane.Transformer;

public partial class Program
{
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
        if (Directory.Exists(destinationPath) && File.Exists(Path.Combine(destinationPath, Constants.TemplateJson)))
        {
            try
            {
                Directory.Delete(destinationPath, true);
                Console.WriteLine($"Destination directory '{destinationPath}' deleted.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError: Could not clean destination directory: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using System.Text;
using ToSic.Cre8magic.Oqtane.Transformer.Models;

// ReSharper disable CheckNamespace
namespace ToSic.Cre8magic.Oqtane.Transformer;

/// <summary>
/// Handles the theme conversion logic.
/// </summary>
public class ThemeConverter
{
    private readonly TransformerConfig _config;

    public ThemeConverter(TransformerConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));

        Console.WriteLine("Configuration loaded successfully.");
        Console.WriteLine($" - Source: {_config.SourcePath}");
        Console.WriteLine($" - Destination: {_config.DestinationPath}");
    }

    public void Process()
    {
        CleanDestinationDirectory();
        Directory.CreateDirectory(_config.DestinationPath!);

        Console.WriteLine("\nStarting conversion...");

        // Step 1: Source selection and copy
        ProcessSource(_config.SourcePath!, _config.DestinationPath!);
    }

    private void CleanDestinationDirectory()
    {
        if (Directory.Exists(_config.DestinationPath) && File.Exists(Path.Combine(_config.DestinationPath, Constants.TemplateJson)))
        {
            Console.WriteLine("Destination directory exists. Cleaning it up...");
            try
            {
                Directory.Delete(_config.DestinationPath, true);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError: Could not clean '{_config.DestinationPath}' destination directory: {ex.Message}");
                Console.ResetColor();
            }
        }
    }

    private void ProcessSource(string sourceDir, string destDir)
    {
        var dirInfo = new DirectoryInfoWrapper(new DirectoryInfo(sourceDir));
        // Collect all files to include using globbing
        var includedFiles = new HashSet<string>();
        foreach (var rule in _config.Source)
        {
            var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
            if (rule.Include != null)
                matcher.AddIncludePatterns(rule.Include);
            if (rule.Exclude != null)
                matcher.AddExcludePatterns(rule.Exclude);
            var result = matcher.Execute(dirInfo);
            foreach (var file in result.Files)
                includedFiles.Add(Path.Combine(sourceDir, file.Path.Replace("/", @"\")));
        }
        // Process files and create directories as needed
        foreach (var file in includedFiles)
        {
            var relFile = Path.GetRelativePath(_config.SourcePath!, file);
            var destFile = Path.Combine(destDir, ApplyRenameRules(relFile));

            // Ensure the directory structure exists for the renamed file
            var destFileDir = Path.GetDirectoryName(destFile);
            if (destFileDir != null)
                Directory.CreateDirectory(destFileDir);
                
            ProcessFile(file, destFile, relFile);
        }
    }

    private void ProcessFile(string sourceFile, string destFile, string relFile)
    {
        if (ShouldProcessFile(relFile))
        {
            var content = File.ReadAllText(sourceFile);
            var processedContent = ApplyProcessRules(content, relFile);
            if (content != processedContent)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"[CHANGED]      {Path.GetRelativePath(_config.SourcePath!, sourceFile)}");
                LogRename(sourceFile, destFile);
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[UNCHANGED]    {Path.GetRelativePath(_config.SourcePath!, sourceFile)}");
                LogRename(sourceFile, destFile);
                Console.ResetColor();
            }
            File.WriteAllText(destFile, processedContent, Encoding.UTF8);
        }
        else
        {
            File.Copy(sourceFile, destFile, true);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"[COPY]         {Path.GetRelativePath(_config.SourcePath!, sourceFile)}");
            LogRename(sourceFile, destFile);
            Console.ResetColor();
        }
    }

    private void LogRename(string sourceFile, string destFile)
    {
        var relativeSourcePath = Path.GetRelativePath(_config.SourcePath!, sourceFile);
        var relativeDestPath = Path.GetRelativePath(_config.DestinationPath!, destFile);
        if (!relativeSourcePath.Equals(relativeDestPath, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"            -> {relativeDestPath}");
        }
    }

    // Returns true if any process rule includes and does not exclude the file
    private bool ShouldProcessFile(string relFile)
        => _config.Process.Any(rule => GlobMatches(relFile, rule.Include, rule.Exclude));

    private string ApplyRenameRules(string relPath)
    {
        foreach (var rule in _config.Rename)
        {
            if (rule.Replace == null) continue;
            if (GlobMatches(relPath, rule.Include, rule.Exclude))
                relPath = ApplyReplacements(relPath, rule.Replace);
        }
        return relPath;
    }

    private string ApplyProcessRules(string content, string relPath)
    {
        foreach (var rule in _config.Process)
        {
            if (rule.Replace == null) continue;
            if (GlobMatches(relPath, rule.Include, rule.Exclude))
                content = ApplyReplacements(content, rule.Replace);
        }
        return content;
    }

    private static bool GlobMatches(string relPath, List<string>? includes, List<string>? excludes)
    {
        var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
        if (includes != null) matcher.AddIncludePatterns(includes);
        if (excludes != null) matcher.AddExcludePatterns(excludes);
        return matcher.Match(relPath).HasMatches;
    }

    private static string ApplyReplacements(string input, Dictionary<string, string> replacements)
        => replacements.Aggregate(input, (current, pair) => current.Replace(pair.Key, pair.Value, StringComparison.CurrentCulture));
}
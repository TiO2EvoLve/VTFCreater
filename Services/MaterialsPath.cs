using System;
using System.IO;

namespace VTFCreater.Services;

/// <summary>
/// Converts a disk output directory to a Source material path. VMT texture
/// references are always relative to the containing <c>materials</c> folder.
/// </summary>
public static class MaterialsPath
{
    public static string GetRelativeDirectory(string outputDirectory)
    {
        if (string.IsNullOrWhiteSpace(outputDirectory))
            throw new ArgumentException("Output directory is required.", nameof(outputDirectory));

        var fullOutputDirectory = Path.GetFullPath(outputDirectory);
        var current = new DirectoryInfo(fullOutputDirectory);

        while (current is not null)
        {
            if (string.Equals(current.Name, "materials", StringComparison.OrdinalIgnoreCase))
            {
                var relativeDirectory = Path.GetRelativePath(current.FullName, fullOutputDirectory);
                return relativeDirectory == "."
                    ? string.Empty
                    : relativeDirectory.Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Replace('\\', '/');
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("输出目录必须是名为 materials 的目录，或其子目录。");
    }
}

using System;
using System.IO;
using System.Linq;
using VTFCreater.Models;

namespace VTFCreater.Services;

public static class TextureClassifier
{
    public static TextureFile? Classify(string fullPath, string sourceDirectory, AppConfig config)
    {
        var fileName = Path.GetFileNameWithoutExtension(fullPath);
        var relativePath = Path.GetRelativePath(sourceDirectory, fullPath);
        var relativeDirectory = Path.GetDirectoryName(relativePath) ?? string.Empty;

        var normalSuffix = FindMatchingSuffix(fileName, config.NormalSuffixes);
        if (normalSuffix is not null)
        {
            return new TextureFile
            {
                FullPath = fullPath,
                RelativePath = relativePath,
                FileName = Path.GetFileName(fullPath),
                MaterialName = fileName[..^normalSuffix.Length],
                Type = TextureType.Normal
            };
        }

        var baseColorSuffix = FindMatchingSuffix(fileName, config.BaseColorSuffixes);
        if (baseColorSuffix is not null)
        {
            return new TextureFile
            {
                FullPath = fullPath,
                RelativePath = relativePath,
                FileName = Path.GetFileName(fullPath),
                MaterialName = fileName[..^baseColorSuffix.Length],
                Type = TextureType.BaseColor
            };
        }

        return null;
    }

    private static string? FindMatchingSuffix(string fileName, System.Collections.Generic.IEnumerable<string> suffixes)
    {
        foreach (var suffix in suffixes.OrderByDescending(s => s.Length))
        {
            if (suffix == string.Empty)
            {
                continue;
            }

            if (fileName.Contains(suffix, StringComparison.OrdinalIgnoreCase))
            {
                return suffix;
            }
        }

        if (suffixes.Contains(string.Empty))
        {
            return string.Empty;
        }

        return null;
    }
}

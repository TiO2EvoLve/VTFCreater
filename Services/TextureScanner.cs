using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VTFCreater.Models;

namespace VTFCreater.Services;

public static class TextureScanner
{
    public static IReadOnlyList<string> ScanPngFiles(string sourceDirectory)
    {
        if (string.IsNullOrWhiteSpace(sourceDirectory) || !Directory.Exists(sourceDirectory))
            return [];

        return Directory
            .EnumerateFiles(sourceDirectory, "*.png", SearchOption.AllDirectories)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}

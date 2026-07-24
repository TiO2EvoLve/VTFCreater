using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VTFCreater.Services;

//检索文件夹内的png图片
public static class TextureScanner
{
    public static IReadOnlyList<string> ScanImageFiles(string sourceDirectory)
    {
        if (string.IsNullOrWhiteSpace(sourceDirectory) || !Directory.Exists(sourceDirectory))
            return [];

        // 支持的图片扩展名（不含*）
        HashSet<string> imageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".png", ".jpg", ".jpeg", ".tga","bmp"
        };

        return Directory
            .EnumerateFiles(sourceDirectory, "*.*", SearchOption.AllDirectories)
            .Where(file => imageExtensions.Contains(Path.GetExtension(file)))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}

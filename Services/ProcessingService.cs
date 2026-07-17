using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VTFCreater.Models;

namespace VTFCreater.Services;

public class ProcessingService
{
    private readonly VtfGenerator _vtfGenerator = new();
    private readonly VmtGenerator _vmtGenerator = new();

    public async Task ProcessAsync(AppConfig config, LogService logService, CancellationToken cancellationToken = default)
    {
        logService.Clear();

        if (string.IsNullOrWhiteSpace(config.SourceDirectory) || !Directory.Exists(config.SourceDirectory))
        {
            logService.Error("源贴图目录无效，请先在设置中配置。");
            return;
        }

        if (string.IsNullOrWhiteSpace(config.OutputDirectory))
        {
            logService.Error("输出目录无效，请先在设置中配置。");
            return;
        }

        logService.Info("开始扫描 PNG 文件…");

        var pngFiles = TextureScanner.ScanPngFiles(config.SourceDirectory);
        logService.Info($"扫描目录完成，共发现 {pngFiles.Count} 个 PNG 文件。");

        var textures = pngFiles
            .Select(path => TextureClassifier.Classify(path, config.SourceDirectory, config))
            .Where(texture => texture is not null)
            .Cast<TextureFile>()
            .ToList();

        var skippedCount = pngFiles.Count - textures.Count;
        if (skippedCount > 0)
        {
            logService.Warn($"有 {skippedCount} 个文件未能识别贴图类型，已跳过。");
        }

        var materials = MaterialMatcher.BuildMaterials(textures);
        logService.Info($"识别出 {materials.Count} 个材质。");

        Directory.CreateDirectory(config.OutputDirectory);

        foreach (var material in materials)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (material.BaseColor is not null)
            {
                logService.Info($"发现材质：{FormatMaterialLabel(material)}");
            }

            if (material.BaseColor is not null)
            {
                var outputPath = BuildOutputPath(config.OutputDirectory, material.BaseColor);
                _vtfGenerator.Generate(config.VTFCmdPath, material.BaseColor.FullPath, outputPath, config.Format);
                logService.Info($"生成：{Path.GetFileName(outputPath)}");
            }

            if (material.Normal is not null)
            {
                var outputPath = BuildOutputPath(config.OutputDirectory, material.Normal);
                _vtfGenerator.Generate(config.VTFCmdPath, material.Normal.FullPath, outputPath, config.Format);
                logService.Info($"生成：{Path.GetFileName(outputPath)}");
            }

            if (material.ShouldGenerateVmt)
            {
                await _vmtGenerator.GenerateAsync(material, config.OutputDirectory);
                logService.Info($"生成：{material.Name}.vmt");
            }
        }

        logService.Info("全部处理完成。");
    }

    private static string BuildOutputPath(string outputDirectory, TextureFile texture)
    {
        var relativeDirectory = texture.RelativePath.Contains(Path.DirectorySeparatorChar)
            ? Path.GetDirectoryName(texture.RelativePath)
            : string.Empty;

        var fileName = Path.GetFileNameWithoutExtension(texture.FileName) + ".vtf";
        return string.IsNullOrEmpty(relativeDirectory)
            ? Path.Combine(outputDirectory, fileName)
            : Path.Combine(outputDirectory, relativeDirectory, fileName);
    }

    private static string FormatMaterialLabel(MaterialInfo material)
    {
        return string.IsNullOrEmpty(material.RelativeDirectory)
            ? material.Name
            : $"{material.RelativeDirectory}/{material.Name}";
    }
}

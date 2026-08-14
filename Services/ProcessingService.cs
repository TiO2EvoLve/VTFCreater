using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageMagick;
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

        if (string.IsNullOrWhiteSpace(config.OutputDirectory) || !Directory.Exists(config.OutputDirectory))
        {
            logService.Error("输出目录无效，请先在设置中配置。");
            return;
        }

        logService.Info("开始扫描 PNG 文件…");

        var pngFiles = TextureScanner.ScanImageFiles(config.SourceDirectory);
        
        foreach (var file in pngFiles)
        {
            var info = new MagickImageInfo(file);

            //检查是否符合n的2次幂
            if (!IsPowerOfTwo(info.Width) ||
                !IsPowerOfTwo(info.Height))
            {
                throw new Exception(
                    $"以下贴图尺寸不符合要求: {file} ({info.Width}x{info.Height})");
            }
        }
        
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
        

        foreach (var material in materials)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Console.WriteLine("漫反射贴图"+material.BaseColor);
            Console.WriteLine("法线"+material.Normal);
            if (material.BaseColor is not null)
            {
                logService.Info($"发现材质：{FormatMaterialLabel(material)}");
            }
            
            if (material.BaseColor is not null)
            {
                var outputPath = BuildOutputPath(config.OutputDirectory, material.BaseColor);
                Console.WriteLine("输出目录："+outputPath);
                await _vtfGenerator.Generate(config.VTFCmdPath, material.BaseColor.FullPath, outputPath, config.Format,config.SizeClamp);
                logService.Info($"生成：{Path.GetFileName(outputPath)}");
            }
            if (material.Normal is not null)
            {
                var outputPath = BuildOutputPath(config.OutputDirectory, material.Normal);
                await _vtfGenerator.Generate(config.VTFCmdPath, material.Normal.FullPath, outputPath, config.Format,config.SizeClamp);
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
    //构建输出路径
    private static string BuildOutputPath(string outputDirectory, TextureFile texture)
    {
        var relativeDirectory = texture.RelativePath.Contains(Path.DirectorySeparatorChar)
            ? Path.GetDirectoryName(texture.RelativePath)
            : string.Empty;
        
        return string.IsNullOrEmpty(relativeDirectory)
            ? Path.Combine(outputDirectory)
            : Path.Combine(outputDirectory, relativeDirectory);
    }
    
    private static string FormatMaterialLabel(MaterialInfo material)
    {
        return string.IsNullOrEmpty(material.RelativeDirectory)
            ? material.Name
            : $"{material.RelativeDirectory}/{material.Name}";
    }
    
    //检查分辨率是否为n的2次幂
    public static bool IsPowerOfTwoTexture(string filePath)
    {
        var info = new MagickImageInfo(filePath);

        return IsPowerOfTwo(info.Width)
               && IsPowerOfTwo(info.Height);
    }
    //配合上述方法
    private static bool IsPowerOfTwo(uint value)
    {
        return value > 0 && (value & (value - 1)) == 0;
    }
}

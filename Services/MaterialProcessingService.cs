using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VTFCreater.Enum;
using VTFCreater.Models;

namespace VTFCreater.Services;

public sealed class MaterialProcessingService
{
    private readonly VtfGenerator _vtfGenerator = new();
    private readonly VmtGenerator _vmtGenerator = new();

    public async Task GenerateAsync(string materialName, string outputDirectory, MaterialShader shader,
        IReadOnlyCollection<MaterialSlot> slots, Formats format, SizeClamp sizeClamp, string vtfCmdPath,
        LogService log)
    {
        var baseColor = Find(slots, "BaseColor");
        if (baseColor?.FilePath is null)
            throw new InvalidOperationException("Base Color 是必填插槽，请先添加图片。");

        var materialRelativeDirectory = MaterialsPath.GetRelativeDirectory(outputDirectory);
        Directory.CreateDirectory(outputDirectory);
        var converted = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var temporaryDirectory = Path.Combine(Path.GetTempPath(), "VTFCreater", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(temporaryDirectory);

        try
        {
            foreach (var slot in slots)
            {
                if (slot.FilePath is null) continue;
                var suffix = slot.Key switch
                {
                    "BaseColor" => string.Empty, "Normal" => "_normal", "Alpha" => "_alpha",
                    "Emissive" => "_emissive", _ => "_" + slot.Key.ToLowerInvariant()
                };
                var stagedImage = Path.Combine(temporaryDirectory, materialName + suffix + Path.GetExtension(slot.FilePath));
                File.Copy(slot.FilePath, stagedImage, true);
                await _vtfGenerator.Generate(vtfCmdPath, stagedImage, temporaryDirectory, format, sizeClamp);
                var stagedVtf = Path.ChangeExtension(stagedImage, ".vtf");
                var targetName = materialName + suffix + ".vtf";
                File.Move(stagedVtf, Path.Combine(outputDirectory, targetName), true);
                var textureName = Path.ChangeExtension(targetName, null)!;
                converted[slot.Key] = string.IsNullOrEmpty(materialRelativeDirectory)
                    ? textureName
                    : $"{materialRelativeDirectory}/{textureName}";
                log.Info($"已生成：{targetName}");
            }
            await _vmtGenerator.GenerateMaterialAsync(materialName, outputDirectory, shader, converted);
            log.Info($"已生成：{materialName}.vmt");
        }
        finally
        {
            if (Directory.Exists(temporaryDirectory)) Directory.Delete(temporaryDirectory, true);
        }
    }

    private static MaterialSlot? Find(IEnumerable<MaterialSlot> slots, string key)
    {
        foreach (var slot in slots) if (slot.Key == key) return slot;
        return null;
    }
}

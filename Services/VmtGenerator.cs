using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Scriban;
using VTFCreater.Models;
using VTFCreater.ViewModels;

namespace VTFCreater.Services;

//VMT文件生成器
public class VmtGenerator
{
    
    private readonly string _templatePath = Path.Combine(AppContext.BaseDirectory, "template", "simple.txt");

    public async Task GenerateAsync(MaterialInfo material, string outputDirectory)
    {
        if (material.BaseColor is null)
        {
            return;
        }
        var outputFolder = Path.Combine(outputDirectory, material.RelativeDirectory);
        //读取模板内容
        var templateContent = await File.ReadAllTextAsync(_templatePath);
        //解析模板内容
        var template = Template.Parse(templateContent);
        //vmt输出路径
        var vmtPath = Path.Combine(outputFolder, $"{material.Name}.vmt");
        var baseColorVtfPath = BuildVtfReference(material.RelativeDirectory, material.BaseColor);
        var normalVtfPath = material.Normal is null
            ? null
            : BuildVtfReference(material.RelativeDirectory, material.Normal);

        HomeViewModel vm = IoC.Resolve<HomeViewModel>();
        Console.WriteLine(vm.SelectedShaderTypes);
        var result = await template.RenderAsync(new
        {
            type = vm.SelectedShaderTypes,
            color = baseColorVtfPath,
            normal = normalVtfPath
        });

        await File.WriteAllTextAsync(vmtPath, result);
    }

    private static string BuildVtfReference(string relativeDirectory, TextureFile texture)
    {
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(texture.FileName);
        return string.IsNullOrEmpty(relativeDirectory)
            ? fileNameWithoutExtension
            : $"{relativeDirectory.Replace('\\', '/')}/{fileNameWithoutExtension}";
    }

    public async Task GenerateMaterialAsync(string materialName, string outputDirectory, MaterialShader shader,
        IReadOnlyDictionary<string, string> textures)
    {
        if (!textures.TryGetValue("BaseColor", out var baseColor))
            throw new InvalidOperationException("Base Color is required.");

        var lines = new List<string> { $"\"{shader}\"", "{", $"    \"$basetexture\" \"{baseColor}\"" };
        if (textures.TryGetValue("Normal", out var normal))
            lines.Add($"    \"$bumpmap\" \"{normal}\"");
        if (textures.TryGetValue("Alpha", out var alpha))
        {
            lines.Add("    \"$translucent\" \"1\"");
            lines.Add($"    \"$alphamask\" \"{alpha}\"");
        }
        if (textures.TryGetValue("Emissive", out var emissive))
            lines.Add($"    \"$selfillummask\" \"{emissive}\"");
        lines.Add("}");
        await File.WriteAllLinesAsync(Path.Combine(outputDirectory, materialName + ".vmt"), lines);
    }
}

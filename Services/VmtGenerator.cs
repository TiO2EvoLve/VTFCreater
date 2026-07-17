using System;
using System.IO;
using System.Threading.Tasks;
using Scriban;
using VTFCreater.Models;

namespace VTFCreater.Services;

public class VmtGenerator
{
    private readonly string _templatePath;

    public VmtGenerator()
    {
        _templatePath = Path.Combine(AppContext.BaseDirectory, "template", "simple.txt");
    }

    public async Task GenerateAsync(MaterialInfo material, string outputDirectory)
    {
        if (material.BaseColor is null)
        {
            return;
        }

        var outputFolder = Path.Combine(outputDirectory, material.RelativeDirectory);
        Directory.CreateDirectory(outputFolder);

        var templateContent = await File.ReadAllTextAsync(_templatePath);
        var template = Template.Parse(templateContent);

        var vmtPath = Path.Combine(outputFolder, $"{material.Name}.vmt");
        var baseColorVtfPath = BuildVtfReference(material.RelativeDirectory, material.BaseColor);
        var normalVtfPath = material.Normal is null
            ? null
            : BuildVtfReference(material.RelativeDirectory, material.Normal);

        var result = template.Render(new
        {
            VTFBaseColorPath = baseColorVtfPath,
            VTFNormalPath = normalVtfPath
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
}

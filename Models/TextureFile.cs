using System.IO;

namespace VTFCreater.Models;

//文件属性
public class TextureFile
{
    public required string FullPath { get; init; }

    public required string RelativePath { get; init; }

    public string RelativeDirectory => Path.GetDirectoryName(RelativePath) ?? string.Empty;

    public required string FileName { get; init; }

    public required string MaterialName { get; init; }

    public required TextureType Type { get; init; }
}

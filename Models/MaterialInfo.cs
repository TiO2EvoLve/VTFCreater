namespace VTFCreater.Models;

//材质属性
public class MaterialInfo
{
    public required string Name { get; init; }

    public required string RelativeDirectory { get; init; }

    public TextureFile? BaseColor { get; set; }

    public TextureFile? Normal { get; set; }

    public bool ShouldGenerateVmt => BaseColor is not null;
}

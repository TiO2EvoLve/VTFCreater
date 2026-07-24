using System.Collections.Generic;
using System.Linq;
using VTFCreater.Models;

namespace VTFCreater.Services;

//图片匹配
public static class MaterialMatcher
{
    public static IReadOnlyList<MaterialInfo> BuildMaterials(IEnumerable<TextureFile> textures)
    {
        return textures
            .GroupBy(texture => (texture.RelativeDirectory, texture.MaterialName), TextureGroupComparer.Instance)
            .Select(group =>
            {
                var material = new MaterialInfo
                {
                    Name = group.Key.MaterialName,
                    RelativeDirectory = group.Key.RelativeDirectory
                };

                foreach (var texture in group)
                {
                    if (texture.Type == TextureType.BaseColor)
                    {
                        material.BaseColor = texture;
                    }
                    else
                    {
                        material.Normal = texture;
                    }
                }

                return material;
            })
            .OrderBy(m => m.RelativeDirectory, System.StringComparer.OrdinalIgnoreCase)
            .ThenBy(m => m.Name, System.StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private sealed class TextureGroupComparer : IEqualityComparer<(string RelativeDirectory, string MaterialName)>
    {
        public static TextureGroupComparer Instance { get; } = new();

        public bool Equals((string RelativeDirectory, string MaterialName) x,
            (string RelativeDirectory, string MaterialName) y)
        {
            return System.StringComparer.OrdinalIgnoreCase.Equals(x.RelativeDirectory, y.RelativeDirectory)
                   && System.StringComparer.OrdinalIgnoreCase.Equals(x.MaterialName, y.MaterialName);
        }

        public int GetHashCode((string RelativeDirectory, string MaterialName) obj)
        {
            return System.HashCode.Combine(
                System.StringComparer.OrdinalIgnoreCase.GetHashCode(obj.RelativeDirectory),
                System.StringComparer.OrdinalIgnoreCase.GetHashCode(obj.MaterialName));
        }
    }
}

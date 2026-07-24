using System.Collections.Generic;
using System.Text.Json.Serialization;
using VTFCreater.Enum;

namespace VTFCreater.Models;

//需要保存的配置项
public class AppConfig
{
    public string SourceDirectory { get; set; } = string.Empty;

    public string OutputDirectory { get; set; } = string.Empty;

    public string VTFCmdPath { get; set; } = "dll/x64/VTFCmd.exe";

    public List<string> BaseColorSuffixes { get; set; } = ["", "_color", "_diffuse", "_diff", "_base"];

    public List<string> NormalSuffixes { get; set; } = ["_n", "_normal", "_nm"];

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Formats Format { get; set; } = Formats.DXT1;
    
}

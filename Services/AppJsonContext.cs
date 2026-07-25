using System.Text.Json.Serialization;
using VTFCreater.Models;

namespace VTFCreater.Services;

//避免打包时优化掉反射序列化
[JsonSerializable(typeof(AppConfig))]
public partial class AppJsonContext : JsonSerializerContext
{
}
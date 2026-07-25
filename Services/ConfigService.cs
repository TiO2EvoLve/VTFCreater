using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using VTFCreater.Models;

namespace VTFCreater.Services;
//配置文件保存/加载
public class ConfigService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly string _configPath;

    public AppConfig Config { get; private set; } = new();

    public ConfigService()
    {
        _configPath = Path.Combine(AppContext.BaseDirectory, "config.json");
        Load();
    }

    public void Load()
    {
        if (!File.Exists(_configPath))
        {
            Config = new AppConfig();
            Save();
            return;
        }

        try
        {
            var json = File.ReadAllText(_configPath);
            Config = JsonSerializer.Deserialize(
                json,
                AppJsonContext.Default.AppConfig
            ) ?? new AppConfig();
        }
        catch
        {
            Config = new AppConfig();
        }
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(
            Config,
            AppJsonContext.Default.AppConfig
        );
        File.WriteAllText(_configPath, json);
    }

    public Task SaveAsync()
    {
        Save();
        return Task.CompletedTask;
    }

    public void Update(AppConfig config)
    {
        Config = config;
        Save();
    }
}

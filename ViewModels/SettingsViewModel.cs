using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VTFCreater.Enum;
using VTFCreater.Models;
using VTFCreater.Services;

namespace VTFCreater.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ConfigService _configService;

    [ObservableProperty] private string _sourceDirectory = string.Empty;

    [ObservableProperty] private string _outputDirectory = string.Empty;

    [ObservableProperty] private string _vtfCmdPath = string.Empty;

    [ObservableProperty] private Formats _format = Formats.DXT1;

    [ObservableProperty] private string _newBaseColorSuffix = string.Empty;

    [ObservableProperty] private string _newNormalSuffix = string.Empty;

    [ObservableProperty] private string? _selectedBaseColorSuffix;

    [ObservableProperty] private string? _selectedNormalSuffix;

    [ObservableProperty] private string _statusMessage = string.Empty;

    public ObservableCollection<string> BaseColorSuffixes { get; } = [];

    public ObservableCollection<string> NormalSuffixes { get; } = [];

    public Formats[] AvailableFormats { get; } = [Formats.DXT1, Formats.DXT5];

    public SettingsViewModel(ConfigService configService)
    {
        _configService = configService;
        LoadFromConfig();
    }

    public void LoadFromConfig()
    {
        var config = _configService.Config;

        SourceDirectory = config.SourceDirectory;
        OutputDirectory = config.OutputDirectory;
        VtfCmdPath = config.VTFCmdPath;
        Format = config.Format;

        BaseColorSuffixes.Clear();
        foreach (var suffix in config.BaseColorSuffixes)
        {
            BaseColorSuffixes.Add(suffix);
        }

        NormalSuffixes.Clear();
        foreach (var suffix in config.NormalSuffixes)
        {
            NormalSuffixes.Add(suffix);
        }

        StatusMessage = string.Empty;
    }

    [RelayCommand]
    private async Task BrowseSourceDirectoryAsync()
    {
        var path = await FileDialogHelper.PickFolderAsync("选择源贴图目录");
        if (!string.IsNullOrWhiteSpace(path))
        {
            SourceDirectory = path;
        }
    }

    [RelayCommand]
    private async Task BrowseOutputDirectoryAsync()
    {
        var path = await FileDialogHelper.PickFolderAsync("选择输出目录");
        if (!string.IsNullOrWhiteSpace(path))
        {
            OutputDirectory = path;
        }
    }

    [RelayCommand]
    private async Task BrowseVtfCmdPathAsync()
    {
        var path = await FileDialogHelper.PickFileAsync("选择 VTFCmd.exe", ["*.exe"]);
        if (!string.IsNullOrWhiteSpace(path))
        {
            VtfCmdPath = path;
        }
    }

    [RelayCommand]
    private void AddBaseColorSuffix()
    {
        if (BaseColorSuffixes.Contains(NewBaseColorSuffix))
        {
            return;
        }

        BaseColorSuffixes.Add(NewBaseColorSuffix);
        NewBaseColorSuffix = string.Empty;
    }

    [RelayCommand]
    private void RemoveBaseColorSuffix()
    {
        if (SelectedBaseColorSuffix is null)
        {
            return;
        }

        BaseColorSuffixes.Remove(SelectedBaseColorSuffix);
        SelectedBaseColorSuffix = null;
    }

    [RelayCommand]
    private void AddNormalSuffix()
    {
        if (NormalSuffixes.Contains(NewNormalSuffix))
        {
            return;
        }

        NormalSuffixes.Add(NewNormalSuffix);
        NewNormalSuffix = string.Empty;
    }

    [RelayCommand]
    private void RemoveNormalSuffix()
    {
        if (SelectedNormalSuffix is null)
        {
            return;
        }

        NormalSuffixes.Remove(SelectedNormalSuffix);
        SelectedNormalSuffix = null;
    }

    [RelayCommand]
    private void SaveConfig()
    {
        _configService.Update(new AppConfig
        {
            SourceDirectory = SourceDirectory,
            OutputDirectory = OutputDirectory,
            VTFCmdPath = VtfCmdPath,
            Format = Format,
            BaseColorSuffixes = BaseColorSuffixes.ToList(),
            NormalSuffixes = NormalSuffixes.ToList()
        });

        StatusMessage = "配置已保存到 config.json";
    }

    [RelayCommand]
    private void ReloadConfig()
    {
        _configService.Load();
        LoadFromConfig();
        StatusMessage = "已从 config.json 重新加载";
    }

    [RelayCommand]
    private void Reset()
    {
        _configService.Update(new AppConfig());
        LoadFromConfig();
    }
}

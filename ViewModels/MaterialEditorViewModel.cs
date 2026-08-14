using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VTFCreater.Enum;
using VTFCreater.Models;
using VTFCreater.Services;

namespace VTFCreater.ViewModels;

public partial class MaterialEditorViewModel : ViewModelBase
{
    private readonly ConfigService _configService;
    private readonly MaterialProcessingService _materialProcessingService;
    private readonly LogService _logService;

    public ObservableCollection<MaterialSlot> Slots { get; } =
    [
        new("BaseColor", "Base Color", true),
        new("Normal", "Normal"),
        new("Alpha", "Alpha"),
        new("Emissive", "Emissive")
    ];

    public Array Shaders { get; } = global::System.Enum.GetValues(typeof(MaterialShader));
    public Array Formats { get; } = global::System.Enum.GetValues(typeof(Formats));
    public Array SizeClamps { get; } = global::System.Enum.GetValues(typeof(SizeClamp));

    [ObservableProperty] private string _materialName = "material";
    [ObservableProperty] private MaterialShader _selectedShader = MaterialShader.VertexLitGeneric;
    [ObservableProperty] private Formats _selectedFormat;
    [ObservableProperty] private SizeClamp _selectedSizeClamp;
    [ObservableProperty] private string _statusMessage = "将图片拖入对应插槽，或点击“选择图片”。";
    [ObservableProperty] private bool _isGenerating;

    public MaterialEditorViewModel(ConfigService configService, MaterialProcessingService materialProcessingService,
        LogService logService)
    {
        _configService = configService;
        _materialProcessingService = materialProcessingService;
        _logService = logService;
        _selectedFormat = configService.Config.Format;
        _selectedSizeClamp = configService.Config.SizeClamp;
    }

    [RelayCommand]
    private async Task BrowseSlotAsync(MaterialSlot? slot)
    {
        if (slot is null) return;
        var file = await FileDialogHelper.PickFileAsync($"选择 {slot.DisplayName} 图片", ["*.png", "*.jpg", "*.jpeg", "*.tga", "*.bmp"]);
        if (file is not null) AssignSlotFile(slot.Key, file);
    }

    [RelayCommand]
    private void ClearSlot(MaterialSlot? slot)
    {
        slot?.Clear();
        StatusMessage = slot is null ? StatusMessage : $"已清空 {slot.DisplayName} 插槽。";
    }

    [RelayCommand]
    private async Task GenerateAsync()
    {
        var normalizedName = MaterialName.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName) || normalizedName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            StatusMessage = "请输入有效的材质名称。";
            return;
        }
        if (!Slots.First(slot => slot.Key == "BaseColor").HasFile)
        {
            StatusMessage = "Base Color 是必填插槽。";
            return;
        }

        var directory = await FileDialogHelper.PickFolderAsync("选择 VTF 和 VMT 的保存位置");
        if (directory is null) return;

        IsGenerating = true;
        try
        {
            await _materialProcessingService.GenerateAsync(normalizedName, directory, SelectedShader, Slots,
                SelectedFormat, SelectedSizeClamp, _configService.Config.VTFCmdPath, _logService);
            StatusMessage = $"已生成 {normalizedName}.vtf、相关插槽 VTF 和 {normalizedName}.vmt。";
        }
        catch (Exception exception)
        {
            StatusMessage = $"生成失败：{exception.Message}";
            _logService.Error(exception.Message);
        }
        finally { IsGenerating = false; }
    }

    public void AssignSlotFile(string key, string path)
    {
        if (!File.Exists(path)) return;
        var slot = Slots.FirstOrDefault(candidate => candidate.Key == key);
        if (slot is null) return;
        try
        {
            slot.SetFile(path);
            StatusMessage = $"已将 {Path.GetFileName(path)} 设置为 {slot.DisplayName}。";
        }
        catch (Exception exception)
        {
            StatusMessage = $"无法读取图片：{exception.Message}";
        }
    }
}

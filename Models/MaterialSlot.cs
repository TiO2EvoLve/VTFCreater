using System.IO;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace VTFCreater.Models;

public sealed partial class MaterialSlot : ObservableObject
{
    public MaterialSlot(string key, string displayName, bool isRequired = false)
    {
        Key = key;
        DisplayName = displayName;
        IsRequired = isRequired;
    }

    public string Key { get; }
    public string DisplayName { get; }
    public bool IsRequired { get; }
    [ObservableProperty] private string? _filePath;
    public string FileName => FilePath is null ? "拖入或点击按钮选择图片" : Path.GetFileName(FilePath);
    [ObservableProperty] private Bitmap? _preview;
    public bool HasFile => FilePath is not null;

    public void SetFile(string path)
    {
        Preview?.Dispose();
        FilePath = path;
        Preview = new Bitmap(path);
        OnPropertyChanged(nameof(FileName));
        OnPropertyChanged(nameof(HasFile));
    }

    public void Clear()
    {
        Preview?.Dispose();
        Preview = null;
        FilePath = null;
        OnPropertyChanged(nameof(FileName));
        OnPropertyChanged(nameof(HasFile));
    }
}

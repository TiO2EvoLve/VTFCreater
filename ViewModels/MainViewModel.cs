using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VTFCreater.Services;

namespace VTFCreater.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly ConfigService _configService;

    [ObservableProperty] private ViewModelBase _currentPage;

    public HomeViewModel Home { get; }

    public SettingsViewModel Settings { get; }

    public MaterialEditorViewModel MaterialEditor { get; }

    public MainViewModel(ConfigService configService, HomeViewModel home, SettingsViewModel settings,
        MaterialEditorViewModel materialEditor)
    {
        _configService = configService;
        Home = home;
        Settings = settings;
        MaterialEditor = materialEditor;
        _currentPage = Home;
    }

    [RelayCommand]
    private void NavigateHome()
    {
        Home.RefreshSummary();
        CurrentPage = Home;
    }

    [RelayCommand]
    private void NavigateSettings()
    {
        Settings.LoadFromConfig();
        CurrentPage = Settings;
    }

    [RelayCommand]
    private void NavigateMaterialEditor()
    {
        CurrentPage = MaterialEditor;
    }

    public bool IsHomeActive => ReferenceEquals(CurrentPage, Home);

    public bool IsSettingsActive => ReferenceEquals(CurrentPage, Settings);

    public bool IsMaterialEditorActive => ReferenceEquals(CurrentPage, MaterialEditor);

    partial void OnCurrentPageChanged(ViewModelBase value)
    {
        OnPropertyChanged(nameof(IsHomeActive));
        OnPropertyChanged(nameof(IsSettingsActive));
        OnPropertyChanged(nameof(IsMaterialEditorActive));
    }
}

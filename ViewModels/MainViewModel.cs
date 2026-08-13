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

    public MainViewModel(ConfigService configService, HomeViewModel home, SettingsViewModel settings)
    {
        _configService = configService;
        Home = home;
        Settings = settings;
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

    public bool IsHomeActive => ReferenceEquals(CurrentPage, Home);

    public bool IsSettingsActive => ReferenceEquals(CurrentPage, Settings);

    partial void OnCurrentPageChanged(ViewModelBase value)
    {
        OnPropertyChanged(nameof(IsHomeActive));
        OnPropertyChanged(nameof(IsSettingsActive));
    }
}

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

    public MainViewModel()
    {
        _configService = new ConfigService();
        var logService = new LogService();
        var processingService = new ProcessingService();

        Home = new HomeViewModel(_configService, processingService, logService);
        Settings = new SettingsViewModel(_configService);
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

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DryIoc;
using VTFCreater.Models;
using VTFCreater.Services;
using VTFCreater.ViewModels;
using VTFCreater.Views;

namespace VTFCreater;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        RegisterMyServices();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(),
            };
        }
        base.OnFrameworkInitializationCompleted();
    }
    private void RegisterMyServices()
    {
        IoC.Container.Register<MainWindow>(Reuse.Singleton);
        IoC.Container.Register<HomeViewModel>(Reuse.Singleton);
        IoC.Container.Register<ConfigService>(Reuse.Singleton);
        IoC.Container.Register<ProcessingService>(Reuse.Singleton);
        IoC.Container.Register<LogService>(Reuse.Singleton);
        
    }
}
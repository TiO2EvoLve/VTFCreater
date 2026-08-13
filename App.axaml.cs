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
        IoC.RegisterServices();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = IoC.Resolve<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
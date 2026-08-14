using DryIoc;
using VTFCreater.Services;
using VTFCreater.ViewModels;

namespace VTFCreater.Models;

public static class IoC
{
    public static IContainer Container { get; } = new Container();

    public static void RegisterServices()
    {
        Container.Register<ConfigService>(Reuse.Singleton);
        Container.Register<LogService>(Reuse.Singleton);
        Container.Register<ProcessingService>(Reuse.Singleton);
        Container.Register<MaterialProcessingService>(Reuse.Singleton);
        Container.Register<HomeViewModel>(Reuse.Singleton);
        Container.Register<MaterialEditorViewModel>(Reuse.Singleton);
        Container.Register<SettingsViewModel>(Reuse.Singleton);
        Container.Register<MainViewModel>(Reuse.Singleton);
    }

    public static T Resolve<T>() => Container.Resolve<T>();
}

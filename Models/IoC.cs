using DryIoc;

namespace VTFCreater.Models;

public static class IoC
{
    public static IContainer Container { get; } = new Container();

    public static T Resolve<T>() => Container.Resolve<T>();
}
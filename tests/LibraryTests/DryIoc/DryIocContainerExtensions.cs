using DryIoc;

namespace AvaloniaApplication1Tests;

public static class DryIocContainerExtensions
{
    public static IContainer Clone(this IContainer container) =>
        container.With(container.Rules, null, RegistrySharing.CloneAndDropCache, Container.NewSingletonScope());
}

namespace WPF.Airprint.Ioc
{
    using Prism.Ioc;
    using WPF.Airprint.Docker;
    using WPF.Airprint.Bonjour;
    using DryIoc;
    using WPF.Airprint.Terminal;

    public static class ContainerRegistryExtensions
    {
        public static IContainerRegistry AddServices(this IContainerRegistry registry)
        {
            registry.Register<IDockerService, DockerService>();
            registry.Register<IBonjourService, BonjourService>();
            registry.Register<ITerminalService, TerminalService>();

            return registry;
        }
    }
}

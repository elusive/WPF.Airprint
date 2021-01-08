namespace WPF.Airprint
{
    using Prism.Ioc;
    using Docker;
    using Bonjour;
    using DryIoc;
    using PrintQueue;
    using Terminal;
    using WPF.Airprint.DeviceEnumeration;

    public static class ContainerRegistryExtensions
    {
        public static IContainerRegistry AddAirprintServices(this IContainerRegistry registry)
        {
            registry.Register<IDockerService, DockerService>();
            registry.Register<IBonjourService, BonjourService>();
            registry.RegisterSingleton<IWindowsDeviceService, WindowsDeviceService>();
            registry.Register<ITerminalService, TerminalService>();
            registry.RegisterSingleton<IPrintQueueService, PrintQueueService>();

            return registry;
        }
    }
}

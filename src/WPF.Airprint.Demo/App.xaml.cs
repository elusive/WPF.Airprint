
namespace WPF.Airprint.Demo
{
    using DryIoc;
    using Prism.DryIoc;
    using Prism.Ioc;
    using Prism.Modularity;
    using WPF.Airprint.Ioc;
    using WPF.Airprint.Demo.Views;
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void OnStartup(StartupEventArgs args)
        {
            base.OnStartup(args);
        }


        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected static IContainer AppContainer { get; set; }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.AddServices();

            AppContainer = containerRegistry.GetContainer();
        }


        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<MainModule>();
            moduleCatalog.AddModule<PrintersModule.PrintersModule>();
        }
    }
}

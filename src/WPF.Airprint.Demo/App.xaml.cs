
namespace WPF.Airprint.Demo
{
    using DryIoc;
    using Prism.DryIoc;
    using Prism.Ioc;
    using Prism.Modularity;
    using WPF.Airprint;
    using WPF.Airprint.Demo.Views;
    using System;
    using System.Windows;
    using WPF.Airprint.Docker;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void OnStartup(StartupEventArgs args)
        {
            base.OnStartup(args);

            // application startup actions
            ProcessStartupActions();
        }


        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected static IContainer AppContainer { get; set; }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.AddAirprintServices();

            AppContainer = containerRegistry.GetContainer();
        }


        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<MainModule>();
            moduleCatalog.AddModule<PrintersModule.PrintersModule>();
        }

        private void ProcessStartupActions()
        {
            var actions = AppContainer.ResolveMany<IStartupAction>();
            foreach (var action in actions)
            {
                try
                {
                    action.ProcessStartupAction();
                }
                catch (Exception ex)
                {
                    // log and throw?
                }
            }
        }
    }
}

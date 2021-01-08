namespace WPF.Airprint.Demo.PrintersModule
{
    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Regions;
    using WPF.Airprint.Demo.PrintersModule.Views;
    using WPF.Airprint.Mvvm;


    public class PrintersModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(AddPrinter));
            regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(FindPrinter));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<AddPrinter>();
            containerRegistry.RegisterForNavigation<FindPrinter>();
        }
    }
}


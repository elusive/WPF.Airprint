namespace WPF.Airprint.Demo
{
    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Regions;
    using WPF.Airprint.Mvvm;
    using WPF.Airprint.Demo.Views;

    public class MainModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(Home));
            regionManager.RegisterViewWithRegion(RegionNames.FooterRegion, typeof(StatusBar));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Home>();
        }
    }
}
namespace WPF.Airprint.Demo.ViewModels
{
    using Prism.Regions;
    using WPF.Airprint.Mvvm;

    public class HomeViewModel : RegionViewModelBase
    {
        public HomeViewModel(IRegionManager regionManager) : base(regionManager)
        {
        }

        public override void OnNavigatedTo(NavigationContext context)
        {
            base.OnNavigatedTo(context);

            // spin up the docker print server here...
        }
    }
}

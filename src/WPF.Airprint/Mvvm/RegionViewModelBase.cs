namespace WPF.Airprint.Mvvm
{
    using Prism.Regions;
    using System;

    public class RegionViewModelBase : ViewModelBase, INavigationAware, IConfirmNavigationRequest
    {
        protected IRegionManager RegionManager { get; private set; }

        public RegionViewModelBase(IRegionManager regionManager)
        {
            RegionManager = regionManager;
        }

        public virtual void ConfirmNavigationRequest(NavigationContext context, Action<bool> done)
        {
            done(true);
        }

        public virtual bool IsNavigationTarget(NavigationContext context)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext context)
        {

        }

        public virtual void OnNavigatedTo(NavigationContext context)
        {

        }
    }
}

namespace WPF.Airprint.Demo.ViewModels
{
    using Prism.Commands;
    using Prism.Events;
    using Prism.Mvvm;
    using Prism.Regions;
    using WPF.Airprint.Events;
    using WPF.Airprint.Mvvm;
    using System;
    using System.Threading.Tasks;

    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _events;

        public MainWindowViewModel(IEventAggregator events, IRegionManager regionManager)
        {
            _events = events;
            _regionManager = regionManager;

            NavigateCommand = new DelegateCommand<string>(ExecuteNavigate);

            Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(_ => 
                _events.GetEvent<StatusMessageEvent>().Publish("Main window loaded. Check here for information while using the demo..."));
        }

        public DelegateCommand<string> NavigateCommand { get; private set; }

        private void ExecuteNavigate(string viewPath)
        {
            if (viewPath != null)
            {
                _regionManager.RequestNavigate(RegionNames.MainRegion, viewPath);
            }
        }

    }
}

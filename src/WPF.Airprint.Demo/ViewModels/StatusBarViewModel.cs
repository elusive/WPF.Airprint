namespace WPF.Airprint.Demo.ViewModels
{
    using PrintersModule.Events;
    using Prism.Events;
    using Prism.Regions;
    using WPF.Airprint.Mvvm;

    public class StatusBarViewModel : RegionViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private string _statusMessage;

        public StatusBarViewModel(IEventAggregator eventAggregator, IRegionManager regionManager) : base(regionManager)
        {
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<StatusMessageEvent>().Subscribe(StatusMessageEventHandler);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private void StatusMessageEventHandler(string message)
        {
            StatusMessage = message;
        }
    }
}

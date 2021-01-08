namespace WPF.Airprint.Demo.PrintersModule.ViewModels
{
    using Prism.Commands;
    using Prism.Events;
    using Prism.Regions;
    using WPF.Airprint.Bonjour;
    using WPF.Airprint.Events;
    using WPF.Airprint.Mvvm;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Threading;
    using WPF.Airprint.Demo.PrintersModule;
    using WPF.Airprint.DeviceEnumeration;
    using System.Collections.Generic;

    public class FindPrinterViewModel : RegionViewModelBase
    {
        private readonly IEventAggregator _events;
        private readonly IBonjourService _bonjourService;
        private readonly IWindowsDeviceService _devices;
        private ObservableCollection<PrinterFoundViewModel> _availablePrinters;
        private bool _isBusy = false;
        private double _findProgress;
        private DeviceFoundViewModel _selectedPrinter;
        private ObservableCollection<string> _printerDetails;
        private List<DeviceFoundViewModel> _devicesFound;

        public FindPrinterViewModel(IRegionManager regionManager, IEventAggregator events, IBonjourService bonjourService, IWindowsDeviceService devices) : base(regionManager)
        {
            _events = events;
            _bonjourService = bonjourService;
            _devices = devices;

            FindPrinterCommand = new DelegateCommand(ExecuteFindPrinterCommand, CanExecuteFindPrinterCommand);
            DevicesFound = new ObservableCollection<DeviceFoundViewModel>();
            _devicesFound = new List<DeviceFoundViewModel>();

            _devices.DeviceFound += _devices_DeviceFound;
        }

        public override void OnNavigatedFrom(NavigationContext context)
        {
            // add the currently selected printer details instance to context
            if (SelectedPrinter != null)
            {
                context.Parameters.Add(Constants.PrinterKey, SelectedPrinter.Model);
            }

            base.OnNavigatedFrom(context);
        }

        public bool CanExecuteFindPrinterCommand()
        {
            return !IsBusy;
        }

        public ObservableCollection<DeviceFoundViewModel> DevicesFound { get; private set; }

        public ObservableCollection<PrinterFoundViewModel> AvailablePrinters 
        {
            get => _availablePrinters;
            set => SetProperty(ref _availablePrinters, value);
        }

        public ObservableCollection<string> PrinterDetails
        {
            get => _printerDetails;
            set => SetProperty(ref _printerDetails, value);
        }

        public DeviceFoundViewModel SelectedPrinter
        {
            get { return _selectedPrinter; }
            set
            {
                SetProperty(ref _selectedPrinter, value);
                RaisePropertyChanged(nameof(ShowPrinterDetails));
                PopulatePrinterDetails();
            }
        }

        public DelegateCommand FindPrinterCommand
        {
            get; 
            private set;
        }

        public double FindProgress
        {
            get => _findProgress;
            set => SetProperty(ref _findProgress, value);
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set {
                SetProperty(ref _isBusy, value);
                FindPrinterCommand.RaiseCanExecuteChanged();
            }
        }

        public bool ShowPrinterDetails
        {
            get => SelectedPrinter != null;
        }

        private async void ExecuteFindPrinterCommand()
        {        

            //var lst = await _bonjourService.FindPrinters();
            //AvailablePrinters = new ObservableCollection<PrinterFoundViewModel>(lst.Select(x => new PrinterFoundViewModel(x)));

            _devices.CancelDiscoverDevices();
            _devices.DiscoverDevices();

            ShowFindProgress();

            _events.GetEvent<StatusMessageEvent>().Publish(Constants.Messages.GetNetworkSearchStatusMessage(DevicesFound.Count()));
        }

        private void _devices_DeviceFound(object sender, DeviceFound e)
        {
            if (e == null) return;
            _devicesFound.Add(new DeviceFoundViewModel(e));
            DevicesFound = new ObservableCollection<DeviceFoundViewModel>(_devicesFound);
            RaisePropertyChanged(nameof(DevicesFound));
        }


        private void ShowFindProgress()
        {
            IsBusy = true;
            var started = DateTime.Now;
            new DispatcherTimer(
                    TimeSpan.FromMilliseconds(50),
                    DispatcherPriority.Normal,
                    new EventHandler((o, e) =>
                    {
                        var totalDuration = started.AddMilliseconds(3000).Ticks - started.Ticks;
                        var currentProgress = DateTime.Now.Ticks - started.Ticks;
                        var currentProgressPercent = 100.0 / totalDuration * currentProgress;

                        FindProgress = currentProgressPercent;

                        if (FindProgress >= 100)
                        {
                            IsBusy = false;
                            FindProgress = 0;
                            if (o is DispatcherTimer timer)
                            {
                                timer.Stop();
                            }
                        }

                    }), Dispatcher.CurrentDispatcher);
        }

        private async void PopulatePrinterDetails()
        {
            PrinterDetails = new ObservableCollection<string>();
 
            PrinterDetails.AddRange(SelectedPrinter.Details);
        }
    }
}

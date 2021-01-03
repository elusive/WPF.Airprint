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

    public class FindPrinterViewModel : RegionViewModelBase
    {
        private readonly IEventAggregator _events;
        private readonly IBonjourService _bonjourService;
        private ObservableCollection<PrinterFoundViewModel> _availablePrinters;
        private bool _isBusy = false;
        private double _findProgress;
        private PrinterFoundViewModel _selectedPrinter;
        private PrinterFoundDetails _lastPrinterDetailModel;
        private ObservableCollection<string> _printerDetails;

        public FindPrinterViewModel(IRegionManager regionManager, IEventAggregator events, IBonjourService bonjourService) : base(regionManager)
        {
            _events = events;
            _bonjourService = bonjourService;

            FindPrinterCommand = new DelegateCommand(ExecuteFindPrinterCommand);
        }

        public override void OnNavigatedFrom(NavigationContext context)
        {
            // add the currently selected printer details instance to context
            if (_lastPrinterDetailModel != null)
            {
                context.Parameters.Add(Constants.PrinterKey, _lastPrinterDetailModel);
            }

            base.OnNavigatedFrom(context);
        }

        public bool CanExecuteFindPrinterCommand()
        {
            return IsBusy;
        }

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

        public PrinterFoundViewModel SelectedPrinter
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
            _lastPrinterDetailModel = null;

            ShowFindProgress();

            var lst = await _bonjourService.FindPrinters();
            AvailablePrinters = new ObservableCollection<PrinterFoundViewModel>(lst.Select(x => new PrinterFoundViewModel(x)));

            _events.GetEvent<StatusMessageEvent>().Publish(Constants.Messages.GetNetworkSearchStatusMessage(lst.Count()));
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
                        var totalDuration = started.AddMilliseconds(BonjourService.SearchTimeMilliseconds).Ticks - started.Ticks;
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
            var details = await _bonjourService.GetPrinterDetails(SelectedPrinter.Model);

            PrinterDetails = new ObservableCollection<string>();
            PrinterDetails.Add($"InterfaceIndex = {details.InterfaceIndex}");
            PrinterDetails.Add($"FullName = {details.FullName}");
            PrinterDetails.Add($"HostName = {details.HostName}");
            PrinterDetails.Add($"Port   =   {details.Port}");
            PrinterDetails.AddRange(details.RenderTxtRecord());

            _lastPrinterDetailModel = details;
        }
    }
}

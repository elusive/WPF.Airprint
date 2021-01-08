
namespace WPF.Airprint.Demo.PrintersModule.ViewModels
{
    using Prism.Commands;
    using Prism.Events;
    using Prism.Regions;
    using WPF.Airprint.Docker;
    using WPF.Airprint.Mvvm;
    using WPF.Airprint.Terminal;
    using System.Threading.Tasks;
    using System;
    using System.Diagnostics;
    using System.Windows.Threading;
    using Events;
    using PrintQueue;
    using WPF.Airprint.DeviceEnumeration;

    public class AddPrinterViewModel : RegionViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDockerService _dockerService;
        private readonly ITerminalService _terminal;
        private readonly IPrintQueueService _queueService;
        private string _printerHostName;
        private string _printerUri;
        private string _printServerImageId;
        private bool _printServerContainerStarted;
        private bool _showPrinterUri;
        private bool _showPrinterInfo;
        private string _printerQueueName;
        private bool _showPrinterQueue;
        private double _addProgress;
        private bool _isBusy;
        private string _printerQueueStatus;
        private string _printServerContainerId;
        private bool _showPrintFile;
        private string _fileToPrint;
        private DeviceFound _printerInfo;

        public DelegateCommand AddPrinterCommand { get; private set; }
        public DelegateCommand PrintCommand { get; private set; }

        public AddPrinterViewModel(IRegionManager regionManager, 
            IEventAggregator eventAggregator, 
            IDockerService dockerService, 
            ITerminalService terminal,
            IPrintQueueService queueService) : base(regionManager)
        {
            _eventAggregator = eventAggregator;
            _dockerService = dockerService;
            _terminal = terminal;
            _queueService = queueService;

            AddPrinterCommand = new DelegateCommand(ExecuteAddPrinter, CanExecuteAddPrinter);
            PrintCommand = new DelegateCommand(ExecutePrintCommand, () => true);//!string.IsNullOrEmpty(FileToPrint));
        }

        public override void OnNavigatedTo(NavigationContext context)
        {
            base.OnNavigatedTo(context);

            _printerInfo = context.Parameters[Constants.PrinterKey] as DeviceFound;
            if (_printerInfo == null)
            {
                _eventAggregator.GetEvent<StatusMessageEvent>().Publish(Constants.Messages.NoPrinterDetailToAdd);
                return;
            }

            PrinterHostName = _printerInfo.HostName;
            ShowPrinterUri = false;
            ShowPrintServerInfo = false;
            ShowPrinterQueue = false;

            AddPrinterCommand.RaiseCanExecuteChanged();

            _eventAggregator.GetEvent<StatusMessageEvent>().Publish($"Click Add Printer to create driverless queue for: {PrinterHostName}");
        }

        public string PrinterHostName
        {
            get => _printerHostName;
            set => SetProperty(ref _printerHostName, value);
        }

        public string PrinterUri
        {
            get => _printerUri;
            set => SetProperty(ref _printerUri, value);
        }

        public bool ShowPrinterUri
        {
            get => _showPrinterUri;
            set => SetProperty(ref _showPrinterUri, value);
        }

        public string PrintServerImageId
        {
            get => _printServerImageId;
            set => SetProperty(ref _printServerImageId, value);
        }

        public string PrintServerContainerId
        {
            get => _printServerContainerId;
            set => SetProperty(ref _printServerContainerId, value);
        }

        public bool PrintServerContainerStarted
        {
            get => _printServerContainerStarted;
            set => SetProperty(ref _printServerContainerStarted, value);
        }

        public bool ShowPrintServerInfo
        {
            get => _showPrinterInfo;
            set => SetProperty(ref _showPrinterInfo, value);
        }

        public string PrinterQueueName
        {
            get => _printerQueueName;
            set => SetProperty(ref _printerQueueName, value);
        }

        public bool ShowPrinterQueue
        {
            get => _showPrinterQueue;
            set => SetProperty(ref _showPrinterQueue, value);
        }

        public bool ShowPrintFile
        {
            get => _showPrintFile;
            set => SetProperty(ref _showPrintFile, value);
        }

        public string FileToPrint
        {
            get => _fileToPrint;
            set {
                SetProperty(ref _fileToPrint, value);
                PrintCommand.RaiseCanExecuteChanged();
            }
        }

        public double AddProgress
        {
            get => _addProgress;
            set => SetProperty(ref _addProgress, value);
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                SetProperty(ref _isBusy, value);
                AddPrinterCommand.RaiseCanExecuteChanged();
            }
        }

        public string PrinterQueueStatus
        {
            get => _printerQueueStatus;
            set => SetProperty(ref _printerQueueStatus, value);
        }

        private bool CanExecuteAddPrinter()
        {
            return !string.IsNullOrEmpty(PrinterHostName) && !IsBusy;
        }
        private void ShowAddProgress()
        {
            IsBusy = true;
            var started = DateTime.Now;
            new DispatcherTimer(
                TimeSpan.FromMilliseconds(50),
                DispatcherPriority.Normal,
                new EventHandler((o, e) =>
                {
                    var totalDuration = started.AddMilliseconds(8000).Ticks - started.Ticks;
                    var currentProgress = DateTime.Now.Ticks - started.Ticks;
                    var currentProgressPercent = 100.0 / totalDuration * currentProgress;

                    AddProgress = currentProgressPercent;
                    Debug.WriteLine(AddProgress);

                    if (AddProgress >= 100)
                    {
                        IsBusy = false;
                        AddProgress = 0;
                        if (o is DispatcherTimer timer)
                        {
                            timer.Stop();
                        }
                    }

                }), Dispatcher.CurrentDispatcher);
        }

        private async void ExecuteAddPrinter()
        {
            ShowAddProgress();

            PrinterUri = string.Format(CommandStrings.DeviceUriFormat, _printerInfo.IpAddress);
            _eventAggregator.GetEvent<StatusMessageEvent>().Publish(Constants.Messages.GetBuildUriStatusMessage(PrinterUri));
            ShowPrinterUri = true;


            var isPrintServerRunning = await CheckPrintServer();
            ShowPrintServerInfo = true;
            if (!isPrintServerRunning)
            {
                throw new Exception(Constants.Messages.PrintServerContainerMustBeRunning);
            }
            _eventAggregator.GetEvent<StatusMessageEvent>().Publish($"Driverless print queue running: {isPrintServerRunning}");


            ShowPrinterQueue = await CreatePrintQueue(PrinterUri);

            ShowPrintFile = await CheckPrintQueueStatus();

            RaisePropertyChanged(nameof(ShowPrinterQueue));
            RaisePropertyChanged(nameof(ShowPrintFile));
        }

        // BONJOUR
        //private async Task<string> BuildPrinterUri()
        //{
        //    var result = await _terminal.ExecuteAsync(CommandType.GetIpAddress, _printerHostName);
        //    if (result != null)
        //    {
        //        var ip = Regex.Split(result.StandardOutput.Skip(1).First(), @"\s+")[5];
        //        return string.Format(CommandStrings.DeviceUriFormat, ip);
        //    }

        //    return string.Empty;
        //}

        private async Task<bool> CheckPrintServer()
        {
            // is the docker image there?
            var printServerSha256 = await _dockerService.IsImageExisting(Constants.PrintServerExistingImageName);
            var printServerImageId = printServerSha256.Replace(Constants.DockerImageIdPrefix, "");
            _eventAggregator.GetEvent<StatusMessageEvent>().Publish(Constants.Messages.GetPrintServerIdStatusMessage(printServerImageId));
            if (printServerImageId == null)
            {
                // TODO: Handle image missing error.
                return false;
            }

            PrintServerImageId = printServerImageId;


            // is container already existing
            var containerId = await _dockerService.IsContainerExisting(PrintServerImageId);
            if (containerId == null)
            {
                var started = await _dockerService.StartContainer(printServerImageId);
                PrintServerContainerStarted = started;
                if (!PrintServerContainerStarted)
                {
                    throw new System.ApplicationException(Constants.Messages.PrintServerContainerMustBeRunning);
                }

                return started;
            }
            
            PrintServerContainerId = containerId;
            return true;
        }

        private async Task<bool> CreatePrintQueue(string printerUri)
        {
            PrinterQueueName = await _queueService.CreatePrintQueue(PrintServerContainerId, PrinterUri);
            _eventAggregator.GetEvent<StatusMessageEvent>().Publish($"Driverless print queue created with name: {PrinterQueueName}");
            return !string.IsNullOrEmpty(PrinterQueueName);
        }

        private async Task<bool> CheckPrintQueueStatus()
        {
            var status = await _queueService.CheckPrintQueueStatus(PrintServerContainerId, PrinterQueueName);

            PrinterQueueStatus = status;
            PrintCommand.RaiseCanExecuteChanged();

            _eventAggregator.GetEvent<StatusMessageEvent>().Publish($"Driverless print queue created with name: {PrinterQueueName}");

            return !string.IsNullOrEmpty(status);
        }

        private void ExecutePrintCommand()
        {
            Task.Run(async() =>
            {
                //var result = await _queueService.PrintPdfFile(FileToPrint, PrinterQueueName);
                var result = await _terminal.ExecuteAsync(CommandType.PrintServerPrintFile, PrinterQueueName, FileToPrint);
            });
        }

        private void ResetView()
        {
            ShowPrinterQueue = false;
            ShowPrinterUri = false;
            ShowPrintServerInfo = false;
            ShowPrintFile = false;
            PrintServerContainerStarted = false;
            PrinterQueueName = string.Empty;
            PrinterQueueStatus = string.Empty;
            PrintServerImageId = string.Empty;
        }
    }
}

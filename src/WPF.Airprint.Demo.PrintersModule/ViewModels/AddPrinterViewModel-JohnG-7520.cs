
namespace WPF.Airprint.Demo.PrintersModule.ViewModels
{
    using Prism.Commands;
    using Prism.Events;
    using Prism.Regions;
    using WPF.Airprint.Bonjour;
    using WPF.Airprint.Docker;
    using WPF.Airprint.Mvvm;
    using WPF.Airprint.Terminal;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System;
    using System.Linq;
    using WPF.Airprint.Events;

    public class AddPrinterViewModel : RegionViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDockerService _dockerService;
        private readonly ITerminalService _terminal;
        private string _printerHostName;
        private string _printerUri;
        private string _printServerImageId;
        private bool _printServerContainerStarted;
        private bool _showPrinterUri;
        private bool _showPrinterInfo;

        public DelegateCommand AddPrinterCommand { get; set; }

        public bool IsPrinterFormValid { get; set; }

        public AddPrinterViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IDockerService dockerService, ITerminalService terminal) : base(regionManager)
        {
            _eventAggregator = eventAggregator;
            _dockerService = dockerService;
            _terminal = terminal;
            
            AddPrinterCommand = new DelegateCommand(ExecuteAddPrinter, CanExecuteAddPrinter);
        }

        public override void OnNavigatedTo(NavigationContext context)
        {
            base.OnNavigatedTo(context);

            var printerDetail = context.Parameters[Constants.PrinterKey] as PrinterFoundDetails;
            if (printerDetail == null)
            {
                _eventAggregator.GetEvent<StatusMessageEvent>().Publish(Constants.Messages.NoPrinterDetailToAdd);
                return;
            }

            PrinterHostName = printerDetail.HostName;
            ShowPrinterUri = false;
            ShowPrintServerInfo = false;

            AddPrinterCommand.RaiseCanExecuteChanged();
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

        private bool CanExecuteAddPrinter()
        {
            return !string.IsNullOrEmpty(PrinterHostName);
        }

        private async void ExecuteAddPrinter()
        {
            var uri = await BuildPrinterUri();
            _eventAggregator.GetEvent<StatusMessageEvent>().Publish(Constants.Messages.GetBuildUriStatusMessage(uri));
            PrinterUri = uri;
            ShowPrinterUri = true;
            

            var isPrintServerRunning = await CheckPrintServer();
            ShowPrintServerInfo = true;
            if (!isPrintServerRunning)
            {
                throw new Exception(Constants.Messages.PrintServerContainerMustBeRunning);
            }
        }

        private async Task<string> BuildPrinterUri()
        {
            //_terminal.CancelAfter(_terminal.CommandTimeoutMilliseconds);
            var result = await _terminal.ExecuteAsync(CommandType.GetIpAddress, _printerHostName);
            if (result != null)
            {
                var ip = Regex.Split(result.StandardOutput.Skip(1).First(), @"\s+")[5];
                return string.Format(CommandStrings.DeviceUriFormat, ip);
            }

            return string.Empty;
        }

        private async Task<bool> CheckPrintServer()
        {
            // is the docker image there?
            var printServerSha256 = await _dockerService.IsImageExisting(Constants.PrintServerExistingImageName);
            var printServerImageId = printServerSha256.Replace("sha256:", "");
            _eventAggregator.GetEvent<StatusMessageEvent>().Publish(Constants.Messages.GetPrintServerIdStatusMessage(printServerImageId));
            PrintServerImageId = printServerImageId;


            // is container already existing
            var containerId = await _dockerService.IsContainerExisting(PrintServerImageId);


            // can we get the container started?
            var started = (containerId != null) || await _dockerService.StartContainer(printServerImageId);
            PrintServerContainerStarted = started;

            return started;
        }
    }
}

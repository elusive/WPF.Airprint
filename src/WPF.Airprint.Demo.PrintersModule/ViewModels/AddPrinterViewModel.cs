
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
    using WPF.Airprint.Events;

    public class AddPrinterViewModel : RegionViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDockerService _dockerService;
        private readonly ITerminalService _terminal;
        private string _printerHostName;


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

            _printerHostName = printerDetail.HostName;
            AddPrinterCommand.RaiseCanExecuteChanged();
        }

        public string PrinterHostName
        {
            get => _printerHostName;
            set => SetProperty(ref _printerHostName, value);
        }

        private bool CanExecuteAddPrinter()
        {
            return !string.IsNullOrEmpty(PrinterHostName);
        }

        private async void ExecuteAddPrinter()
        {
            var uri = await BuildPrinterUri();
            var isPrintServerRunning = await CheckPrintServer();

            if (isPrintServerRunning)
            {

            }
            else
            {
                throw new Exception(Constants.Messages.PrintServerContainerMustBeRunning);
            }
        }

        private async Task<string> BuildPrinterUri()
        {
            var linesOut = new List<string>();
            var linesErr = new List<string>();
            var getIpCmd = string.Format(CommandStrings.GetIpAddressFormat, _printerHostName);
            var getIpPsi = _terminal.CreateCmdStartInfo(getIpCmd);
            var result = await _terminal.ExecuteAsync(getIpPsi, linesOut, linesErr);
            var ip = Regex.Split(result.StandardOutput[1], "/s")[5];
            return string.Format(CommandStrings.DeviceUriFormat, ip);
        }

        private async Task<bool> CheckPrintServer()
        {
            // is the docker image there?
            var printServerImageId = await _dockerService.IsImageExisting(Constants.PrintServerExistingImageName);

            // can we get the container started?
            var started = await _dockerService.StartContainer(printServerImageId);

            return true;
        }
    }
}

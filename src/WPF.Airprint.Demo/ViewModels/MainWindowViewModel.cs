namespace WPF.Airprint.Demo.ViewModels
{
    using Prism.Commands;
    using Prism.Events;
    using Prism.Mvvm;
    using Prism.Regions;
    using WPF.Airprint.Mvvm;
    using System;
    using System.Threading.Tasks;
    using PrintersModule.Events;
    using System.Diagnostics;
    using Microsoft.Win32;
    using WPF.Airprint.Docker;

    public class MainWindowViewModel : BindableBase
    {
        private const string AutoLogonKey = "AutoAdminLogon";
        private const string WinlogonRegistryLocation = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon";

        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _events;
        private readonly IDockerService _dockerService;

        public MainWindowViewModel(IEventAggregator events, IRegionManager regionManager, IDockerService dockerService)
        {
            _events = events;
            _regionManager = regionManager;
            _dockerService = dockerService;

            NavigateCommand = new DelegateCommand<string>(ExecuteNavigate);
            ExitCommand = new DelegateCommand(PerformOsShutdown);
            StartDockerCommand = new DelegateCommand(ExecuteStartDocker);

            Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(_ => 
                _events.GetEvent<StatusMessageEvent>().Publish("Main window loaded. Check here for information while using the demo..."));
        }

        private void ExecuteStartDocker()
        {
            (_dockerService as IStartupAction)?.ProcessStartupAction();
        }

        public DelegateCommand<string> NavigateCommand { get; private set; }

        public DelegateCommand ExitCommand { get; private set; }

        public DelegateCommand StartDockerCommand { get; private set; }

        private void ExecuteNavigate(string viewPath)
        {
            if (viewPath != null)
            {
                _regionManager.RequestNavigate(RegionNames.MainRegion, viewPath);
            }
        }


        private void PerformOsShutdown()
        {
            try
            {
                var bootToDesktopArgs = @"/r /t 0";

                ModifyRegistry(WinlogonRegistryLocation, AutoLogonKey, 0);

                var p = new Process
                {
                    StartInfo =
                    {
                        CreateNoWindow = true,
                        FileName = "shutdown",
                        UseShellExecute = false,
                        Arguments = bootToDesktopArgs
                    }
                };

                p.Start();
            }
            catch (Exception ex)
            {
                // TODO: logging
            }            
        }

        private void ModifyRegistry(string keyName, string valueName, object value, RegistryValueKind valueKind = RegistryValueKind.DWord)
        {
            try
            {
                var registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;

                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, registryView))
                using (var subKey = baseKey.OpenSubKey(keyName, true))
                {
                    if (subKey != null)
                    {
                        subKey.SetValue(valueName, value, valueKind);
                    }
                }
            }
            catch (Exception ex)
            {
                // logging
            }
        }        
    }
}

namespace WPF.Airprint.Demo.PrintersModule.ViewModels
{
    using System.Collections.Generic;
    using WPF.Airprint.DeviceEnumeration;
    using WPF.Airprint.Mvvm;

    public class DeviceFoundViewModel : ViewModelBase
    {
        private string _name;
        private string _ip;
        private string _host;
        private string _service;
        private readonly DeviceFound _model;

        public DeviceFoundViewModel(DeviceFound model)
        {
            Name = model.Name;
            IpAddress = model.IpAddress;
            HostName = model.HostName;
            ServiceName = model.ServiceName;
            _model = model;
        }

        public DeviceFound Model => _model;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string IpAddress
        {
            get => _ip;
            set => SetProperty(ref _ip, value);
        }

        public string HostName
        {
            get => _host;
            set => SetProperty(ref _host, value);
        }

        public string ServiceName
        {
            get => _service;
            set => SetProperty(ref _service, value);
        }

        public IEnumerable<string> Details => _model.Details;
    }
}

namespace WPF.Airprint.DeviceEnumeration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Windows.Devices.Enumeration;

    public class WindowsDeviceService : IWindowsDeviceService
    {
        private const string DnssdFilter = "System.Devices.AepService.ProtocolId:={4526e8c1-8aac-4153-9b16-55e86ada0e54} AND System.Devices.Dnssd.Domain:=\"local\" AND System.Devices.Dnssd.ServiceName:=\"_ipp._tcp\"";
        private string[] DeviceProperties = new[] {
                DeviceProperty.IpAddress,
                DeviceProperty.HostName,
                DeviceProperty.ServiceName,
                DeviceProperty.Port,
                DeviceProperty.Attributes
            };
        private DeviceWatcher _watcher;
        private List<DeviceFound> _found;

        public WindowsDeviceService()
        {
            _found = new List<DeviceFound>();
        }

        public event EventHandler<DeviceFound> DeviceFound;

        public void DiscoverDevices()
        {
            _watcher = DeviceInformation.CreateWatcher(DnssdFilter, DeviceProperties, DeviceInformationKind.AssociationEndpointService);
            _watcher.Added += _watcher_Added;   
            _watcher.Updated += (sender, args) => Debug.WriteLine("Updated: " + args.Id);
            _watcher.EnumerationCompleted += (sender, args) => Debug.WriteLine("EnumerationCompleted");
            _watcher.Start();
        }

        private void _watcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            var ipAddresses = args.Properties[DeviceProperty.IpAddress] as string[];
            var hostName = args.Properties[DeviceProperty.HostName].ToString();
            var serviceName = args.Properties[DeviceProperty.ServiceName].ToString();
            var d = new DeviceFound(args.Name, ipAddresses.FirstOrDefault(), hostName, serviceName, args);
            _found.Add(d);
            OnDeviceFound(d);
        }

        private void OnDeviceFound(DeviceFound d)
        {
            DeviceFound?.Invoke(this, d);
        }

        public void CancelDiscoverDevices()
        {
            _watcher?.Stop();
            _watcher = null;
        }
    }
}

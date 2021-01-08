namespace WPF.Airprint.DeviceEnumeration
{
    using System.Collections.Generic;
    using System.Linq;
    using Windows.Devices.Enumeration;

    public class DeviceFound
    {
        public DeviceFound(string name, string ip, string host, string  service, DeviceInformation info)
        {
            Name = name;
            IpAddress = ip;
            HostName = host;
            ServiceName = service;
            DeviceInformation = info;
        }

        public string Name { get; set; }
        public string HostName { get; set; }
        public string IpAddress { get; set; }
        public string ServiceName { get; set; }
        public DeviceInformation DeviceInformation { get; set; }

        public IEnumerable<string> Details
        {
            get
            {
                return DeviceInformation.Properties.Select(x => $"{x.Key} = {x.Value}");
            }
        }
    }
}


namespace WPF.Airprint.DeviceEnumeration
{
    using System;

    public interface IWindowsDeviceService
    {
        event EventHandler<DeviceFound> DeviceFound;

        void DiscoverDevices();

        void CancelDiscoverDevices();
    }
}

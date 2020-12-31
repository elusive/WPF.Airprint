namespace WPF.Airprint.Terminal
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class CommandStrings
    {
        public const string DeviceUriFormat = @"ipp://{0}:631/ipp/print";
        public const string GetIpAddressFormat = @"dns-sd -G v4 {0}";
    }
}

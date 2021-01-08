namespace WPF.Airprint.Terminal
{

    public class CommandStrings
    {
        public const string CommandExe = @"cmd.exe";
        public const string DndSdExe = @"dns-sd";
        public const string DockerExe = @"docker";
        public const string DeviceUriFormat = @"ipp://{0}:631/ipp/print";                       // {0} - printer host name
        public const string GetIpAddressFormat = @" -G v4 {0}";                                 // {0} - printer ip address
        public const string AddPrintQueue = @"exec {0} lpadmin -p {1} -v {2} -E -m everywhere"; // {0} - container id, {1} - queue name, {2} - printer uri
        public const string GetPrintQueueStatus = @"exec {0} lpstat -a";                        // {0} - container id
        public const string PrintFile = @"exec lp  -d {0} {1}";                                 // {0} - container id, {1} - queue name, {2} - file
    }
}

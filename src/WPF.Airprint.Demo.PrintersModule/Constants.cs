namespace WPF.Airprint.Demo.PrintersModule
{
    internal class Constants
    {
        internal const string PrinterKey = "PrinterKey";
        internal const string PrintServerExistingImageName = "cups-docker_cups"; //"cups-print-server";
        internal const string DockerImageIdPrefix = @"sha256:";

        internal class Messages
        {
            internal const string PrintServerContainerMustBeRunning = "The print server container must be running in docker.";
            internal const string NoPrinterDetailToAdd = "Unable to add printer without detail. Click the printer row.";

            internal static string GetNetworkSearchStatusMessage(int count)
            {
                return $"Network search found: {count} printers.";
            }

            internal static string GetBuildUriStatusMessage(string ip)
            {
                return $"Lookup of ip address result: {ip}";
            }

            internal static string GetPrintServerIdStatusMessage(string id)
            {
                return $"The print server docker image id is: {id}";
            }
        }
    }
}

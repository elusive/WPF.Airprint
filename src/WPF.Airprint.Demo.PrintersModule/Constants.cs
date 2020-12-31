namespace WPF.Airprint.Demo.PrintersModule
{
    internal class Constants
    {
        internal const string PrinterKey = "PrinterKey";
        internal const string PrintServerExistingImageName = "cups-print-server";

        internal class Messages
        {
            internal const string PrintServerContainerMustBeRunning = "The print server container must be running in docker.";
            internal const string NoPrinterDetailToAdd = "Unable to add printer without detail. Try Find Printer first.";

            internal static string GetNetworkSearchStatusMessage(int count)
            {
                return $"Network search found: {count} printers.";
            }
        }
    }
}

namespace WPF.Airprint
{

    internal class Constants
    {
        internal const string DockerEngineWindowsUri = "npipe://./pipe/docker_engine";
        internal const string DockerEngineLinuxUri = "unix:/var/run/docker.sock";

        internal const string DockerPrintServerImageName = "cups-print-server";
        internal const string DockerfileTarGzPath = @"Docker/CupsPrintServer/cups-docker.tar.gz";

        internal const string PrintQueueNameFormat = "ipp_printer_queue{0}";

        internal const string PrintServerVolumePath = "/files";
        internal const string PrintServerHostPath = "c:\\print";

        internal class Messages
        {
            internal const string CannotDetermineOs = "Unable to determine the host OS.";
            internal const string DockerfileNotFound = "Dockerfile not found.";
            
        }
    }
}

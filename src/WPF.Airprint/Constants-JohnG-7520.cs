namespace WPF.Airprint
{

    internal class Constants
    {
        internal const string DockerEngineWindowsUri = "npipe://./pipe/docker_engine";
        internal const string DockerEngineLinuxUri = "unix:/var/run/docker.sock";

        internal const string DockerPrintServerImageName = "cups-docker_cups"; // "cups-print-server";
        internal const string DockerPrintServerContainerName = "cups-print-server_cups1";
        internal const string DockerfileTarGzPath = @"Docker/CupsPrintServer/cups-docker.tar.gz";


        internal class Messages
        {
            internal const string CannotDetermineOs = "Unable to determine the host OS.";
            internal const string DockerfileNotFound = "Dockerfile not found.";
            
        }
    }
}

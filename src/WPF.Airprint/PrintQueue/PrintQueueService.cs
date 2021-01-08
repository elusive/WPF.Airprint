namespace WPF.Airprint.PrintQueue
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Terminal;
    using WPF.Airprint.Docker;

    public class PrintQueueService : IPrintQueueService
    {
        private readonly ITerminalService _terminal;
        private readonly IDockerService _docker;
        private readonly List<PrintQueue> _queues;
        
        public PrintQueueService(ITerminalService terminal, IDockerService docker)
        {
            _terminal = terminal;
            _docker = docker;
            _queues = new List<PrintQueue>();
        }

        public IList<PrintQueue> ExistingPrintQueues => _queues;


        public async Task<string> CheckPrintQueueStatus(string containerName, string queueName)
        {
            try
            {
                var cmd = BuildCheckPrintQueueStatusCommandArray(queueName);
                var status = await _docker.ExecuteCommandForOutput(containerName, cmd);

                return status;
            }
            catch(Exception ex)
            {
                return string.Empty;
            }
        }

        public async Task<string> CreatePrintQueue(string containerName, string printerUri)
        {
            try
            {
                var nextPrinterNum = ExistingPrintQueues.Count() + 1;
                var queueName = string.Format(Constants.PrintQueueNameFormat, nextPrinterNum);
                var command = BuildCreatePrintQueueCommandArray(queueName, printerUri);
                var success = await _docker.ExecuteCommand(containerName, command);

                if (!success)
                {
                    // TODO: figure out handling failure to create queue.
                    return string.Empty;
                }

                // add queue to printer manager list
                ExistingPrintQueues.Add(new PrintQueue(queueName, printerUri));
                return queueName;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public async Task<bool> PrintPdfFile(string file, string queueName)
        {
            // check 
            return true;
        }

        private string[] BuildCreatePrintQueueCommandArray(string queueName, string uri)
        {
            return new[]
            {
                "lpadmin",
                "-p",
                queueName,
                "-v",
                uri,
                "-E",
                "-m",
                "everywhere"
            };
        }

        private string[] BuildCheckPrintQueueStatusCommandArray(string queueName)
        {
            return new[]
            {
                "lpstat",
                "-a",
                queueName
            };
        }

        private string[] BuildPrintFileCommandArray(string file, string queueName)
        {
            return new[]
            {
                "lp",
                "-d",
                queueName,
                file
            };
        }
    }
}

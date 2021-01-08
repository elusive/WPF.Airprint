namespace WPF.Airprint.PrintQueue
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPrintQueueService
    {
        IList<PrintQueue> ExistingPrintQueues { get; }

        Task<string> CreatePrintQueue(string containerName, string printerUri);

        Task<string> CheckPrintQueueStatus(string containerName, string queueName);

        Task<bool> PrintPdfFile(string fileName, string queueName);
    }
}

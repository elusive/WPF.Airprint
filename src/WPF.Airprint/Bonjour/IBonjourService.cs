namespace WPF.Airprint.Bonjour
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBonjourService
    {
        Task<IList<PrinterFound>> FindPrinters();

        Task<PrinterFoundDetails> GetPrinterDetails(PrinterFound data);
    }
}

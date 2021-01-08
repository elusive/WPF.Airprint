namespace WPF.Airprint.Bonjour
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Obsolete("Use the windows device service instead.")]
    public interface IBonjourService
    {
        Task<IList<PrinterFound>> FindPrinters();

        Task<PrinterFoundDetails> GetPrinterDetails(PrinterFound data);
    }
}

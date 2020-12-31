namespace WPF.Airprint.Bonjour
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Bonjour;
    using WPF.Airprint.Extensions;

    public class BonjourService : IBonjourService
    {
        public const int SearchTimeMilliseconds = 6000;

        private const string Protocol = "_ipp._tcp";
        private const int Retries = 2;

        private DNSSDEventManager _eventManager = null;
        private DNSSDService _service = null;
        private DNSSDService _browser = null;
        private DNSSDService _resolver = null;
        private List<PrinterFound> _browseList = new List<PrinterFound>();
        private AutoResetEvent _browseSignal = new AutoResetEvent(initialState: false);
        private AutoResetEvent _resolveSignal = new AutoResetEvent(initialState: false);
        private PrinterFoundDetails _lastResolved;

        public BonjourService()
        {
            _eventManager = new DNSSDEventManager();
            _eventManager.ServiceFound += new _IDNSSDEvents_ServiceFoundEventHandler(ServiceFound);
            _eventManager.ServiceLost += new _IDNSSDEvents_ServiceLostEventHandler(ServiceLost);
            _eventManager.ServiceResolved += new _IDNSSDEvents_ServiceResolvedEventHandler(ServiceResolved);
            _eventManager.OperationFailed += new _IDNSSDEvents_OperationFailedEventHandler(OperationFailed);

            _service = new DNSSDService();
        }
        
        public async Task<IList<PrinterFound>> FindPrinters()
        {
            try
            {
                 _browser?.Stop();

                //
                // Start a new browse operation.
                // 
                _browser = _service.Browse(0, 0, Protocol, null, _eventManager);
                await _browseSignal.WaitOneAsync(SearchTimeMilliseconds);
                return _browseList;
            }
            catch (Exception e)
            {
                return new List<PrinterFound>();
                return new List<PrinterFound>();
            }
        }

        public async Task<PrinterFoundDetails> GetPrinterDetails(PrinterFound data)
        {
            try
            {
                _resolver?.Stop();

                _resolver = _service.Resolve(0, data.InterfaceIndex, data.Name, data.Type, data.Domain, _eventManager);
                var success = await _resolveSignal.WaitOneAsync(SearchTimeMilliseconds);
                return success ? _lastResolved : null;
            }
            catch (Exception e)
            {
                return null; // await Task<PrinterFoundDetails>.FromResult(default(PrinterFoundDetails));
            }
        }

        private void ServiceFound(DNSSDService sref,
                        DNSSDFlags flags,
                        uint ifIndex,
                        String serviceName,
                        String regType,
                        String domain)
        {
            int index = _browseList.FindIndex(x => x.Name.Equals(serviceName));

            //
            // Check to see if we've seen this service before. If the machine has multiple
            // interfaces, we could potentially get called back multiple times for the
            // same service. Implementing a simple reference counting scheme will address
            // the problem of the same service showing up more than once in the browse list.
            //
            if (index == -1)
            {
                PrinterFound data = new PrinterFound();

                data.InterfaceIndex = ifIndex;
                data.Name = serviceName;
                data.Type = regType;
                data.Domain = domain;
                data.Refs = 1;

                _browseList.Add(data);
            }
            else
            {
                PrinterFound data = (PrinterFound)_browseList[index];
                data.Refs++;
            }
        }

        private void ServiceLost
                                (
                                DNSSDService sref,
                                DNSSDFlags flags,
                                uint ifIndex,
                                String serviceName,
                                String regType,
                                String domain
                                )
        {
            // could remove from list if found
        }

        private void ServiceResolved
                                (
                                DNSSDService sref,
                                DNSSDFlags flags,
                                uint ifIndex,
                                String fullName,
                                String hostName,
                                ushort port,
                                TXTRecord txtRecord
                                )
        {
            PrinterFoundDetails data = new PrinterFoundDetails();

            data.InterfaceIndex = ifIndex;
            data.FullName = fullName;
            data.HostName = hostName;
            data.Port = port;
            data.TxtRecord = txtRecord;

            _lastResolved = data;

            //
            // Don't forget to stop the resolver. This eases the burden on the network
            //
            _resolver.Stop();
            _resolver = null;

            _resolveSignal.Set();
        }

        private void OperationFailed
                                (
                                DNSSDService sref,
                                DNSSDError error
                                )
        {
            // TODO: Add failure logging/handling.
        }
    }
}

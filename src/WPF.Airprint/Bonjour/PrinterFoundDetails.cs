namespace WPF.Airprint.Bonjour
{
    using global::Bonjour;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class PrinterFoundDetails
    {
        public uint InterfaceIndex;
        public String FullName;
        public String HostName;
        public int Port;
        public TXTRecord TxtRecord;

        public override String
            ToString()
        {
            return FullName;
        }

        public IEnumerable<string> RenderTxtRecord()
        {
            var sb = new List<string>() { "TXTRecord" };
            if (TxtRecord != null)
            {
                for (uint idx = 0; idx < TxtRecord.GetCount(); idx++)
                {
                    String key;
                    Byte[] bytes;

                    key = TxtRecord.GetKeyAtIndex(idx);
                    bytes = (Byte[])TxtRecord.GetValueAtIndex(idx);

                    if (key.Length > 0)
                    {
                        String val = "";

                        if (bytes != null)
                        {
                            val = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                        }

                        sb.Add(key + " = " + val);
                    }
                }
                
                return sb;
            }

            return Enumerable.Empty<string>();
        }
    }
}

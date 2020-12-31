namespace WPF.Airprint.Bonjour
{
    using System;

    public class PrinterFound
    {
        public uint InterfaceIndex;
        public String Name;
        public String Type;
        public String Domain;
        public int Refs;

        public override String ToString()
        {
            return Name;
        }

        public override bool Equals(object other)
        {
            bool result = false;

            if (other != null)
            {
                result = (this.Name == other.ToString());
            }

            return result;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}

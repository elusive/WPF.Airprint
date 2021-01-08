namespace WPF.Airprint.PrintQueue
{
    public class PrintQueue
    {
        public PrintQueue(string name, string uri)
        {
            Name = name;
            Uri = uri;
        }

        public string Name { get; set; }
        public string Uri { get; set; }
        public string Status { get; set; }
    }
}

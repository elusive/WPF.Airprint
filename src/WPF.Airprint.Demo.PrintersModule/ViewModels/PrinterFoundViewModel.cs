namespace WPF.Airprint.Demo.PrintersModule.ViewModels
{
    using WPF.Airprint.Bonjour;
    using WPF.Airprint.Mvvm;

    public class PrinterFoundViewModel : ViewModelBase
    {
        private string _name;
        private string _type;
        private string _domain;
        private int _refs;

        public PrinterFoundViewModel(PrinterFound model)
        {
            Name = model.Name;
            Type = model.Type;
            Domain = model.Domain;
            Refs = model.Refs;
            Model = model;
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public string Domain
        {
            get => _domain;
            set => SetProperty(ref _domain, value);
        }

        public int Refs
        {
            get => _refs;
            set => SetProperty(ref _refs, value);
        }

        public PrinterFound Model { get; private set; }
    }
}

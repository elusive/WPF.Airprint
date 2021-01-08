namespace WPF.Airprint.Demo.PrintersModule.Views
{
    using System;
    using System.Windows.Controls;
        
    /// <summary>
    /// Interaction logic for AddPrinter.xaml
    /// </summary>
    public partial class AddPrinter : UserControl
    {
        public AddPrinter()
        {
            InitializeComponent();
        }

        private void AddPrinterView_BrowseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.DefaultExt = ".pdf";
            openFileDlg.Filter = "PDF documents (.pdf)|*.pdf";
            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                AddPrinterView_PrintFileTextBox.Text = openFileDlg.FileName;
            }
        }
    }
}

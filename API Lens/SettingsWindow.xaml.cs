using System.Collections.ObjectModel;
using System.Windows;

namespace API_Lens
{
    public partial class SettingsWindow : Window
    {
        public ObservableCollection<ApiParameter> Parameters { get; set; } = new ObservableCollection<ApiParameter>();

        public SettingsWindow()
        {
            InitializeComponent();
            ParametersGrid.ItemsSource = Parameters;
        }

        private void SaveParameters_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }

    public class ApiParameter
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}

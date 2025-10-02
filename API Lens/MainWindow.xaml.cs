using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace API_Lens
{
    public partial class MainWindow : Window
    {
        private string lastRawBody = "";
        private string lastData = "";
        private HttpResponseMessage lastRawResponse = null;
        private Dictionary<string, string> lastHeaders = null;
        public ObservableCollection<ApiParameter> ApiParameters { get; set; } = new ObservableCollection<ApiParameter>();

        public MainWindow()
        {
            InitializeComponent();
            UpdatePlaceholder();
            UrlBox.GotFocus += UrlBox_GotFocus;
            UrlBox.LostFocus += UrlBox_LostFocus;
            this.MouseDown += MainWindow_MouseDown;
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(e.OriginalSource is TextBox))
                Keyboard.ClearFocus();
        }

        private void UrlBox_GotFocus(object sender, RoutedEventArgs e) => UpdatePlaceholder();
        private void UrlBox_LostFocus(object sender, RoutedEventArgs e) => UpdatePlaceholder();
        private void UrlBox_TextChanged(object sender, TextChangedEventArgs e) => UpdatePlaceholder();

        private void UpdatePlaceholder()
        {
            PlaceholderText.Visibility = string.IsNullOrEmpty(UrlBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string url = UrlBox.Text.Trim();

            if (!UrlValidator.IsUrlValid(url))
            {
                MessageBox.Show("The URL is invalid. Please enter a valid API link.", "Invalid URL", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SearchButton.IsEnabled = false;
            DisplayFormattedResponse("Sending data to API...\nPlease wait...");

            try
            {
                using HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);

                var parametersDict = ApiParameters
                    .Where(p => !string.IsNullOrWhiteSpace(p.Key))
                    .ToDictionary(p => p.Key, p => p.Value);

                string jsonBody = JsonSerializer.Serialize(parametersDict);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);

                lastRawResponse = response;

                lastHeaders = new Dictionary<string, string>();
                foreach (var header in response.Headers)
                    lastHeaders[header.Key] = string.Join(", ", header.Value);
                foreach (var header in response.Content.Headers)
                    lastHeaders[header.Key] = string.Join(", ", header.Value);

                lastRawBody = await response.Content.ReadAsStringAsync();

                string formattedResponse = await ApiResponseFormatter.GetFormattedResponseAsync(url, ApiParameters);
                DisplayFormattedResponse(formattedResponse);

                lastData = formattedResponse;
            }
            catch (Exception ex)
            {
                DisplayFormattedResponse($"Unexpected error: {ex.Message}");
            }
            finally
            {
                SearchButton.IsEnabled = true;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (lastData == null)
            {
                MessageBox.Show("No output available to save.", "Save Output", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                DefaultExt = "txt",
                FileName = "api_response.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllText(dialog.FileName, lastData, Encoding.UTF8);
                    MessageBox.Show("Output saved successfully!", "Save Output", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to save file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void DisplayFormattedResponse(string response)
        {
            ResultBox.Document.Blocks.Clear();
            string[] lines = response.Split(new[] { "\n" }, StringSplitOptions.None);

            foreach (var line in lines)
            {
                Run run = new Run(line);

                if (line.StartsWith("Status:"))
                    run.Foreground = Brushes.LimeGreen;
                else if (line.StartsWith("HEADERS:"))
                    run.Foreground = Brushes.Cyan;
                else if (line.StartsWith("BODY:"))
                    run.Foreground = Brushes.Orange;
                else if (line.StartsWith("PARAMETERS:"))
                    run.Foreground = Brushes.Yellow;
                else if (line.Contains(":"))
                    run.Foreground = Brushes.LightBlue;
                else
                    run.Foreground = Brushes.White;

                Paragraph paragraph = new Paragraph(run) { Margin = new Thickness(0) };
                ResultBox.Document.Blocks.Add(paragraph);
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;

            foreach (var p in ApiParameters)
                settingsWindow.Parameters.Add(new ApiParameter { Key = p.Key, Value = p.Value });

            if (settingsWindow.ShowDialog() == true)
            {
                ApiParameters.Clear();
                foreach (var p in settingsWindow.Parameters)
                    ApiParameters.Add(new ApiParameter { Key = p.Key, Value = p.Value });
            }
        }
    }
}

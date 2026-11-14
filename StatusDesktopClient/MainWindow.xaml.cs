using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StatusDesktopClient
{
    // 1. Create a class that matches the shape of the API's JSON data
    public class SystemStatusDto
    {
        // The properties here must map exactly to the JSON keys returned by the API!
        // We use attributes to handle C# PascalCase to JSON camelCase differences.
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public int Id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("serviceName")]
        public string ServiceName { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("status")]
        public string Status { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // HttpClient should ideally be instantiated once and reused throughout the app
        private static readonly HttpClient client = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // NOTE: Ensure this matches the http port in the API's launchSettings.json!
            string apiUrl = ApiUrlInput.Text;

            await GetStatus(apiUrl);

        }

        private async Task GetStatus(string apiUrl)
        {
            try
            {
                // 1. Send the GET request
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                // 2. Check if the server returned a 404, 500, etc.
                if (response.IsSuccessStatusCode)
                {
                    // 3. Read the JSON string from the body
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // 4. Convert the JSON into a List of C# Objects (Deserialization)
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    List<SystemStatusDto> statuses = JsonSerializer.Deserialize<List<SystemStatusDto>>(jsonResponse, options);

                    StatusListBox.ItemsSource = statuses;
                }
                else
                {
                    // Handle API-side errors (e.g., 404 Not Found)
                    Console.WriteLine($"API Error: Received HTTP {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle networking errors (e.g., the API isn't running, wrong port)
                string errorMessage = $"\nCRITICAL ERROR: Unable to connect to the server.Did you remember to run the API project? \n Technical Details: {ex.Message}";
                MessageBox.Show(errorMessage, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
    

using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XHair
{
    public partial class MainWindow : Window
    {
        internal List<Window> crosshairWindows = new List<Window>();
        internal Dictionary<Window, Slider> windowSliders = new Dictionary<Window, Slider>();
        private Window? settingsWindow;
        public bool useCustomFolder = false;
        public string customFolderPath = "";

        public MainWindow()
        {
            InitializeComponent();
            Closed += OnWindowClosed;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            ShowCrosshairSelection();
        }

        private void ShowCrosshairSelection()
        {
            // Check if a CrosshairSelection window is already open
            var crosshairWindow = crosshairWindows.FirstOrDefault(w => w.Content is CrosshairSelection);

            if (crosshairWindow != null)
            {
                // Bring the window to the front
                crosshairWindow.Activate();
            }
            else
            {
                // Create a new CrosshairSelection window
                crosshairWindow = new Window
                {
                    Content = new CrosshairSelection(),
                    Title = "Crosshair Selection",
                    Width = 570,
                    Height = 600
                };

                // Add the window to the list of CrosshairSelection windows
                crosshairWindows.Add(crosshairWindow);

                // Show the window
                crosshairWindow.Show();

                // Register the Closed event handler
                crosshairWindow.Closed += (s, e) => CrosshairWindow_Closed(crosshairWindow, e, crosshairWindow);
            }
        }

        private void CrosshairWindow_Closed(Window sender, EventArgs e, Window crosshairWindow)
        {
            if (windowSliders.TryGetValue(crosshairWindow, out Slider? slider) && slider != null)
            {
                sliderPanel.Children.Remove(slider);
                windowSliders.Remove(crosshairWindow);
            }
            crosshairWindows.Remove(crosshairWindow);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (settingsWindow == null)
            {
                settingsWindow = new Window
                {
                    Content = new SettingsPage(),
                    Title = "Settings",
                    Width = 400,
                    Height = 300
                };
                settingsWindow.Closed += (s, e) => settingsWindow = null;
            }
            settingsWindow.Show();
            settingsWindow.Activate();
            settingsWindow.Focus();
        }

        public void SaveSettings()
        {

        }

        private void OnWindowClosed(object? sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
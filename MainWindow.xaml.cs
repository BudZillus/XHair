using System.IO;
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
            ReadSettings();
            Closed += OnWindowCloseing;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            ShowCrosshairSelection();
        }

        private void ShowCrosshairSelection()
        {
            var crosshairWindow = crosshairWindows.FirstOrDefault(w => w.Content is CrosshairSelection);

            if (crosshairWindow != null)
            {
                crosshairWindow.Activate();
            }
            else
            {
                crosshairWindow = new Window
                {
                    Content = new CrosshairSelection(),
                    Title = "Crosshair Selection",
                    Width = 570,
                    Height = 600
                };

                if (useCustomFolder)
                {
                    string folderPath = customFolderPath;
                    string[] fileEntries = Directory.GetFiles(folderPath, "*.png");

                    CrosshairSelection crosshairSelection = (CrosshairSelection)crosshairWindow.Content;
                    foreach (string fileName in fileEntries)
                    {
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.UriSource = new Uri(fileName, UriKind.Absolute);
                        image.EndInit();
                        crosshairSelection.ImagePaths.Add(image);
                    }
                }
                crosshairWindows.Add(crosshairWindow);
                crosshairWindow.Show();
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

        private void ReadSettings()
        {
            string settingsFilePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings.ini");
            if (File.Exists(settingsFilePath))
            {
                string[] lines = File.ReadAllLines(settingsFilePath);
                foreach (string line in lines)
                {
                    if (line.StartsWith("useCustomFolder: "))
                    {
                        useCustomFolder = bool.Parse(line.Substring("useCustomFolder: ".Length));
                    }
                    else if (line.StartsWith("customFolderPath: "))
                    {
                        customFolderPath = line.Substring("customFolderPath: ".Length);
                    }
                }
            }
        }

        private void OnWindowCloseing(object? sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
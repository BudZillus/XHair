using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace XHair
{
    public partial class CrosshairSelection : Page
    {
        public ObservableCollection<BitmapImage> ImagePaths { get; set; }
        private List<CrosshairWindow> _openWindows;

        public CrosshairSelection()
        {
            InitializeComponent();
            ImagePaths = new ObservableCollection<BitmapImage>();
            _openWindows = new List<CrosshairWindow>();
            DataContext = this;
            LoadImages();
        }

        private void LoadImages()
        {
            var crosshairPath = "pack://application:,,,/XHair;component/Images/Crosshair/";
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetName().Name + ".g.resources";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (var reader = new System.Resources.ResourceReader(stream))
                    {
                        foreach (System.Collections.DictionaryEntry entry in reader)
                        {
                            string resourceKey = (string)entry.Key;
                            if (resourceKey.StartsWith("images/crosshair/") && resourceKey.EndsWith(".png"))
                            {
                                var imageUri = new Uri(crosshairPath + resourceKey.Substring("images/crosshair/".Length), UriKind.Absolute);
                                var image = new BitmapImage();
                                image.BeginInit();
                                image.UriSource = imageUri;
                                image.EndInit();
                                ImagePaths.Add(image);
                            }
                        }
                    }
                }
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image image && image.Source is BitmapImage bitmapImage)
            {
                var existingWindow = _openWindows.FirstOrDefault(w => w.DataContext == bitmapImage);
                if (existingWindow != null)
                {
                    existingWindow.Close();
                    _openWindows.Remove(existingWindow);
                }
                else
                {
                    var newWindow = new CrosshairWindow
                    {
                        DataContext = bitmapImage
                    };
                    newWindow.Closed += (s, args) => CrosshairWindow_Closed(newWindow);
                    _openWindows.Add(newWindow);
                    newWindow.Show();

                    // Slider und Label erstellen und hinzufügen
                    string sliderName = $"Slider_{bitmapImage.UriSource.Segments.Last()}";
                    sliderName = sliderName.Replace(".", "_").Replace("-", "_").Replace("%20", "_").Replace(" ", "_");
                    sliderName = new string(sliderName.Where(char.IsLetterOrDigit).ToArray());

                    Slider imageSizeSlider = new Slider
                    {
                        Minimum = 30,
                        Maximum = 400,
                        Value = 200,
                        Width = 200,
                        Margin = new Thickness(0, 10, 0, 0),
                        Name = sliderName,
                        IsSnapToTickEnabled = true,
                        TickFrequency = 0.01
                    };
                    imageSizeSlider.ValueChanged += (s, e) => ImageSizeSlider_ValueChanged(s, e, newWindow);

                    TextBox imageSizeTextBox = new TextBox
                    {
                        Width = 50,
                        Margin = new Thickness(5, 0, 0, 0),
                        Text = imageSizeSlider.Value.ToString("F2")
                    };
                    imageSizeTextBox.TextChanged += (s, e) =>
                    {
                        if (double.TryParse(imageSizeTextBox.Text, out double newValue))
                        {
                            imageSizeSlider.Value = newValue;
                        }
                    };
                    imageSizeSlider.ValueChanged += (s, e) =>
                    {
                        imageSizeTextBox.Text = e.NewValue.ToString("F2");
                    };

                    Slider rotationSlider = new Slider
                    {
                        Minimum = 0,
                        Maximum = 360,
                        Value = 0,
                        Width = 200,
                        Margin = new Thickness(0, 10, 0, 0),
                        Name = $"Rotation_{sliderName}",
                        IsSnapToTickEnabled = true,
                        TickFrequency = 0.01
                    };
                    rotationSlider.ValueChanged += (s, e) => RotateImage(newWindow, e.NewValue);

                    TextBox rotationTextBox = new TextBox
                    {
                        Width = 50,
                        Margin = new Thickness(5, 0, 0, 0),
                        Text = rotationSlider.Value.ToString("F2")
                    };
                    rotationTextBox.TextChanged += (s, e) =>
                    {
                        if (double.TryParse(rotationTextBox.Text, out double newValue))
                        {
                            rotationSlider.Value = newValue;
                        }
                    };
                    rotationSlider.ValueChanged += (s, e) =>
                    {
                        rotationTextBox.Text = e.NewValue.ToString("F2");
                    };

                    Label sliderLabel = new Label
                    {
                        Content = bitmapImage.UriSource.Segments.Last(),
                        Margin = new Thickness(0, 10, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Center
                    };

                    Label sizeLabel = new Label
                    {
                        Content = "Größe",
                        Margin = new Thickness(0, 5, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Center
                    };

                    Label rotationLabel = new Label
                    {
                        Content = "Rotation",
                        Margin = new Thickness(0, 5, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Center
                    };

                    StackPanel sizePanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    sizePanel.Children.Add(imageSizeSlider);
                    sizePanel.Children.Add(imageSizeTextBox);

                    StackPanel rotationPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    rotationPanel.Children.Add(rotationSlider);
                    rotationPanel.Children.Add(rotationTextBox);

                    StackPanel sliderPanel = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Margin = new Thickness(0, 10, 0, 0)
                    };
                    sliderPanel.Children.Add(sliderLabel);
                    sliderPanel.Children.Add(sizeLabel);
                    sliderPanel.Children.Add(sizePanel);
                    sliderPanel.Children.Add(rotationLabel);
                    sliderPanel.Children.Add(rotationPanel);

                    Border sliderBorder = new Border
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        Margin = new Thickness(5),
                        Padding = new Thickness(5),
                        Child = sliderPanel
                    };

                    ((MainWindow)Application.Current.MainWindow).sliderPanel.Children.Add(sliderBorder);
                    ((MainWindow)Application.Current.MainWindow).windowSliders[newWindow] = imageSizeSlider;
                    newWindow.Tag = sliderBorder;
                }
            }
        }

        private void ImageSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e, CrosshairWindow crosshairWindow)
        {
            if (crosshairWindow.Content is Grid grid)
            {
                Image? crosshairImage = grid.Children.OfType<Image>().FirstOrDefault();
                if (crosshairImage != null)
                {
                    crosshairImage.Width = e.NewValue;
                    crosshairImage.Height = e.NewValue;
                }
            }
        }

        private void RotateImage(CrosshairWindow crosshairWindow, double angle)
        {
            if (crosshairWindow.Content is Grid grid)
            {
                Image? crosshairImage = grid.Children.OfType<Image>().FirstOrDefault();
                if (crosshairImage != null)
                {
                    RotateTransform rotateTransform = new RotateTransform(angle);
                    crosshairImage.RenderTransform = rotateTransform;
                    crosshairImage.RenderTransformOrigin = new Point(0.5, 0.5);
                }
            }
        }

        private void CrosshairWindow_Closed(CrosshairWindow crosshairWindow)
        {
            var mainWindow = (MainWindow?)Application.Current.MainWindow;
            if (mainWindow != null && mainWindow.windowSliders.TryGetValue(crosshairWindow, out Slider? slider))
            {
                // Entferne den Slider und das zugehörige Label
                var sliderPanel = mainWindow.sliderPanel;
                var sliderBorder = (Border?)crosshairWindow.Tag;
                if (sliderBorder != null)
                {
                    sliderPanel.Children.Remove(sliderBorder);
                }

                mainWindow.windowSliders.Remove(crosshairWindow);
            }                      

            _openWindows.Remove(crosshairWindow);
        }   
    }
}

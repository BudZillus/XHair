using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;

namespace XHair
{
    public partial class CrosshairSelection : Page
    {
        public ObservableCollection<BitmapImage> ImagePaths { get; set; }

        public CrosshairSelection()
        {
            InitializeComponent();
            ImagePaths = new ObservableCollection<BitmapImage>();
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
                var crosshairWindow = CrosshairWindow.Instance;
                crosshairWindow.AddItem(bitmapImage);
                crosshairWindow.Show();

                string sliderName = $"Slider_{bitmapImage.UriSource.Segments.Last()}";
                sliderName = sliderName.Replace(".", "_").Replace("-", "_").Replace("%20", "_").Replace(" ", "_");
                sliderName = new string(sliderName.Where(char.IsLetterOrDigit).ToArray());

                Slider imageSizeSlider = new Slider
                {
                    Minimum = 0,
                    Maximum = 500,
                    Value = 200,
                    Width = 200,
                    Margin = new Thickness(0, 10, 0, 0),
                    Name = sliderName,
                    IsSnapToTickEnabled = true,
                    TickFrequency = 0.01
                };
                imageSizeSlider.ValueChanged += (s, e) => ImageSizeSlider_ValueChanged(s, e, bitmapImage);

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
                rotationSlider.ValueChanged += (s, e) => RotateImage(bitmapImage, e.NewValue);

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

                Button removeButton = new Button
                {
                    Content = "X",
                    Foreground = Brushes.White,
                    Background = Brushes.Red,
                    Width = 20,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 0, 5, 0)
                };
                removeButton.Click += (s, e) => RemoveItem(bitmapImage);

                Button moveUpButton = new Button
                {
                    Content = "↑",
                    Foreground = Brushes.White,
                    Background = Brushes.Gray,
                    Width = 20,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 25, 5, 0)
                };
                moveUpButton.Click += (s, e) => MoveItemUp(bitmapImage);

                Button moveDownButton = new Button
                {
                    Content = "↓",
                    Foreground = Brushes.White,
                    Background = Brushes.Gray,
                    Width = 20,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 50, 5, 0)
                };
                moveDownButton.Click += (s, e) => MoveItemDown(bitmapImage);

                Grid labelGrid = new Grid();
                labelGrid.ColumnDefinitions.Add(new ColumnDefinition());
                labelGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                labelGrid.Children.Add(sliderLabel);
                Grid.SetColumn(sliderLabel, 0);
                labelGrid.Children.Add(removeButton);
                Grid.SetColumn(removeButton, 1);
                labelGrid.Children.Add(moveUpButton);
                Grid.SetColumn(moveUpButton, 1);
                labelGrid.Children.Add(moveDownButton);
                Grid.SetColumn(moveDownButton, 1);

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
                sliderPanel.Children.Add(labelGrid);
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
                    Child = sliderPanel,
                    Tag = bitmapImage
                };

                ((MainWindow)Application.Current.MainWindow).sliderPanel.Children.Add(sliderBorder);
                ((MainWindow)Application.Current.MainWindow).windowSliders[crosshairWindow] = imageSizeSlider;
                crosshairWindow.Tag = sliderBorder;
            }
        }

        private void RemoveItem(BitmapImage bitmapImage)
        {
            var crosshairWindow = CrosshairWindow.Instance;
            crosshairWindow.RemoveItem(bitmapImage);
        }

        private void MoveItemUp(BitmapImage bitmapImage)
        {
            var crosshairWindow = CrosshairWindow.Instance;
            crosshairWindow.MoveItemUp(bitmapImage);
        }

        private void MoveItemDown(BitmapImage bitmapImage)
        {
            var crosshairWindow = CrosshairWindow.Instance;
            crosshairWindow.MoveItemDown(bitmapImage);
        }

        private void ImageSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e, BitmapImage bitmapImage)
        {
            var crosshairWindow = CrosshairWindow.Instance;
            if (crosshairWindow.Content is Grid grid)
            {
                Image? crosshairImage = grid.Children.OfType<Image>().FirstOrDefault(img => img.Source == bitmapImage);
                if (crosshairImage != null)
                {
                    crosshairImage.Width = e.NewValue;
                    crosshairImage.Height = e.NewValue;
                }
            }
        }

        private void RotateImage(BitmapImage bitmapImage, double angle)
        {
            var crosshairWindow = CrosshairWindow.Instance;
            if (crosshairWindow.Content is Grid grid)
            {
                Image? crosshairImage = grid.Children.OfType<Image>().FirstOrDefault(img => img.Source == bitmapImage);
                if (crosshairImage != null)
                {
                    RotateTransform rotateTransform = new RotateTransform(angle);
                    crosshairImage.RenderTransform = rotateTransform;
                    crosshairImage.RenderTransformOrigin = new Point(0.5, 0.5);
                }
            }
        }
    }
}

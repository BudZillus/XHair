using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;

namespace XHair;
public partial class CrosshairWindow : Window
{
    private static CrosshairWindow? crosshairwindow;

    public static CrosshairWindow Instance
    {
        get
        {
            if (crosshairwindow == null)
            {
                crosshairwindow = new CrosshairWindow();
            }
            return crosshairwindow;
        }
    }

    private CrosshairWindow()
    {
        InitializeComponent();
        Topmost = true;
    }

    public void AddItem(ImageSource imageSource)
    {
        var image = new Image
        {
            Source = imageSource,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Width = 100,
            Height = 100
        };
        MainGrid.Children.Add(image);
    }

    public void RemoveItem(ImageSource imageSource)
    {
        var image = MainGrid.Children.OfType<Image>().FirstOrDefault(img => img.Source == imageSource);
        if (image != null)
        {
            MainGrid.Children.Remove(image);
        }

        var mainWindow = (MainWindow)Application.Current.MainWindow;
        var sliderBorder = mainWindow.sliderPanel.Children.OfType<Border>().FirstOrDefault(b => b.Tag == imageSource);
        if (sliderBorder != null)
        {
            mainWindow.sliderPanel.Children.Remove(sliderBorder);
        }
    }

    public void MoveItemUp(ImageSource imageSource)
    {
        var image = MainGrid.Children.OfType<Image>().FirstOrDefault(img => img.Source == imageSource);
        if (image != null)
        {
            int index = MainGrid.Children.IndexOf(image);
            if (index > 0)
            {
                MainGrid.Children.RemoveAt(index);
                MainGrid.Children.Insert(index - 1, image);

                var mainWindow = (MainWindow)Application.Current.MainWindow;
                var sliderBorder = mainWindow.sliderPanel.Children.OfType<Border>().FirstOrDefault(b => b.Tag == imageSource);
                if (sliderBorder != null)
                {
                    int sliderIndex = mainWindow.sliderPanel.Children.IndexOf(sliderBorder);
                    if (sliderIndex > 0)
                    {
                        mainWindow.sliderPanel.Children.RemoveAt(sliderIndex);
                        mainWindow.sliderPanel.Children.Insert(sliderIndex - 1, sliderBorder);
                    }
                }
            }
        }
    }

    public void MoveItemDown(ImageSource imageSource)
    {
        var image = MainGrid.Children.OfType<Image>().FirstOrDefault(img => img.Source == imageSource);
        if (image != null)
        {
            int index = MainGrid.Children.IndexOf(image);
            if (index < MainGrid.Children.Count - 1)
            {
                MainGrid.Children.RemoveAt(index);
                MainGrid.Children.Insert(index + 1, image);

                var mainWindow = (MainWindow)Application.Current.MainWindow;
                var sliderBorder = mainWindow.sliderPanel.Children.OfType<Border>().FirstOrDefault(b => b.Tag == imageSource);
                if (sliderBorder != null)
                {
                    int sliderIndex = mainWindow.sliderPanel.Children.IndexOf(sliderBorder);
                    if (sliderIndex < mainWindow.sliderPanel.Children.Count - 1)
                    {
                        mainWindow.sliderPanel.Children.RemoveAt(sliderIndex);
                        mainWindow.sliderPanel.Children.Insert(sliderIndex + 1, sliderBorder);
                    }
                }
            }
        }
    }
}



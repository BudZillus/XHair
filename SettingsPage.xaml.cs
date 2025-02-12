using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace XHair
{
    public partial class SettingsPage : Page
    {
        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public SettingsPage()
        {
            InitializeComponent();
        }

        private void UserPath_Click(object sender, RoutedEventArgs e)
        {            
            var folderPicker = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select Folder"
            };
            
            if (folderPicker.ShowDialog() == CommonFileDialogResult.Ok)
            {                
                mainWindow.useCustomFolder = true;
                mainWindow.customFolderPath = folderPicker.FileName; 
                mainWindow.SaveSettings();

                // DEBUG
                MessageBox.Show("Path set to: " + mainWindow.customFolderPath,
                                "Done!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}

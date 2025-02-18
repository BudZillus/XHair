using System;
using System.IO;
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
            if (mainWindow.customFolderPath != null)
            {
                userPathLabel.Content = mainWindow.customFolderPath;
            }        
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
                SaveSettings();
                MessageBox.Show("Custom folder selected: " + folderPicker.FileName);
            }
        }

        private void SaveSettings()
        {
            string settingsFilePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings.ini");
            try
            {
                using (StreamWriter writer = new StreamWriter(settingsFilePath, false))
                {
                    writer.WriteLine("customFolderPath: " + mainWindow.customFolderPath);
                }
                userPathLabel.Content = mainWindow.customFolderPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving settings: " + ex.Message);
            }
        }

        private void SetHideKey_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("SetHideKey_Click");
        }
    }
}

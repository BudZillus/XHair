using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace XHair
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void UserPath_Click(object sender, RoutedEventArgs e)
        {
            // prompt user for folder path selection
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ValidateNames = false;
            openFileDialog.CheckFileExists = false;
            openFileDialog.CheckPathExists = true;
            openFileDialog.FileName = "Ordner auswählen";

            if (openFileDialog.ShowDialog() == true)
            {
                // set the text of the textbox to the selected folder path
                ((Button)sender).Content = openFileDialog.FileName;
            }
        }
    }
}
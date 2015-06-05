using Hearthstone_Deck_Tracker;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using StatsConverter.Properties;
using Microsoft.Win32;
using System.Windows.Forms;

namespace AndBurn.HDT.Plugins.StatsConverter.Controls
{
    public partial class PluginSettings : CustomDialog
    {
        private string defaultPath;

        public PluginSettings()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            bool timestamp = Settings.Default.UseExportFileTimestamp;
            CheckBox_Timestamp.IsChecked = timestamp;

            string filename = Settings.Default.ExportFileName;
            TextBox_Prefix.Text = filename;

            defaultPath = Settings.Default.DefaultExportPath;
        }

        private void SaveSettings()
        {
            Settings.Default.UseExportFileTimestamp = CheckBox_Timestamp.IsChecked == true ? true : false;
            Settings.Default.ExportFileName = TextBox_Prefix.Text;
            Settings.Default.DefaultExportPath = defaultPath;
            Settings.Default.Save();
        }

        private void BtnDefaultDirectory_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fdlg = new FolderBrowserDialog();
            fdlg.RootFolder = Environment.SpecialFolder.MyComputer;
            DialogResult result = fdlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                defaultPath = fdlg.SelectedPath;
            }
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            Helper.MainWindow.HideMetroDialogAsync(this);
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Helper.MainWindow.HideMetroDialogAsync(this);
        }

    }
}

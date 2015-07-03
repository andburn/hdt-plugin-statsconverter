using Hearthstone_Deck_Tracker;
using System;
using System.Windows;

using StatsConverter.Properties;
using System.Windows.Forms;

namespace AndBurn.HDT.Plugins.StatsConverter.Controls
{
	public partial class PluginSettings : System.Windows.Controls.UserControl
    {

        public PluginSettings()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            CheckBox_Timestamp.IsChecked = Settings.Default.UseExportFileTimestamp;
            TextBox_Prefix.Text = Settings.Default.ExportFileName;
        }

        private void BtnDefaultDirectory_Click(object sender, RoutedEventArgs e)
        {
			// display the folder chooser dialog
            FolderBrowserDialog fdlg = new FolderBrowserDialog();
			fdlg.ShowNewFolderButton = true;
            fdlg.RootFolder = Environment.SpecialFolder.MyComputer;
            DialogResult result = fdlg.ShowDialog();

			// if a selection was made save settings
            if (result == DialogResult.OK)
            {
				Settings.Default.DefaultExportPath = fdlg.SelectedPath;
				Settings.Default.Save();
            }
        }

		private void CheckBox_Timestamp_Checked(object sender, RoutedEventArgs e)
		{
			Settings.Default.UseExportFileTimestamp = true;
			Settings.Default.Save();
		}

		private void CheckBox_Timestamp_Unchecked(object sender, RoutedEventArgs e)
		{
			Settings.Default.UseExportFileTimestamp = false;
			Settings.Default.Save();
		}

		private void TextBox_Prefix_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			Settings.Default.ExportFileName = TextBox_Prefix.Text;
			Settings.Default.Save();
		}

    }
}

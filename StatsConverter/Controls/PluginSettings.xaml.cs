using System;
using System.Windows;
using System.Windows.Forms;
using StatsConverter.Properties;

namespace HDT.Plugins.StatsConverter.Controls
{
	public partial class PluginSettings : System.Windows.Controls.UserControl
	{
		private bool _initialized;

		public PluginSettings()
		{
			InitializeComponent();
			LoadSettings();
			_initialized = true;
		}

		private void LoadSettings()
		{
			CheckBox_Timestamp.IsChecked = Settings.Default.UseExportFileTimestamp;
			TextBox_Prefix.Text = Settings.Default.ExportFileName;
			if (Settings.Default.DevMode)
			{
				Dev_FlushLines.Text = Settings.Default.FlushLines.ToString();
				Dev_LogReadFreq.Text = Settings.Default.ReadFreq.ToString();
				DevPanel.Visibility = System.Windows.Visibility.Visible;
			}
		}

		private void BtnDefaultDirectory_Click(object sender, RoutedEventArgs e)
		{
			if (!_initialized)
				return;

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
			if (!_initialized)
				return;
			Settings.Default.UseExportFileTimestamp = true;
			Settings.Default.Save();
		}

		private void CheckBox_Timestamp_Unchecked(object sender, RoutedEventArgs e)
		{
			if (!_initialized)
				return;
			Settings.Default.UseExportFileTimestamp = false;
			Settings.Default.Save();
		}

		private void TextBox_Prefix_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (!_initialized)
				return;
			Settings.Default.ExportFileName = TextBox_Prefix.Text;
			Settings.Default.Save();
		}

		private void Dev_FlushLines_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (!_initialized)
				return;
			var lines = 0;
			bool result = int.TryParse(Dev_FlushLines.Text, out lines);
			if (result)
			{
				Settings.Default.FlushLines = lines;
				Settings.Default.Save();
			}
		}

		private void Dev_LogReadFreq_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (!_initialized)
				return;
			var freq = 0;
			bool result = int.TryParse(Dev_LogReadFreq.Text, out freq);
			if (result)
			{
				Settings.Default.ReadFreq = freq;
				Settings.Default.Save();
			}
		}
	}
}
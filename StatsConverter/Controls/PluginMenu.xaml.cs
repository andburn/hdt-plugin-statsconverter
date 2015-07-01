using Hearthstone_Deck_Tracker;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace AndBurn.HDT.Plugins.StatsConverter.Controls
{
    public partial class PluginMenu : MenuItem
    {
        public PluginMenu()
        {
            InitializeComponent();
        }

		// TODO: can probably do this better, instead of creating new objects each time

        private void MenuItem_Export_Click(object sender, RoutedEventArgs e)
        {
            Helper.MainWindow.ShowMetroDialogAsync(new ExportDialog());
        }

		private void MenuItem_Import_Click(object sender, RoutedEventArgs e)
		{			
			Helper.MainWindow.ShowMetroDialogAsync(new ImportDialog());
		}

    }
}

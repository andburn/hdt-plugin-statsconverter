using System;
using System.Windows;
using Hearthstone_Deck_Tracker;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace AndBurn.HDT.Plugins.StatsConverter.Controls
{
    public partial class ImportDialog : CustomDialog
    {
		public ImportDialog()
        {
            InitializeComponent();
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Hearthstone_Deck_Tracker.API.Core.MainWindow.HideMetroDialogAsync(this);
        }

        private async void BtnImport_OnClick(object sender, RoutedEventArgs e)
        {            
            var importer = new HsLogImporter();

            // set up and open file dialog
			OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = "." + importer.FileExtension;
            dlg.Filter = importer.Name + " Files | *." + importer.FileExtension;
            Nullable<bool> result = dlg.ShowDialog();
            // close export dialog
			await Hearthstone_Deck_Tracker.API.Core.MainWindow.HideMetroDialogAsync(this);

            // Process save file dialog box results
            if (result == true)
            {
                // Import from file
                string filename = dlg.FileName;
				Converter.Import(importer, filename);            
            }
            
        }
    }
}

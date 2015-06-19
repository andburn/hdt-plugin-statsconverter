using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StatsConverter.Properties;
using AndBurn.HDT.Plugins.StatsConverter.CSV;
using System.ComponentModel;
using AndBurn.HDT.Plugins.StatsConverter.HearthstoneTracker;

namespace AndBurn.HDT.Plugins.StatsConverter.Controls
{
    public partial class ImportDialog : CustomDialog
    {
        private bool _initialized;

        public ImportDialog()
        {
            InitializeComponent();
            _initialized = true;
        }

  
        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Helper.MainWindow.HideMetroDialogAsync(this);
        }

        private async void BtnImport_OnClick(object sender, RoutedEventArgs e)
        {
			Logger.WriteLine("Import clicked","plugin");
			var importer = new HearthstoneTrackerImporter();
			await Helper.MainWindow.HideMetroDialogAsync(this);
			try
			{
				await Converter.Import(importer, importer.DefaultLocation);
			}
			catch (Exception ex)
			{
				Logger.WriteLine(ex.Message, "errror");
			}
			finally
			{
				Logger.WriteLine("finally", "plugin");
			}
			

			Logger.WriteLine("after import call", "plugin");
			//await Helper.MainWindow.HideMetroDialogAsync(this);
        }
    }
}

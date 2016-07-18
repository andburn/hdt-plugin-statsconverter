using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HDT.Plugins.StatsConverter.Export;
using HDT.Plugins.StatsConverter.Utilities;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Utility.Logging;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using StatsConverter.Properties;

namespace HDT.Plugins.StatsConverter.Controls
{
	public partial class ExportDialog : CustomDialog
	{
		private bool _initialized;
		private List<Deck> decks;
		private List<string> deckNames;

		public ExportDialog()
		{
			InitializeComponent();
			LoadDecks();

			ComboBoxDeckPicker.ItemsSource = deckNames;
			ComboBoxMode.ItemsSource = Enum.GetValues(typeof(GameMode));
			ComboBoxRegion.ItemsSource = Enum.GetValues(typeof(StatsRegion));
			ComboBoxTime.ItemsSource = Enum.GetValues(typeof(TimeFrame));

			ComboBoxDeckPicker.SelectedIndex = 0;
			ComboBoxMode.SelectedItem = GameMode.All;
			ComboBoxRegion.SelectedItem = StatsRegion.All;
			ComboBoxTime.SelectedItem = TimeFrame.Today;

			_initialized = true;
		}

		private void LoadDecks()
		{
			Facade.LoadDeckList();
			decks = DeckList.Instance.Decks.ToList();
			deckNames = decks.Select(d => d.Name).OrderBy(x => x).ToList();
			deckNames.Insert(0, "All");
		}

		private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
		{
			Hearthstone_Deck_Tracker.API.Core.MainWindow.HideMetroDialogAsync(this);
		}

		private async void BtnExport_OnClick(object sender, RoutedEventArgs e)
		{
			// Get input field values
			var deckIndex = ComboBoxDeckPicker.SelectedIndex;
			var deck = deckIndex <= 0 ? (Guid?)null : decks.ElementAt(deckIndex - 1).DeckId;
			var region = (StatsRegion)ComboBoxRegion.SelectedItem;
			var time = (TimeFrame)ComboBoxTime.SelectedItem;
			var mode = (GameMode)ComboBoxMode.SelectedItem;

			// create exporting objects
			var filter = new StatsFilter(deck, region, mode, time);
			var exporter = new CSVExporter();
			// filter stats first
			var stats = Converter.Filter(filter);

			// exit on empty stats list
			if (stats.Count <= 0)
			{
				Log.Info("No stats found to export");
				await Hearthstone_Deck_Tracker.API.Core.MainWindow.HideMetroDialogAsync(this);
				return;
			}

			// set up and open save dialog
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.FileName = GetDefaultFileName();
			dlg.DefaultExt = "." + exporter.FileExtension;
			dlg.InitialDirectory = Settings.Default.DefaultExportPath;
			dlg.Filter = exporter.Name + " Files | *." + exporter.FileExtension;
			Nullable<bool> result = dlg.ShowDialog();
			// close export dialog
			await Hearthstone_Deck_Tracker.API.Core.MainWindow.HideMetroDialogAsync(this);

			// process save file dialog box results
			if (result == true)
			{
				var filename = dlg.FileName;
				// export and save document
				await Converter.Export(exporter, filename, stats);
				// export arena extras
				if (mode == GameMode.Arena && CheckBoxArenaExtras.IsChecked == true)
				{
					await Task.Run(() => Converter.ArenaExtras(filename, stats, deck, decks));
				}
			}
		}

		private string GetDefaultFileName()
		{
			var name = Settings.Default.ExportFileName;
			if (Settings.Default.UseExportFileTimestamp)
			{
				name += "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
			}
			return name;
		}

		private void ComboBoxMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!_initialized)
				return;

			// disable deck picker, while it is repopulated
			ComboBoxDeckPicker.IsHitTestVisible = false;
			ComboBoxDeckPicker.Focusable = false;
			// hide arena extras box
			ArenaExtras.Visibility = Visibility.Collapsed;

			var mode = (GameMode)ComboBoxMode.SelectedValue;
			var deckList = new List<Deck>();

			switch (mode)
			{
				case GameMode.All:
					UpdateDeckList(false);
					CheckBoxArenaExtras.IsChecked = false;
					break;

				case GameMode.Arena:
					UpdateDeckList(true, true);
					ArenaExtras.Visibility = Visibility.Visible;
					break;

				default: // Otherwise Constructed
					UpdateDeckList(true, false);
					CheckBoxArenaExtras.IsChecked = false;
					break;
			}

			// renable deck picker
			ComboBoxDeckPicker.IsHitTestVisible = true;
			ComboBoxDeckPicker.Focusable = true;
		}

		private void UpdateDeckList(bool filter, bool arena = false)
		{
			LoadDecks();
			if (filter)
			{
				decks = decks.Where(d => d.IsArenaDeck == arena).ToList();
				deckNames = decks.Select(d => d.Name).OrderBy(x => x).ToList();
				deckNames.Insert(0, "All");
			}
			ComboBoxDeckPicker.ItemsSource = deckNames;
			ComboBoxDeckPicker.SelectedIndex = 0;
		}
	}
}
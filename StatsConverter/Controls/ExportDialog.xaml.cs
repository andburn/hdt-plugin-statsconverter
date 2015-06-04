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

namespace AndBurn.HDT.Plugins.StatsConverter.Controls
{
    public partial class ExportDialog : CustomDialog
    {
        private bool _initialized;
        private List<Deck> decks;
        private List<String> deckNames;

        public ExportDialog()
        {
            InitializeComponent();
            LoadDecks();
            ComboBoxDeckPicker.ItemsSource = deckNames;
            ComboBoxTime.ItemsSource = Enum.GetValues(typeof(TimeFrame));
            _initialized = true;
        }

        private void LoadDecks()
        {
            DeckList.Load();
            decks = DeckList.Instance.Decks.ToList<Deck>();
            deckNames = decks.Select<Deck, String>(d => d.Name).ToList<String>();
            deckNames.Insert(0, "All");
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Helper.MainWindow.HideMetroDialogAsync(this);
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
            // set up and open save dialog
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "hdt-stats-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV Files | *.csv";
            Nullable<bool> result = dlg.ShowDialog();
            // close export dialog
            await Helper.MainWindow.HideMetroDialogAsync(this);
            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;                
                await Converter.Export(exporter, filter, filename);                
            }
            
        }

        private void ComboBoxMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_initialized)
                return;

            // disable deck picker, while it is repopulated
            ComboBoxDeckPicker.IsHitTestVisible = false;
            ComboBoxDeckPicker.Focusable = false;

            var mode = (GameMode)ComboBoxMode.SelectedValue;
            var deckList = new List<Deck>();

            switch (mode)
            {
                case GameMode.All:
                    UpdateDeckList(false);
                    break;
                case GameMode.Arena:
                    UpdateDeckList(true, true);
                    break;
                default: // Otherwise Constructed
                    UpdateDeckList(true, false);
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
                decks = decks.Where<Deck>(d => d.IsArenaDeck == arena).ToList<Deck>();
                deckNames = decks.Select<Deck, String>(d => d.Name).ToList<String>();
                deckNames.Insert(0, "All");
            }            
            ComboBoxDeckPicker.ItemsSource = deckNames;
            ComboBoxDeckPicker.SelectedIndex = 0;
        }
    }
}

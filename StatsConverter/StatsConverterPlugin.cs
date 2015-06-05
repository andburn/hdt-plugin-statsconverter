using System;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker.Plugins;
using AndBurn.HDT.Plugins.StatsConverter.Controls;
using Hearthstone_Deck_Tracker;
using MahApps.Metro.Controls.Dialogs;

namespace AndBurn.HDT.Plugins.StatsConverter
{
    public class StatsConverterPlugin : IPlugin
    {
        private MenuItem StatsMenuItem;
        private Controls.PluginSettings SettingsDialog;

        public string Name
        {
            get { return "Stats Converter"; }
        }

        public string Description
        {
            get { return "Converts game statistics to other formats. Currently exports to CSV only."; }
        }

        public string ButtonText
        {
            get { return "Settings"; }
        }

        public string Author
        {
            get { return "andburn"; }
        }

        public Version Version
        {
            get { return new Version(0, 1, 0); }
        }       
        
        public MenuItem MenuItem
        {
            get { return StatsMenuItem; }
        }

        public void OnLoad()
        {
            StatsMenuItem = new PluginMenu();
            SettingsDialog = new Controls.PluginSettings();
        }

        public void OnUnload()
        {
            SettingsDialog.RequestCloseAsync();
        }

        public void OnUpdate()
        {
        }

        public void OnButtonPress()
        {
            if (SettingsDialog != null)
                Helper.MainWindow.ShowMetroDialogAsync(SettingsDialog);
        }
        
    }
}

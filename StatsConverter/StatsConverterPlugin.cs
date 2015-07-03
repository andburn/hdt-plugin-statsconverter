using System;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker.Plugins;
using AndBurn.HDT.Plugins.StatsConverter.Controls;
using Hearthstone_Deck_Tracker;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Diagnostics;
using MahApps.Metro.Controls;

namespace AndBurn.HDT.Plugins.StatsConverter
{
    public class StatsConverterPlugin : IPlugin
    {
        private MenuItem _statsMenuItem;
        private static Flyout _settings;

        public string Name
        {
            get { return "Stats Converter"; }
        }

        public string Description
        {
            get { return "Import and export game statistics in different formats."; }
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
            get { return new Version(0, 0, 1); }
        }       
        
        public MenuItem MenuItem
        {
            get { return _statsMenuItem; }
        }

        public async void OnLoad()
        {
            _statsMenuItem = new PluginMenu();
			SetSettingsFlyout();

			var latest = await Github.CheckForUpdate("andburn", "hdt-plugin-statsconverter", Version);
			if (latest != null)
			{
				await ShowUpdateMessage(latest);
				Logger.WriteLine("Update available: " + latest.tag_name, "StatsConverter");
			}
        }

        public void OnUnload()
        {
			if (_settings != null)
				_settings.IsOpen = false;
        }

        public void OnUpdate()
        {
        }

        public void OnButtonPress()
        {
			OpenSettingsFlyout();
        }

		public static void OpenSettingsFlyout()
		{
			if (_settings != null)
				_settings.IsOpen = true;
		}

		private static void SetSettingsFlyout()
		{
			var window = Hearthstone_Deck_Tracker.Helper.MainWindow;
			var flyouts = window.Flyouts.Items;

			// TODO: how to set Panel.ZIndex
			Flyout settings = new Flyout();
			settings.Name = "PluginSettingsFlyout";
			settings.Position = Position.Left;			
			settings.Header = "Stats Converter Settings";
			settings.Content = new Controls.PluginSettings();
			flyouts.Add(settings);

			_settings = settings;
		}

		private async Task ShowUpdateMessage(Github.GithubRelease release)
		{
			var settings = new MetroDialogSettings { AffirmativeButtonText = "Get Update", NegativeButtonText = "Close" };

			var result = await Helper.MainWindow.ShowMessageAsync("Uptate Available", 
				"For Plugin: \"" + this.Name + "\"", MessageDialogStyle.AffirmativeAndNegative, settings);
			if (result == MessageDialogResult.Affirmative)
				Process.Start(release.html_url);
		}
    }
}

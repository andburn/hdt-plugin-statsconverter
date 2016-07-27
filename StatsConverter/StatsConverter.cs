using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using CsvHelper;
using GalaSoft.MvvmLight.Command;
using HDT.Plugins.Common.Controls.SlidePanels;
using HDT.Plugins.Common.Models;
using HDT.Plugins.Common.Plugin;
using HDT.Plugins.Common.Providers;
using HDT.Plugins.Common.Services;
using HDT.Plugins.StatsConverter.Export;
using HDT.Plugins.StatsConverter.Import;
using HDT.Plugins.StatsConverter.Models;
using HDT.Plugins.StatsConverter.Utilities;

namespace HDT.Plugins.StatsConverter
{
	[Name("Stats Converter")]
	[PluginVersion("0.2.1")]
	[Description("Import and export game statistics in different formats")]
	public class StatsConverter : PluginBase
	{
		private static readonly Uri UpdateUrl =
			new Uri(@"https://api.github.com/repos/andburn/hdt-plugin-statsconverter/releases");

		private static IUpdateService _updater;
		private static ILoggingService _logger;
		private static IDataRepository _data;

		private MenuItem _statsMenuItem;

		public override MenuItem MenuItem
		{
			get { return _statsMenuItem; }
		}

		public StatsConverter()
			: base()
		{
			_updater = ServiceFactory.CreateUpdateService();
			_logger = ServiceFactory.CreateLoggingService();
			_data = ServiceFactory.CreateDataRepository();
		}

		public override async void OnLoad()
		{
			PluginMenu pm = new PluginMenu("Stats Converter", "pie-chart");
			pm.Append("Settings", "cog", new RelayCommand(() => System.Console.WriteLine()));
			pm.Append("Import", new RelayCommand(() => System.Console.WriteLine()));
			pm.Append("Export", new RelayCommand(() => System.Console.WriteLine()));

			_statsMenuItem = pm.Menu;

			try
			{
				var latest = await _updater.CheckForUpdate(UpdateUrl, Version);
				if (latest.HasUpdate)
				{
					_logger.Info($"Plugin Update available ({latest.Version})");
					SlidePanelManager.Notification("Plugin Update Available",
						$"[DOWNLOAD]({latest.DownloadUrl}) EndGame v{latest.Version}",
						"download3", () => Process.Start(latest.DownloadUrl)
						).AutoClose(10);
				}
			}
			catch (Exception e)
			{
				_logger.Error(e);
			}
		}

		public override void OnUnload()
		{
			SlidePanelManager.DetachAll();
		}

		public override void OnButtonPress()
		{
			new Views.MainView().Show();
		}

		// Converter stuff (put here for now) ---------------

		public static List<GameStats> Filter(StatsFilter filter)
		{
			return filter.Apply(_data.GetAllStats());
		}

		public static async Task Export(IStatsExporter export, string filepath, List<GameStats> stats)
		{
			// TODO have loading spinner on view
			try
			{
				if (stats.Count <= 0)
					throw new Exception("No stats found");
				export.To(filepath, stats);
			}
			catch (Exception e)
			{
				_logger.Error(e);
			}
		}

		public static void Import(IStatsImporter import, string filename)
		{
			var games = import.From(filename);
			// for current import options, do nothing with result
		}

		public static void ArenaExtras(string filename, List<GameStats> stats, Guid? deck, List<Deck> decks)
		{
			List<ArenaExtra> arenaRuns = null;
			if (deck == null)
				arenaRuns = decks
					.Where(x => x.IsArena && stats.Any(s => s.DeckId == x.DeckId))
					.Select(x => new ArenaExtra(x, stats))
					.OrderByDescending(x => x.LastPlayed).ToList();
			else
				arenaRuns = decks
					.Where(x => x.DeckId == deck && x.IsArena)
					.Select(x => new ArenaExtra(x, stats))
					.OrderByDescending(x => x.LastPlayed).ToList();

			var fn = filename.Replace(".csv", "-extra.csv");
			using (var writer = new StreamWriter(fn))
			using (var csv = new CsvWriter(writer))
			{
				csv.Configuration.RegisterClassMap<ArenaExtraMap>();
				csv.WriteHeader<ArenaExtra>();
				csv.WriteRecords(arenaRuns);
			}
		}
	}
}
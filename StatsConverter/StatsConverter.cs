using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using CsvHelper;
using GalaSoft.MvvmLight.Command;
using HDT.Plugins.Common.Controls.SlidePanels;
using HDT.Plugins.Common.Models;
using HDT.Plugins.Common.Plugin;
using HDT.Plugins.Common.Providers;
using HDT.Plugins.Common.Services;
using HDT.Plugins.StatsConverter.Converters;
using HDT.Plugins.StatsConverter.Models;
using HDT.Plugins.StatsConverter.Properties;
using Microsoft.Win32;

namespace HDT.Plugins.StatsConverter
{
	[Name("Stats Converter")]
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
			: base(Assembly.GetExecutingAssembly())
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

		public static List<Game> Filter(GameFilter filter)
		{
			_logger.Info($"Filter: {filter.Deck}, {filter.Mode}, {filter.Region}, {filter.TimeFrame}");
			var games = _data.GetAllGames();
			_logger.Info($"game count = {games.Count}");
			return filter.Apply(games);
		}

		public static void Export(IStatsConverter conveter, string filepath, List<Game> stats)
		{
			// TODO have loading spinner on view
			try
			{
				if (stats.Count <= 0)
					throw new Exception("No stats found");
				var stream = conveter.To(stats);
				using (var file = File.Create(@"E:\Dump\tmp.csv"))
				{
					stream.Seek(0, SeekOrigin.Begin);
					stream.CopyTo(file);
				}
			}
			catch (Exception e)
			{
				_logger.Error(e);
			}
		}

		public static void Export(IStatsConverter conveter, GameFilter filter)
		{
			var stats = Filter(filter);
			try
			{
				if (stats.Count <= 0)
					throw new Exception("No stats found");

				// set up and open save dialog
				SaveFileDialog dlg = new SaveFileDialog();
				dlg.FileName = GetDefaultFileName();
				dlg.DefaultExt = "." + conveter.FileExtension;
				dlg.InitialDirectory = Settings.Default.DefaultExportPath;
				dlg.Filter = conveter.Name + " Files | *." + conveter.FileExtension;
				bool? result = dlg.ShowDialog();

				// TODO failed message
				if (result != true)
					return;

				var filename = dlg.FileName;
				// export and save document
				//await Converter.Export(exporter, filename, stats);
				// export arena extras
				//if (mode == GameMode.Arena && CheckBoxArenaExtras.IsChecked == true)
				//{
				//	await Task.Run(() => Converter.ArenaExtras(filename, stats, deck, decks));
				//}

				var stream = conveter.To(stats);
				using (var file = File.Create(filename))
				{
					stream.Seek(0, SeekOrigin.Begin);
					stream.CopyTo(file);
				}
			}
			catch (Exception e)
			{
				_logger.Error(e);
			}
		}

		private static string GetDefaultFileName()
		{
			var name = Settings.Default.ExportFileName;
			if (Settings.Default.UseExportFileTimestamp)
			{
				name += "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
			}
			return name;
		}

		public static void ArenaExtras(string filename, List<Game> stats, Guid? deck, List<Deck> decks)
		{
			List<ArenaExtra> arenaRuns = null;
			if (deck == null)
				arenaRuns = decks
					.Where(x => x.IsArena && stats.Any(s => s.Deck.Id == x.Id))
					.Select(x => new ArenaExtra(x, stats))
					.OrderByDescending(x => x.LastPlayed).ToList();
			else
				arenaRuns = decks
					.Where(x => x.Id == deck && x.IsArena)
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
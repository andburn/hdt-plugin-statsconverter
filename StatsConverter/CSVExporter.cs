using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Hearthstone_Deck_Tracker.Stats;

namespace HDT.Plugins.StatsConverter
{
	public class CSVExporter : IStatsExporter
	{
		public string Name
		{
			get { return "CSV"; }
		}

		public string FileExtension
		{
			get { return "csv"; }
		}

		public void To(string file, List<GameStats> stats)
		{
			var wrapped = stats.Select(x => new GameStatsWrapper(x));
			using (var writer = new StreamWriter(file))
			using (var csv = new CsvWriter(writer))
			{
				csv.Configuration.RegisterClassMap<GameStatsWrapperMap>();
				csv.WriteHeader<GameStatsWrapper>();
				csv.WriteRecords(wrapped);
			}
		}
	}
}
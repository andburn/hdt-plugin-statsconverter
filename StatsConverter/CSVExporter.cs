using System.Collections.Generic;
using System.IO;
using CsvHelper;

namespace AndBurn.HDT.Plugins.StatsConverter
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
			using (var writer = new StreamWriter(file))
			{
				using (var csv = new CsvWriter(writer))
				{
					csv.Configuration.RegisterClassMap<GameStatsMap>();
					csv.WriteHeader<GameStats>();
					csv.WriteRecords(stats);
				}
			}
		}
	}
}
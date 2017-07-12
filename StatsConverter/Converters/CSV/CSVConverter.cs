using System.Collections.Generic;
using System.IO;
using CsvHelper;
using HDT.Plugins.Common.Models;
using HDT.Plugins.StatsConverter.Converters.CSV.Maps;

namespace HDT.Plugins.StatsConverter.Converters.CSV
{
	public class CSVConverter : IStatsConverter
	{
		public string Name
		{
			get { return "CSV"; }
		}

		public string FileExtension
		{
			get { return "csv"; }
		}

		public string Description
		{
			get { return "CSV files"; }
		}

		public List<Game> ConvertFromStream(Stream stream)
		{
			using (var streamReader = new StreamReader(stream))
			using (var csv = new CsvReader(streamReader))
			{
				csv.Configuration.RegisterClassMap<GameMap>();
				return new List<Game>(csv.GetRecords<Game>());
			}
		}

		public Stream ConvertToStream(List<Game> stats)
		{
			using (var memoryStream = new MemoryStream())
			{
				using (var streamWriter = new StreamWriter(memoryStream))
				using (var csvWriter = new CsvWriter(streamWriter))
				{
					csvWriter.Configuration.RegisterClassMap<GameMap>();
					csvWriter.WriteHeader<Game>();
					csvWriter.WriteRecords(stats);
				}
				// QSTN: why doesn't CopyTo work here instead
				return new MemoryStream(memoryStream.ToArray());
			}
		}
	}
}
using HDT.Plugins.Common.Models;
using System.Collections.Generic;
using System.IO;

namespace HDT.Plugins.StatsConverter.Converters
{
	public interface IStatsConverter
	{
		string Name { get; }

		string FileExtension { get; }

		string Description { get; }

		Stream ConvertToStream(List<Game> stats);

		List<Game> ConvertFromStream(Stream stream);
	}
}
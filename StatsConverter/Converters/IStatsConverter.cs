using System.Collections.Generic;
using System.IO;
using HDT.Plugins.Common.Models;

namespace HDT.Plugins.StatsConverter.Converters
{
	public interface IStatsConverter
	{
		string Name { get; }
		string FileExtension { get; }
		string Description { get; }

		Stream To(List<Game> stats);

		List<Game> From(Stream stream);
	}
}
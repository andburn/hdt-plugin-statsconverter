using System.Collections.Generic;
using System.IO;
using HDT.Plugins.Common.Models;
using HDT.Plugins.StatsConverter.Models;

namespace HDT.Plugins.StatsConverter.Converters
{
	public interface IStatsConverter
	{
		string Name { get; }
		string FileExtension { get; }
		string Description { get; }

		Stream To(List<Game> stats);

		Stream To(List<ArenaExtra> extras);

		List<Game> From(Stream stream);
	}
}
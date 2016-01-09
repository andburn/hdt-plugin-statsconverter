using System.Collections.Generic;

namespace AndBurn.HDT.Plugins.StatsConverter
{
	public interface IStatsImporter
	{
		string Name { get; }
		string FileExtension { get; }

		List<GameStats> From(string file);
	}
}
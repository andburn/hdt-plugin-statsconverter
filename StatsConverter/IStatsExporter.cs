using System.Collections.Generic;

namespace AndBurn.HDT.Plugins.StatsConverter
{
	public interface IStatsExporter
	{
		string Name { get; }
		string FileExtension { get; }

		void To(string file, List<GameStats> stats);
	}
}
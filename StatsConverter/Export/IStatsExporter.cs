using System.Collections.Generic;
using Hearthstone_Deck_Tracker.Stats;

namespace HDT.Plugins.StatsConverter.Export
{
	public interface IStatsExporter
	{
		string Name { get; }
		string FileExtension { get; }

		void To(string file, List<GameStats> stats);
	}
}
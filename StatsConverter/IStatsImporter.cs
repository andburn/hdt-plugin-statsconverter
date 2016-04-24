using System.Collections.Generic;
using Hearthstone_Deck_Tracker.Stats;

namespace AndBurn.HDT.Plugins.StatsConverter
{
	public interface IStatsImporter
	{
		string Name { get; }
		string FileExtension { get; }

		List<GameStats> From(string file);
	}
}
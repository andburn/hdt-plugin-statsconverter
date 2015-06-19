using System.Collections.Generic;
using Hearthstone_Deck_Tracker.Stats;
using Hearthstone_Deck_Tracker.Hearthstone;

namespace AndBurn.HDT.Plugins.StatsConverter
{
    public interface IStatsImporter
    {
        string Name { get; }
        string FileExtension { get; }

		// <Deck Name, Games>
		Dictionary<string, List<GameStats>> From(string file);
    }
}
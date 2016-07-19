using System.Collections.Generic;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Stats;

namespace HDT.Plugins.StatsConverter.Services
{
	public interface IStatsRepository
	{
		List<DeckStats> GetAllStats();

		List<Deck> GetAllDecks();
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Stats;

namespace HDT.Plugins.StatsConverter.Services
{
	public class HDTStatsRepository : IStatsRepository
	{
		private static readonly BindingFlags bindFlags =
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		public List<DeckStats> GetAllStats()
		{
			ReloadDeckStatsList();
			ReloadDefaultDeckStats();
			var ds = new List<DeckStats>(DeckStatsList.Instance.DeckStats.Values);
			ds.AddRange(DefaultDeckStats.Instance.DeckStats);
			return ds;
		}

		public List<Deck> GetAllDecks()
		{
			ReloadDeckList();
			return DeckList.Instance.Decks.ToList();
		}

		private void ReloadDeckList()
		{
			Type type = typeof(DeckList);
			MethodInfo method = type.GetMethod("Reload", bindFlags);
			method.Invoke(null, new object[] { });
		}

		private void ReloadDefaultDeckStats()
		{
			Type type = typeof(DefaultDeckStats);
			MethodInfo method = type.GetMethod("Reload", bindFlags);
			method.Invoke(null, new object[] { });
		}

		private void ReloadDeckStatsList()
		{
			Type type = typeof(DeckStatsList);
			MethodInfo method = type.GetMethod("Reload", bindFlags);
			method.Invoke(null, new object[] { });
		}
	}
}
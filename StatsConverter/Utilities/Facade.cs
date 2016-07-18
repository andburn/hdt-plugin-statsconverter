using System;
using System.Reflection;

namespace HDT.Plugins.StatsConverter.Utilities
{
	internal static class Facade
	{
		private static readonly BindingFlags bindFlags =
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		internal static void LoadDefaultDeckStats()
		{
			Type type = typeof(Hearthstone_Deck_Tracker.Stats.DefaultDeckStats);
			MethodInfo method = type.GetMethod("Reload", bindFlags);
			method.Invoke(null, new object[] { });
		}

		internal static void LoadDeckList()
		{
			Type type = typeof(Hearthstone_Deck_Tracker.DeckList);
			MethodInfo method = type.GetMethod("Reload", bindFlags);
			method.Invoke(null, new object[] { });
		}

		internal static void LoadDeckStatsList()
		{
			Type type = typeof(Hearthstone_Deck_Tracker.Stats.DeckStatsList);
			MethodInfo method = type.GetMethod("Reload", bindFlags);
			method.Invoke(null, new object[] { });
		}
	}
}
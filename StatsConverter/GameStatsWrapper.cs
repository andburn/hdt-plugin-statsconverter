using System;
using System.Text.RegularExpressions;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Stats;

namespace HDT.Plugins.StatsConverter
{
	public class GameStatsWrapper
	{
		private static readonly Regex _noteRegex =
			new Regex(@"^\s*\[(?<tag>([A-Za-z0-9\s_\-',]+))\]\s*(?<note>(.*))$",
				RegexOptions.Compiled);

		private GameStats _stats;

		public string DeckName { get; private set; }
		public string PlayerDeckVersionString { get; private set; }
		public string PlayerHero { get; private set; }
		public GameMode GameMode { get; private set; }
		public string RegionString { get; private set; }
		public string RankString { get; private set; }
		public DateTime StartTime { get; private set; }
		public string GotCoin { get; private set; }
		public string OpponentHero { get; private set; }
		public string OpponentName { get; private set; }
		public int Turns { get; private set; }
		public int SortableDuration { get; private set; }
		public GameResult Result { get; private set; }
		public bool WasConceded { get; private set; }
		public string Archetype { get; private set; }
		public string GameNote { get; private set; }
		public Guid GameId { get; private set; }

		public GameStatsWrapper()
		{
			_stats = null;
		}

		public GameStatsWrapper(GameStats stats)
		{
			_stats = stats;
			DeckName = _stats.DeckName;
			PlayerDeckVersionString = _stats.PlayerDeckVersionString;
			PlayerHero = _stats.PlayerHero;
			GameMode = _stats.GameMode;
			RegionString = _stats.RegionString;
			RankString = _stats.RankString;
			StartTime = _stats.StartTime;
			GotCoin = _stats.GotCoin;
			OpponentHero = _stats.OpponentHero;
			OpponentName = _stats.OpponentName;
			Turns = _stats.Turns;
			SortableDuration = _stats.SortableDuration;
			Result = _stats.Result;
			WasConceded = _stats.WasConceded;
			GameId = _stats.GameId;
			ParseNote();
		}

		public bool HasNoteArchetype()
		{
			return string.IsNullOrEmpty(Archetype);
		}

		private void ParseNote()
		{
			var match = _noteRegex.Match(_stats.Note);
			if (match.Success)
			{
				Archetype = match.Groups["tag"].Value;
				GameNote = match.Groups["note"].Value;
			}
			else
			{
				GameNote = _stats.Note;
			}
		}
	}
}
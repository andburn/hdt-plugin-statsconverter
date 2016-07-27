using System;
using System.Collections.Generic;
using System.Linq;
using HDT.Plugins.Common.Models;
using HDT.Plugins.Common.Util;

namespace HDT.Plugins.StatsConverter.Models
{
	public class ArenaExtra
	{
		public string Name { get; private set; }
		public string PlayerClass { get; private set; }
		public List<string> Cards { get; private set; }
		public int Gold { get; private set; }
		public int Dust { get; private set; }
		public int CardReward { get; private set; }
		public int Packs { get; private set; }
		public string Payment { get; private set; }
		public DateTime LastPlayed { get; private set; }
		public int Win { get; private set; }
		public int Loss { get; private set; }

		public ArenaExtra(Deck deck, List<GameStats> stats = null)
		{
			Name = deck.Name;
			PlayerClass = deck.Class;
			LastPlayed = deck.LastPlayed;
			Cards = deck.Cards.Select(x => x.ToString()).ToList();
			var run = RunRecord(deck, stats);
			Win = run.Item1;
			Loss = run.Item2;
			//Win = deck.GetGames().Count(g => g.Result == GameResult.Win);
			//Loss = deck.GetGames().Count(g => g.Result == GameResult.Loss);

			if (deck.ArenaReward != null)
			{
				Gold = deck.ArenaReward.Gold;
				Dust = deck.ArenaReward.Dust;
				CardReward = deck.ArenaReward.Cards.Where(x => x != null).Count();
				Packs = deck.ArenaReward.Packs.Where(x => !string.IsNullOrWhiteSpace(x)).Count();
				Payment = deck.ArenaReward.PaymentMethod.ToString();
			}
		}

		private Tuple<int, int> RunRecord(Deck d, List<GameStats> filtered)
		{
			var relevant = new List<GameStats>(d.DeckStats.Games);
			if (filtered != null)
			{
				relevant = relevant.Intersect(filtered).ToList();
			}
			return new Tuple<int, int>(
				relevant.Count(g => g.Result == GameResult.WIN),
				relevant.Count(g => g.Result == GameResult.LOSS));
		}
	}
}
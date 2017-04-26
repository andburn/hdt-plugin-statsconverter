using System;
using System.Collections.Generic;
using HDT.Plugins.Common.Enums;
using HDT.Plugins.Common.Models;

namespace StatsConverter.Tests
{
	public class TestHelper
	{
		public static List<Game> GetGameList()
		{
			return new List<Game>()
			{
				new Game() {
					Deck = new Deck(),
					Region = Region.US,
					Mode = GameMode.BRAWL,
					Format = GameFormat.WILD,
					StartTime = new DateTime(2015, 01, 25, 19, 03, 26),
					EndTime = new DateTime(2015, 01, 25, 19, 09, 14)
				},
				new Game() {
					Deck = new Deck(),
					Region = Region.EU,
					Mode = GameMode.RANKED,
					Format = GameFormat.STANDARD,
					StartTime = DateTime.Now - new TimeSpan(2, 10, 0),
					EndTime = DateTime.Now - new TimeSpan(2, 0, 0)
				},
			};
		}

		public static List<Card> GetCards()
		{
			return new List<Card>() {
				new Card() {
					Name = "Boot Hoarder",
					Id = "AB_123",
					Count = 2
				},
				new Card() {
					Name = "Acolyte of Rain",
					Id = "AB_789",
					Count = 1
				}
			};
		}

		public static Deck GetDeck()
		{
			return new Deck() {
				Name = "A Deck",
				Class = PlayerClass.PALADIN,
				LastPlayed = new DateTime(2015, 03, 12, 21, 11, 22),
				Cards = GetCards(),
				ArenaReward = new ArenaReward() {
					Gold = 100,
					Dust = 20,
					Cards = new List<Card>(),
					Packs = new List<string>() { "Classic" },
					PaymentMethod = "Gold"
				}
			};
		}
	}
}
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AndBurn.HDT.Plugins.StatsConverter.Test
{
    [TestClass]
    public class TestImport
    {
        [TestMethod]
        public void TestAddingGamesWithoutDeck()
        {
			var deck = new Hearthstone_Deck_Tracker.Hearthstone.Deck();
			deck.Name = "Some Deck";
			deck.DeckStats.AddGameResult(Hearthstone_Deck_Tracker.Enums.GameResult.Win, "Mage", "Warlock");
			deck.DeckStats.AddGameResult(Hearthstone_Deck_Tracker.Enums.GameResult.Loss, "Mage", "Warrior");
			Hearthstone_Deck_Tracker.DeckList.Instance.Decks.Add(deck);
			//Hearthstone_Deck_Tracker.DeckList.Save();

			Hearthstone_Deck_Tracker.DeckList.Load();
			var deckCount = Hearthstone_Deck_Tracker.DeckList.Instance.Decks.Count;
			var gameCount = Hearthstone_Deck_Tracker.Stats.DeckStatsList.Instance.DeckStats.First().Games.Count;
			Assert.AreEqual(1, deckCount);
			Assert.AreEqual(2, gameCount);
        }

    }
}

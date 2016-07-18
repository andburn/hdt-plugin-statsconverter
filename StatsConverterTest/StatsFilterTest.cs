using System;
using System.Collections.Generic;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Stats;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HDT.Plugins.StatsConverter.Test
{
	[TestClass]
	public class StatsFilterTest
	{
		private List<DeckStats> stats = new List<DeckStats>();
		private Guid deck = new Guid();
		private Guid empty = new Guid();

		[TestInitialize]
		public void Setup()
		{
			stats = TestHelper.SampleStats;
			deck = stats[1].DeckId;
			empty = stats[0].DeckId;
		}

		[TestCleanup]
		public void TearDown()
		{
			// nothing
		}

		[TestMethod]
		public void TestDefaultFilterReturnsAll()
		{
			var filter = new StatsFilter();
			var filtered = filter.Apply(stats);
			Assert.AreEqual(9, filtered.Count);
		}

		[TestMethod]
		public void TestSingleDeck()
		{
			var filter = new StatsFilter(deck, StatsRegion.All, GameMode.All, TimeFrame.AllTime);
			var filtered = filter.Apply(stats);
			Assert.AreEqual(1, filtered.Count);
		}

		[TestMethod]
		public void TestSingleDeckNoStats()
		{
			var filter = new StatsFilter(empty, StatsRegion.All, GameMode.All, TimeFrame.AllTime);
			var filtered = filter.Apply(stats);
			Assert.AreEqual(0, filtered.Count);
		}

		[TestMethod]
		public void TestUnknownDeckId()
		{
			var filter = new StatsFilter(new Guid(), StatsRegion.All, GameMode.All, TimeFrame.AllTime);
			var filtered = filter.Apply(stats);
			Assert.AreEqual(0, filtered.Count);
		}

		[TestMethod]
		public void TestRegionFilter()
		{
			var filter = new StatsFilter(null, StatsRegion.EU, GameMode.All, TimeFrame.AllTime);
			var filtered = filter.Apply(stats);
			Assert.AreEqual(5, filtered.Count);
		}

		[TestMethod]
		public void TestGameModeFilter()
		{
			var filter = new StatsFilter(null, StatsRegion.All, GameMode.Arena, TimeFrame.AllTime);
			var filtered = filter.Apply(stats);
			Assert.AreEqual(2, filtered.Count);
		}

		[TestMethod]
		public void TestTimeFrameFilter()
		{
			var filter = new StatsFilter(null, StatsRegion.All, GameMode.All, TimeFrame.Last7Days);
			var filtered = filter.Apply(stats);
			Assert.AreEqual(7, filtered.Count);
		}

		[TestMethod]
		public void TestTimeFrameTodayFilter()
		{
			var filter = new StatsFilter(null, StatsRegion.All, GameMode.All, TimeFrame.Today);
			var filtered = filter.Apply(stats);
			Assert.AreEqual(1, filtered.Count);
		}
	}
}
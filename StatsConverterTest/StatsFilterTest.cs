using System;
using System.Collections.Generic;
using HDT.Plugins.Common.Models;
using HDT.Plugins.Common.Util;
using HDT.Plugins.StatsConverter.Export;
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
			var filter = new GameFilter();
			var filtered = filter.Apply(stats);
			Assert.AreEqual(9, filtered.Count);
		}

		[TestMethod]
		public void TestSingleDeck()
		{
			var filter = new GameFilter(deck, Region.ALL, GameMode.ALL, TimeFrame.ALL);
			var filtered = filter.Apply(stats);
			Assert.AreEqual(1, filtered.Count);
		}

		[TestMethod]
		public void TestSingleDeckNoStats()
		{
			var filter = new GameFilter(empty, Region.ALL, GameMode.ALL, TimeFrame.ALL);
			var filtered = filter.Apply(stats);
			Assert.AreEqual(0, filtered.Count);
		}

		[TestMethod]
		public void TestUnknownDeckId()
		{
			var filter = new GameFilter(new Guid(), Region.ALL, GameMode.ALL, TimeFrame.ALL);
			var filtered = filter.Apply(stats);
			Assert.AreEqual(0, filtered.Count);
		}

		[TestMethod]
		public void TestRegionFilter()
		{
			var filter = new GameFilter(null, Region.EU, GameMode.ALL, TimeFrame.ALL);
			var filtered = filter.Apply(stats);
			Assert.AreEqual(5, filtered.Count);
		}

		[TestMethod]
		public void TestGameModeFilter()
		{
			var filter = new GameFilter(null, Region.ALL, GameMode.ARENA, TimeFrame.ALL);
			var filtered = filter.Apply(stats);
			Assert.AreEqual(2, filtered.Count);
		}

		[TestMethod]
		public void TestTimeFrameFilter()
		{
			var filter = new GameFilter(null, Region.ALL, GameMode.ALL, TimeFrame.LAST_7_DAYS);
			var filtered = filter.Apply(stats);
			Assert.AreEqual(7, filtered.Count);
		}

		[TestMethod]
		public void TestTimeFrameTodayFilter()
		{
			var filter = new GameFilter(null, Region.ALL, GameMode.ALL, TimeFrame.TODAY);
			var filtered = filter.Apply(stats);
			Assert.AreEqual(1, filtered.Count);
		}
	}
}
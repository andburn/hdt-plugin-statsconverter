﻿using System.Collections.Generic;
using HDT.Plugins.StatsConverter.Export;
using HDT.Plugins.StatsConverter.Utilities;
using Hearthstone_Deck_Tracker.Stats;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HDT.Plugins.StatsConverter.Test
{
	[TestClass]
	public class CSVExportTest
	{
		private List<DeckStats> stats = new List<DeckStats>();

		[TestInitialize]
		public void Setup()
		{
			stats = TestHelper.SampleStats;
		}

		[TestMethod]
		public void TestCsvHelper()
		{
			var exporter = new CSVExporter();
			var filter = new StatsFilter();
			var file = "sample-export.csv";
			exporter.To(file, filter.Apply(stats));
			var count = TestHelper.CountLines(file);
			Assert.AreEqual(10, count);
		}
	}
}
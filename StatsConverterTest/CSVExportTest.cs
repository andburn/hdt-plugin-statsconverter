using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Stats;
using Hearthstone_Deck_Tracker.Hearthstone;

namespace AndBurn.HDT.Plugins.StatsConverter.Test
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

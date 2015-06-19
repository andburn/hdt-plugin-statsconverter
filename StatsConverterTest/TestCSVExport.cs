using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Stats;
using Hearthstone_Deck_Tracker.Hearthstone;
using AndBurn.HDT.Plugins.StatsConverter.CSV;

namespace AndBurn.HDT.Plugins.StatsConverter.Test
{
    [TestClass]
    public class TestCSVExport
    {
        private List<DeckStats> stats = new List<DeckStats>();

        [TestInitialize]
        public void Setup()
        {
            stats = Helper.SampleStats;
        }

        [TestMethod]
        public void TestCsvExport()
        {
            var exporter = new CSVExporter();
            var filter = new StatsFilter();
            var file = "sample-export.csv";
            exporter.To(file, filter.Apply(stats));
            var count = Helper.CountLines(file);
            Assert.AreEqual(10, count);
        }
    }
}

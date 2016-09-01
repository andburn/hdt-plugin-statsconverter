using System;
using System.Collections.Generic;
using HDT.Plugins.Common.Models;
using HDT.Plugins.Common.Util;
using HDT.Plugins.StatsConverter.Converters.CSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StatsConverterTest
{
	[TestClass]
	public class Debugging
	{
		[TestMethod]
		public void TestMethod1()
		{
			var games = new List<Game>() {
				new Game() {
					Id = Guid.NewGuid(),
					Deck = new Deck(),
					Region = Region.US,
					Mode = GameMode.BRAWL,
					StartTime = new DateTime(2015, 01, 25, 19, 03, 26),
					EndTime = new DateTime(2015, 01, 25, 19, 09, 14)
				},
				new Game() {
					Id = Guid.NewGuid(),
					Deck = new Deck(),
					Region = Region.EU,
					Mode = GameMode.RANKED,
					StartTime = DateTime.Now - new TimeSpan(2, 10, 0),
					EndTime =  DateTime.Now - new TimeSpan(2, 0, 0)
				},
			};

			var s = new CSVConverter().To(games);
		}
	}
}
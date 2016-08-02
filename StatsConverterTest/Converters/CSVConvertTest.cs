using System;
using System.Collections.Generic;
using System.IO;
using HDT.Plugins.Common.Models;
using HDT.Plugins.Common.Util;
using HDT.Plugins.StatsConverter.Converters;
using HDT.Plugins.StatsConverter.Converters.CSV;
using NUnit.Framework;

namespace StatsConverterTest.Converters
{
	[TestFixture]
	public class CSVConvertTest
	{
		private IStatsConverter convert;
		private List<Game> games;
		private Stream stream;

		[OneTimeSetUp]
		public void Setup()
		{
			convert = new CSVConverter();
			games = new List<Game>() {
				new Game() {
					Deck = new Deck(),
					Region = Region.US,
					Mode = GameMode.BRAWL,
					StartTime = new DateTime(2015, 01, 25, 19, 03, 26),
					EndTime = new DateTime(2015, 01, 25, 19, 09, 14)
				},
				new Game() {
					Id = new Guid("00000000-0000-0000-0000-000000000000"),
					Deck = new Deck() { Name = "A Deck" },
					DeckVersion = new Version(1, 0),
					Region = Region.EU,
					Mode = GameMode.RANKED,
					Result = GameResult.LOSS,
					StartTime = new DateTime(2015, 01, 25, 19, 14, 36),
					EndTime = new DateTime(2015, 01, 25, 19, 24, 17),
					Rank = 12,
					PlayerClass = PlayerClass.WARLOCK,
					PlayerName = "ThePlayer",
					OpponentClass = PlayerClass.HUNTER,
					OpponentName = "后海大白鲨",
					Turns = 5,
					Seconds = 360,
					PlayerGotCoin = false,
					WasConceded = false,
					Note = new Note() { Text = "Some notes" }
				}
			};
		}

		[SetUp]
		public void TestSetup()
		{
			stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);//, new UnicodeEncoding()); TODO
			writer.WriteLine("Deck,Version,Class,Mode,Region,Rank,Start Time,Coin,Opponent Class,Opponent Name,Turns,Duration,Result,Conceded,Note,Archetype,Id");
			writer.WriteLine(",,Druid,Brawl,US,0,2015-01-25 19:03:26,No,Druid,,0,0,Win,No,,,00000000-0000-0000-0000-000000000000");
			writer.WriteLine("A Deck,1.0,Warlock,Ranked,EU,12,2015-01-25 19:14:36,No,Hunter,后海大白鲨,5,360,Loss,No,Some notes,,00000000-0000-0000-0000-000000000000");
			writer.Flush();
			stream.Position = 0;
		}

		[Test]
		public void ToStream()
		{
			var to = convert.To(games);
			FileAssert.AreEqual(stream, to);
		}

		[Test]
		public void FromStream()
		{
			var from = convert.From(stream);
			CollectionAssert.AreEqual(games, from); // TODO Game.Equals only on Id
		}
	}
}
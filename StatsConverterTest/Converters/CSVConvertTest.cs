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
					PlayerClass = PlayerClass.HUNTER,
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
					Minutes = 6,
					PlayerGotCoin = true,
					WasConceded = false,
					Note = new Note() { Text = "Some notes" }
				}
			};
		}

		[SetUp]
		public void TestSetup()
		{
			stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);
			writer.WriteLine("Deck,Version,Class,Mode,Region,Rank,Start Time,Coin,Opponent Class,Opponent Name,Turns,Duration,Result,Conceded,Note,Archetype,Id");
			writer.WriteLine(",,Hunter,Brawl,US,0,2015-01-25 19:03:26,No,All,,0,0,Win,No,,,00000000-0000-0000-0000-000000000000");
			writer.WriteLine("A Deck,1.0,Warlock,Ranked,EU,12,2015-01-25 19:14:36,Yes,Hunter,后海大白鲨,5,6,Loss,No,Some notes,,00000000-0000-0000-0000-000000000000");
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
		public void Should_MapFromStream_Correctly_WithAllProps()
		{
			var game = convert.From(stream)[1];
			Assert.AreEqual("A Deck", game.Deck.Name);			
			Assert.AreEqual(new Version(1, 0), game.DeckVersion);
			Assert.AreEqual(PlayerClass.WARLOCK, game.PlayerClass);
			Assert.AreEqual(GameMode.RANKED, game.Mode);
			Assert.AreEqual(Region.EU, game.Region);
			Assert.AreEqual(12, game.Rank);
			Assert.AreEqual(new DateTime(2015, 01, 25, 19, 14, 36), game.StartTime);
			Assert.AreEqual(new DateTime(), game.EndTime);
			Assert.IsTrue(game.PlayerGotCoin);
			Assert.AreEqual(PlayerClass.HUNTER, game.OpponentClass);
			Assert.AreEqual("后海大白鲨", game.OpponentName);
			Assert.AreEqual(5, game.Turns);
			Assert.AreEqual(6, game.Minutes);
			Assert.AreEqual(GameResult.LOSS, game.Result);
			Assert.IsFalse(game.WasConceded);
			Assert.AreEqual("Some notes", game.Note.Text);
			// TODO need test arch too
			Assert.AreEqual(string.Empty, game.Note.Archetype);
			Assert.AreEqual(Guid.Empty, game.Id);
		}

		[Test]
		public void Should_MapFromStream_Correctly_WithMissingProps()
		{
			var game = convert.From(stream)[0];
			Assert.AreEqual(string.Empty, game.Deck.Name);
			Assert.AreEqual(null, game.DeckVersion);
			Assert.AreEqual(PlayerClass.HUNTER, game.PlayerClass);
			Assert.AreEqual(GameMode.BRAWL, game.Mode);
			Assert.AreEqual(Region.US, game.Region);
			Assert.AreEqual(0, game.Rank);
			Assert.AreEqual(new DateTime(2015, 01, 25, 19, 03, 26), game.StartTime);
			Assert.AreEqual(new DateTime(), game.EndTime);
			Assert.IsFalse(game.PlayerGotCoin);
			Assert.AreEqual(PlayerClass.ALL, game.OpponentClass);
			Assert.AreEqual(string.Empty, game.OpponentName);
			Assert.AreEqual(0, game.Turns);
			Assert.AreEqual(0, game.Minutes);
			Assert.AreEqual(GameResult.WIN, game.Result);
			Assert.IsFalse(game.WasConceded);
			Assert.AreEqual(string.Empty, game.Note.Text);
			Assert.AreEqual(string.Empty, game.Note.Archetype);
			Assert.AreEqual(Guid.Empty, game.Id);
		}		
	}
}
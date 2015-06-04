using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Stats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndBurn.HDT.Plugins.StatsConverter.Test
{
    public class TestHelper
    {
        public static List<DeckStats> SampleStats;

        static TestHelper()
        {
            SampleStats = new List<DeckStats>();
            CreateStats(SampleStats);
        }

        public static GameStats CreateGame(Guid deck, Region region, GameMode mode, int days)
        {
            var game = new GameStats(GameResult.Win, "Mage", "Mage");
            game.DeckId = deck;
            game.Region = region;
            game.GameMode = mode;
            game.StartTime = DateTime.Now.Subtract(TimeSpan.FromDays(days));

            return game;
        }

        public static Deck[] CreateStats(List<DeckStats> list)
        {
            // Deck A - no stats
            var deckA = new Deck();
            deckA.Name = "DeckA";
            var deckAStats = new DeckStats(deckA);

            // Deck B - a single game
            var deckB = new Deck();
            deckB.Name = "DeckB";
            var deckBStats = new DeckStats(deckB);
            deckBStats.Games = new List<GameStats>()
            {
                CreateGame(deckB.DeckId, Region.EU, GameMode.Ranked, 2)
            };

            // Deck C - multiple games
            var deckC = new Deck();
            deckC.Name = "DeckC";
            var deckCStats = new DeckStats(deckC);
            deckCStats.Games = new List<GameStats>()
            {
                CreateGame(deckC.DeckId, Region.EU, GameMode.Arena, 2),
                CreateGame(deckC.DeckId, Region.EU, GameMode.Arena, 4),
                CreateGame(deckC.DeckId, Region.US, GameMode.Ranked, 7),
                CreateGame(deckC.DeckId, Region.ASIA, GameMode.Ranked, 30),
                CreateGame(deckC.DeckId, Region.US, GameMode.Casual, 1),
                CreateGame(deckC.DeckId, Region.UNKNOWN, GameMode.Casual, 1),
                CreateGame(deckC.DeckId, Region.EU, GameMode.Ranked, 2),
                CreateGame(deckC.DeckId, Region.EU, GameMode.Ranked, 80),
            };

            list.Add(deckAStats);
            list.Add(deckBStats);
            list.Add(deckCStats);

            return new Deck[] { deckA, deckB, deckC };
        }

        public static int CountLines(string file)
        {
            var count = 0;
            try
            {
                var lines = File.ReadLines(file);
                count = lines.Count();
            }
            catch (Exception e)
            {
                Console.WriteLine("File Not Found (" + e.Message + ")");
                count = -1;
            }
            return count;
        }
    }
}

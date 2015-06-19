using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Hearthstone_Deck_Tracker.Stats;
using AndBurn.HDT.Plugins.StatsConverter.HearthstoneTracker.Model;
using HDTDeck = Hearthstone_Deck_Tracker.Hearthstone.Deck;

namespace AndBurn.HDT.Plugins.StatsConverter.HearthstoneTracker
{
    public class HearthstoneTrackerImporter : IStatsImporter
    {
        private string defaultLocation;

        public HearthstoneTrackerImporter()
        {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            defaultLocation = Path.Combine(appdata, "HearthstoneTracker", "db.sdf");
        }

        public string Name
        {
            get { return "Hearthstone Tracker"; }
        }

        public string FileExtension
        {
            get { return "sdf"; }
        }

        public string DefaultLocation
        {
            get { return defaultLocation; }
        }

		public Dictionary<string, List<GameStats>> From(string file)
        {
            //var appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //string filepath = Path.Combine(appdata, "HearthstoneTracker", "db.sdf");
            string connectionString = "Data Source=" + file;
            using (HSDbContext dbContext = new HSDbContext(connectionString))
            {
                var data = dbContext.GameResults
                    .Include(x => x.Hero)
                    .Include(x => x.OpponentHero)
                    .Include(x => x.Deck)
                    .Include(x => x.ArenaSession)
                    .ToList();
                return Map(data);
            }
        }

		private Dictionary<string, List<GameStats>> Map(List<GameResult> data)
        {
            var stats = new Dictionary<string,List<GameStats>>();

			foreach (var game in data)
			{
				var deckName = game.Deck.Name;
				if (!stats.ContainsKey(deckName))
				{
					stats[deckName] = new List<GameStats>();
				}
				stats[deckName].Add(new GameResultAdapter(game));
			}

			return stats;
        }

	}
}

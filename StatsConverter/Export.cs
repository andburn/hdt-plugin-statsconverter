using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Stats;

using MahApps.Metro.Controls.Dialogs;

namespace AndBurn.HDT.Plugins.StatsConverter
{
    public class Export
    {
        public static async void AsCSV()
        {
            var controller = await Helper.MainWindow.ShowProgressAsync("Exporting to CSV", "Please Wait...");

            string filename = "HDT_Stats_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".csv";
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // use HDT to load the stats
            DeckStatsList.Load();

            // create the output file
            StreamWriter file = new StreamWriter(Path.Combine(desktop, filename));

            // write the header
            file.WriteLine(
                String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}",
                    "Deck Name",
                    "Deck Version",
                    "Start Time",
                    "Region",
                    "Rank",
                    "Player Hero",
                    "Player Coin",
                    "Opponent Hero",
                    "Opponent Name",
                    "Game Mode",
                    "Turns",
                    "Duration",
                    "Result",
                    "Conceded"
                )
            );

            // write stats for each game by deck
            foreach (var deckStats in DeckStatsList.Instance.DeckStats)
            {
                foreach (var game in deckStats.Games)
                {
                    file.WriteLine(
                        String.Format("\"{0}\",{1},{2},{3},{4},{5},{6},{7},\"{8}\",{9},{10},{11},{12},{13}",
                            game.DeckName,
                            game.PlayerDeckVersionString,
                            game.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            game.RegionString,
                            game.RankString,
                            game.PlayerHero,
                            game.GotCoin,
                            game.OpponentHero,
                            game.OpponentName,
                            game.GameMode,
                            game.Turns,
                            (game.EndTime - game.StartTime).Minutes,
                            game.Result,
                            game.WasConceded ? "Yes" : "No"
                        )
                    );
                }
            }

            file.Close();

            // close the progress dialog
            await controller.CloseAsync();
        }
    }
}

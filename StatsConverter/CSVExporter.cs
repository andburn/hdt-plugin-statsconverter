using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Hearthstone_Deck_Tracker.Stats;
using CsvHelper;

namespace AndBurn.HDT.Plugins.StatsConverter
{
    public class CSVExporter : IStatsExporter
    {
        public string Name
        {
            get { return "CSV"; }
        }

        public string FileExtension
        {
            get { return "csv"; }
        }

        public void To(string file, List<GameStats> stats)
        {
            var csv = new CsvWriter(new StreamWriter(file));
            csv.Configuration.RegisterClassMap<GameStatsMap>();

            csv.WriteHeader(typeof(GameStats));

            csv.WriteRecords(stats);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Hearthstone_Deck_Tracker.Stats;
using CsvHelper;
using Hearthstone_Deck_Tracker;

namespace AndBurn.HDT.Plugins.StatsConverter.CSV
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

        public string DefaultLocation
        {
            get { return String.Empty; }
        }

        public void To(string file, List<GameStats> stats)
        {
            using (var writer = new StreamWriter(file))
            {
                using (var csv = new CsvWriter(writer))
                {
                    csv.Configuration.RegisterClassMap<GameStatsMap>();
                    csv.WriteHeader<GameStats>();
                    csv.WriteRecords(stats);
                }
            }
        }
    }
}

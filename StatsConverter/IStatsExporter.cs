using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hearthstone_Deck_Tracker.Stats;

namespace AndBurn.HDT.Plugins.StatsConverter
{
    public interface IStatsExporter
    {
        string Name { get; }
        string FileExtension { get; }
        string DefaultLocation { get; }

        void To(string file, List<GameStats> stats);
    }
}

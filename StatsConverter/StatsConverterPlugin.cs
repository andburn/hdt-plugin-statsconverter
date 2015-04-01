using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hearthstone_Deck_Tracker.Plugins;

namespace AndBurn.HDT.Plugins.StatsConverter
{
    public class StatsConverterPlugin : IPlugin
    {
        public string Name
        {
            get { return "StatsConverter"; }
        }

        public string Description
        {
            get { return "Dumps all stats to the desktop in csv format."; }
        }

        public string ButtonText
        {
            get { return "Export"; }
        }

        public string Author
        {
            get { return "andburn"; }
        }

        public Version Version
        {
            get { return new Version(0, 1, 0); }
        }

        public void OnLoad()
        {
            // Nothing for now
        }

        public void OnUnload()
        {
            // Nothing for now
        }

        public void OnUpdate()
        {
            // Nothing for now
        }

        public void OnButtonPress()
        {
            Export.AsCSV();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hearthstone_Deck_Tracker.Stats;
using AndBurn.HDT.Plugins.StatsConverter.HearthstoneTracker.Model;

namespace AndBurn.HDT.Plugins.StatsConverter.HearthstoneTracker
{
    public class GameResultAdapter : GameStats
    {
        public GameResultAdapter(GameResult game)
            : base()                        
        {
			Result = game.Victory ?
				Hearthstone_Deck_Tracker.Enums.GameResult.Win : 
				Hearthstone_Deck_Tracker.Enums.GameResult.Loss;
			Coin = !game.GoFirst;
			StartTime = game.Started;
			EndTime = game.Stopped;			
			GameMode = GetMode(game.GameMode);
			Turns = game.Turns;
			WasConceded = game.Conceded;
			Note = game.Notes;
			PlayerHero = game.Hero.ClassName;
			OpponentHero = game.OpponentHero.ClassName;
        }

		private Hearthstone_Deck_Tracker.Enums.GameMode GetMode(int m)
		{
			switch (m)
			{
				case 2:
					return Hearthstone_Deck_Tracker.Enums.GameMode.Casual;
				case 3:
					return Hearthstone_Deck_Tracker.Enums.GameMode.Ranked;
				case 4:
					return Hearthstone_Deck_Tracker.Enums.GameMode.Practice;
				case 5:
					return Hearthstone_Deck_Tracker.Enums.GameMode.Arena;
				default:
					return Hearthstone_Deck_Tracker.Enums.GameMode.None;
			}
		}
    }
}

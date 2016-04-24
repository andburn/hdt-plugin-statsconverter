using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Stats;
using Hearthstone_Deck_Tracker.Utility.Logging;
using MahApps.Metro.Controls.Dialogs;
using StatsConverter.Properties;

namespace AndBurn.HDT.Plugins.StatsConverter
{
	public class HsLogImporter : IStatsImporter
	{
		private GameV2 _game;

		public HsLogImporter()
		{
			_game = Hearthstone_Deck_Tracker.API.Core.Game;
		}

		public string Name
		{
			get { return "Hearthstone Log"; }
		}

		public string FileExtension
		{
			get { return "txt"; }
		}

		public List<GameStats> From(string file)
		{
			ReadStaticLogFile(file);
			// importing is done via log reading,
			// do not need to add stats manualy
			return new List<GameStats>();
		}

		private async void ReadStaticLogFile(string file)
		{
			if (!_game.IsRunning)
			{
				await Hearthstone_Deck_Tracker.API.Core.MainWindow.ShowMessageAsync("Warning",
					"Hearthstone needs to be running to import from log files",
					MessageDialogStyle.Affirmative, null);
				Log.Error("Hearthstone needs to be running");
				return;
			}

			var controller = await Hearthstone_Deck_Tracker.API.Core.MainWindow.ShowProgressAsync("Importing Games", "Please Wait...");

			// get log path
			var hslog = Path.Combine(Config.Instance.HearthstoneDirectory, "Logs", "Power.log");
			if (!File.Exists(hslog))
			{
				//throw new FileNotFoundException("Hearthstone log not found", hslog);
				Log.Info("Log not found, it will be created", "StatsConverter");
			}

			// get log to import
			var filepath = Path.GetFullPath(file);
			if (!File.Exists(filepath))
			{
				throw new FileNotFoundException("File does not exist", filepath);
			}
			var filename = Path.GetFileName(filepath);
			var dirpath = Path.GetDirectoryName(filepath);

			string line = "";
			int linesAtATime = Settings.Default.FlushLines;

			var heroes = new HeroState(_game);

			using (StreamReader fileIn = new StreamReader(filepath))
			{
				try
				{
					using (FileStream fs = new FileStream(hslog, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
					using (StreamWriter sw = new StreamWriter(fs))
					{
						Log.Info("Starting to write HS log file", "StatsConverter");
						int lineCount = 0;
						while ((line = fileIn.ReadLine()) != null)
						{
							lineCount++;
							heroes.Read(line);
							sw.WriteLine(line);
							// every linesAtATime, flush the buffer
							// so it can be read by reader
							if (lineCount >= linesAtATime)
							{
								lineCount = 0;
								await sw.FlushAsync();
								await Task.Delay(Settings.Default.ReadFreq);
							}
						}
					}
				}
				catch (Exception e)
				{
					Log.Error(e, "StatsConverter");
				}
			}
			Log.Info("Finished writing to log file", "StatsConverter");
			// attempt to save stats
			await controller.CloseAsync();
		}

		private class HeroState
		{
			private readonly Regex playerRegex =
				new Regex(@"Player EntityID=\d+ PlayerID=(\d+)");

			private readonly Regex playerHandRegex =
				new Regex(@"TAG_CHANGE Entity=\[name=.+ id=\d+ zone=HAND zonePos=\d+ cardId=\w+ player=(\d+)\]");

			private readonly Regex heroZoneRegex =
				new Regex(@"TAG_CHANGE Entity=\[name=.+ id=\d+ zone=PLAY zonePos=\d+ cardId=(HERO\w+) player=(\d+)\]");

			private GameV2 game;
			private bool hasPlayerLine;
			private bool idsAreKnown;

			public PlayerEntity Player { get; set; }
			public PlayerEntity Opponent { get; set; }

			public bool HeroesAreKnown
			{
				get
				{
					if (idsAreKnown && !string.IsNullOrWhiteSpace(Player.Hero) && !string.IsNullOrWhiteSpace(Opponent.Hero))
					{
						return true;
					}
					return false;
				}
			}

			public HeroState(GameV2 game)
			{
				this.game = game;
				hasPlayerLine = false;
				idsAreKnown = false;
			}

			public void Read(string line)
			{
				if (HeroesAreKnown)
				{
					return;
				}

				var playerMatch = playerRegex.Match(line);
				var playerHand = playerHandRegex.Match(line);
				var heroZone = heroZoneRegex.Match(line);

				if (playerMatch.Success)
				{
					// possible new game, reset PlayerEntities
					Player = new PlayerEntity();
					Opponent = new PlayerEntity();
					idsAreKnown = false;
					hasPlayerLine = true;
				}
				else if (playerHand.Success && hasPlayerLine)
				{
					hasPlayerLine = false;
					// if id already assigned skip rest
					if (Player.Id != 0)
						return;
					Player.Id = int.Parse(playerHand.Groups[1].Value);
					Player.IsPlayer = true; // TODO: this is a bit pointless
											// assuming ids are either 1 or 2!
					Opponent.Id = Player.Id == 1 ? 2 : 1;
					idsAreKnown = true;
				}
				else if (heroZone.Success && idsAreKnown)
				{
					var hid = int.Parse(heroZone.Groups[2].Value);
					if (hid == Player.Id)
					{
						Player.Hero = heroZone.Groups[1].Value;
					}
					else if (hid == Opponent.Id)
					{
						Opponent.Hero = heroZone.Groups[1].Value;
					}
					if (HeroesAreKnown)
					{
						SetPlayers();
					}
				}
			}

			private void SetPlayers()
			{
				game.Player.Class = Database.GetHeroNameFromId(Player.Hero, false);
				game.Opponent.Class = Database.GetHeroNameFromId(Opponent.Hero, false);
			}
		}

		private class PlayerEntity
		{
			public int Id { get; set; }
			public string Hero { get; set; }
			public bool IsPlayer { get; set; }

			public PlayerEntity()
			{
				Id = 0;
				Hero = null;
				IsPlayer = false;
			}

			public override string ToString()
			{
				return string.Format("{0}: {1} {2}", Id, Hero, IsPlayer);
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.LogReader;
using Hearthstone_Deck_Tracker.Stats;
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
			if(!_game.IsRunning)
			{
				await Helper.MainWindow.ShowMessageAsync("Warning",
					"Hearthstone needs to be running to import from log files",
					MessageDialogStyle.Affirmative, null);
				Logger.WriteLine("Hearthstone needs to be running");
				return;
			}

			var controller = await Helper.MainWindow.ShowProgressAsync("Importing Games", "Please Wait...");

			// get log path
			var hslog = Path.Combine(Config.Instance.HearthstoneDirectory, "Hearthstone_Data", "output_log.txt");
			if(!File.Exists(hslog))
			{
				throw new FileNotFoundException("Hearthstone log not found", hslog);
			}

			// get log to import
			var filepath = Path.GetFullPath(file);
			if(!File.Exists(filepath))
			{
				throw new FileNotFoundException("File does not exist", filepath);
			}
			var filename = Path.GetFileName(filepath);
			var dirpath = Path.GetDirectoryName(filepath);

			string line = "";
			int linesAtATime = Settings.Default.FlushLines;
			// TODO: skip blank lines and/or "File..."
			using(StreamReader fileIn = new StreamReader(filepath))
			{
				try
				{
					using(FileStream fs = new FileStream(hslog, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
					using(StreamWriter sw = new StreamWriter(fs))
					{
						Logger.WriteLine("Starting to write HS log file", "StatsConverter");
						int lineCount = 0;
						while((line = fileIn.ReadLine()) != null)
						{
							lineCount++;
							sw.WriteLine(line);
							// every linesAtATime, flush the buffer
							// so it can be read by reader
							if(lineCount >= linesAtATime)
							{
								lineCount = 0;
								await sw.FlushAsync();
							}
						}
					}
				}
				catch(Exception e)
				{
					Logger.WriteLine(e.Message, "Error");
				}
			}
			Logger.WriteLine("Finished writing to log file", "StatsConverter");
			await controller.CloseAsync();
		}
	}
}

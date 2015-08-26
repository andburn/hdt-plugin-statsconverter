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

			// values
			string hsdata = "Hearthstone_Data";
			string hslog = "output_log.txt";

			// stop current log reading
			var currentLogReader = HsLogReaderV2.Instance;
			if (currentLogReader != null)
			{
				Logger.WriteLine("Stopping current HsLogReader", "StatsConverter");
				currentLogReader.Stop();
			}

			// create fake hs log and dir
			var filepath = Path.GetFullPath(file);
			if (!File.Exists(filepath))
			{
				throw new FileNotFoundException("File does not exist", filepath);
			}
			var filename = Path.GetFileName(filepath);
			var dirpath = Path.GetDirectoryName(filepath);
			// create temp "HS data folder" in file dir
			var fakeDataPath = Directory.CreateDirectory(Path.Combine(dirpath, hsdata)).FullName;
			var fakeLogPath = Path.Combine(fakeDataPath, hslog);

			Logger.WriteLine("Setting fake log file to " + fakeLogPath, "StatsConverter");

			// open stream to fake log file for writing
			FileStream fakeLog = new FileStream(fakeLogPath, 
				FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

			// create new reader
			Logger.WriteLine("Creating new HsLogReader", "StatsConverter");
			Logger.WriteLine("HsLogReaderV2.Instance" + " = " + HsLogReaderV2.Instance, "StatsConverter");

			HsLogReaderV2.Create(dirpath, Settings.Default.ReadFreq, true, true);
			var fakeLogReader = HsLogReaderV2.Instance;
			fakeLogReader.Start(_game);

			string line = "";
			int linesAtATime = Settings.Default.FlushLines;
			// TODO: skip blank lines and/or "File..."
			using (StreamReader fileIn = new StreamReader(filepath)) 
			{
				try
				{
					using (StreamWriter fileOut = new StreamWriter(fakeLog))
					{
						Logger.WriteLine("Starting to write to fake log file", "StatsConverter");
						int lineCount = 0;
						while ((line = fileIn.ReadLine()) != null)
						{
							lineCount++;
							fileOut.WriteLine(line);
							// evey linesAtATime, flush the buffer
							// so it can be read by reader
							if (lineCount >= linesAtATime)
							{
								lineCount = 0;
								await fileOut.FlushAsync();
							}
						}
					}				
				}
				catch (Exception e)
				{
					Logger.WriteLine(e.Message, "Error");
				}
			}

			Logger.WriteLine("Finished writing to log file", "StatsConverter");
			// stop the reader reading
			fakeLogReader.Stop();
			// ensure stream is closed
			fakeLog.Close();

			Logger.WriteLine("Resetting HsLogReader to default", "StatsConverter");
			HsLogReaderV2.Create();
			HsLogReaderV2.Instance.Start(_game);

			await controller.CloseAsync();
		}
	}
}

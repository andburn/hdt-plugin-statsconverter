using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Stats;
using MahApps.Metro.Controls.Dialogs;


namespace AndBurn.HDT.Plugins.StatsConverter
{
	public class HsLogImporter : IStatsImporter
	{
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
			if (!Game.IsRunning)
			{
				// TODO: popup - start hearthstone
				Logger.WriteLine("Hearthstone needs to be running");
				return;
			}

			var controller = await Helper.MainWindow.ShowProgressAsync("Importing Games", "Please Wait...");

			// TODO: test hero vs selected deck difference

			// values
			string hsdata = "Hearthstone_Data"; // NOTE: this is added in the HsLogReader!
			string hslog = "output_log.txt";

			// stop current log reading
			var currentLogReader = HsLogReader.Instance;
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
			HsLogReader.Create(dirpath, 100);
			var fakeLogReader = HsLogReader.Instance;
			fakeLogReader.Start();

			string line = "";
			int linesAtATime = 20;
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
			HsLogReader.Create();
			HsLogReader.Instance.Start();

			await controller.CloseAsync();
		}
	}
}

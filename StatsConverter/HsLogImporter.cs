using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Hearthstone;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndBurn.HDT.Plugins.StatsConverter
{
	public class HsLogImporter : IStatsImporter
	{
		public string Name
		{
			get { return "HsLogImporter"; }
		}

		public string FileExtension
		{
			get { return "txt"; }
		}

		public void From(string file)
		{
			ReadStaticLogFile(file);			
		}

		private async void ReadStaticLogFile(string file)
		{
			// TODO: Unload: HideMetroDialogAsync null ref error
			// TODO: test on current stable

			// NOTE: won't save game unless mode enabled in options (e.g. None)
			// NOTE: time/duration will be wrong, whenever imported & how long it takes
			// NOTE: deck will that which is active or default if none

			if (!Game.IsRunning)
			{
				// TODO: popup - start hearthstone
				Logger.WriteLine("Hearthstone needs to be running");
				return;
			}

			Logger.WriteLine("Reading Log");

			// values
			string hsdata = "Hearthstone_Data"; // NOTE: this is added in the HsLogReader!
			string hslog = "output_log.txt";

			// stop current log reading
			var currentLogReader = HsLogReader.Instance;
			if (currentLogReader != null)
			{
				Logger.WriteLine("Stopping current reader");
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

			Logger.WriteLine("File setup complete:");
			Logger.WriteLine("filename: " + filename);
			Logger.WriteLine("dirpath: " + dirpath);
			Logger.WriteLine("fakedatapath: " + fakeDataPath);
			Logger.WriteLine("fakepath: " + fakeLogPath);

			// open stream to fake log file for writing
			FileStream fakeLog = new FileStream(fakeLogPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

			// create new reader
			HsLogReader.Create(dirpath, 100);
			var newLogReader = HsLogReader.Instance;
			newLogReader.Start();

			string line = "";
			int linesAtATime = 10; // is buffer better?
			using (StreamReader fileIn = new StreamReader(filepath)) 
			{
				try
				{
					Logger.WriteLine("fs write = " + fakeLog.CanWrite);
					Logger.WriteLine("fs read = " + fakeLog.CanRead);
					StreamWriter fileOut = new StreamWriter(fakeLog);
					Logger.WriteLine("begin writing log");

					int lineCount = 0;
					while ((line = fileIn.ReadLine()) != null)
					{
						lineCount++;
						fileOut.WriteLine(line);
						if (lineCount >= linesAtATime)
						{
							lineCount = 0;
							await fileOut.FlushAsync();
							//await task.delay(10);
						}
					}
					fileOut.Close(); // also disables stream writing				
				}
				catch (Exception e)
				{
					Logger.WriteLine(e.Message, "Error");
				}
			}

			fakeLog.Close();
			newLogReader.Stop();

			var stats = Game.CurrentGameStats;
			if (stats != null)
				Logger.WriteLine("stats: turns={0}", stats.Turns);
		}
	}
}

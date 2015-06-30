using Hearthstone_Deck_Tracker;
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
			Logger.WriteLine("Reading Log");

			// stop current log reading
			var currentLogReader = HsLogReader.Instance;
			if (currentLogReader != null)
			{
				Logger.WriteLine("Stopping current reader");
				currentLogReader.Stop();
			}

			// create fake hs log and dir
			// File....
			FileStream fs = new FileStream(@"..\output_log.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

			// create new reader
			HsLogReader.Create(@"..", 100);
			var newLogReader = HsLogReader.Instance;
			newLogReader.Start();

			string line = "";
			int linesAtATime = 20; // is buffer better?
			using (StreamReader fileIn = new StreamReader(@"..\sample.txt"))
			{
				try
				{
					Logger.WriteLine("fs write = " + fs.CanWrite);
					Logger.WriteLine("fs read = " + fs.CanRead);
					StreamWriter fileOut = new StreamWriter(fs);
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
					fileOut.Close();
				}
				catch (Exception e)
				{
					Logger.WriteLine(e.Message, "Error");
				}
			}

			fs.Close();
			newLogReader.Stop();
		}
	}
}

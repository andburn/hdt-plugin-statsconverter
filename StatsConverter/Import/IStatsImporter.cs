using System.Collections.Generic;
using HDT.Plugins.Common.Models;

namespace HDT.Plugins.StatsConverter.Import
{
	public interface IStatsImporter
	{
		string Name { get; }
		string FileExtension { get; }

		List<GameStats> From(string file);
	}
}
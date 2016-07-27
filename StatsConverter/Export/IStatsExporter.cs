﻿using System.Collections.Generic;
using HDT.Plugins.Common.Models;

namespace HDT.Plugins.StatsConverter.Export
{
	public interface IStatsExporter
	{
		string Name { get; }
		string FileExtension { get; }

		void To(string file, List<GameStats> stats);
	}
}
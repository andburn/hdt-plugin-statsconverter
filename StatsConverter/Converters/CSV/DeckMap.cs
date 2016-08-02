using CsvHelper.Configuration;
using HDT.Plugins.Common.Models;

namespace HDT.Plugins.StatsConverter.Converters.CSV
{
	public class DeckMap : CsvClassMap<Deck>
	{
		public DeckMap()
		{
			Map(m => m.Name).Name("Deck");
		}
	}
}
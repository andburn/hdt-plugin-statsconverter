using CsvHelper.Configuration;
using HDT.Plugins.StatsConverter.Models;

namespace HDT.Plugins.StatsConverter.Converters.CSV.Maps
{
	public class ArenaExtraMap : CsvClassMap<ArenaExtra>
	{
		public ArenaExtraMap()
		{
			Map(m => m.Name).Name("Deck Name");
			Map(m => m.PlayerClass).Name("Class");
			Map(m => m.LastPlayed).Name("Date");
			Map(m => m.Win).Name("Win");
			Map(m => m.Loss).Name("Loss");
			Map(m => m.Payment);
			Map(m => m.Gold);
			Map(m => m.Dust);
			Map(m => m.Packs);
			Map(m => m.CardReward).Name("Cards");
			Map(m => m.Cards).TypeConverter<CardsConverter>().Name("Deck");
		}
	}
}
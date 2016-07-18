using System.Collections.Generic;
using System.Linq;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace HDT.Plugins.StatsConverter
{
	public sealed class ArenaExtraMap : CsvClassMap<ArenaExtra>
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
			Map(m => m.Cards).TypeConverter<CardConverter>().Name("Deck");
		}
	}

	public class CardConverter : DefaultTypeConverter
	{
		public override string ConvertToString(TypeConverterOptions options, object value)
		{
			if (value == null)
			{
				return string.Empty;
			}

			List<string> cards = value as List<string>;
			if (cards == null)
			{
				return string.Empty;
			}

			return cards.Select(x => x.Replace(',', ' ')).Aggregate((x, y) => $"{x}|{y}");
		}
	}
}
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace HDT.Plugins.StatsConverter.Models
{
	public sealed class GameStatsWrapperMap : CsvClassMap<GameStatsWrapper>
	{
		public GameStatsWrapperMap()
		{
			Map(m => m.DeckName).Name("Deck");
			Map(m => m.PlayerDeckVersionString).Name("Version");
			Map(m => m.PlayerHero).Name("Hero");
			Map(m => m.GameMode).Name("Mode");
			Map(m => m.RegionString).Name("Region");
			Map(m => m.RankString).Name("Rank");
			Map(m => m.StartTime).TypeConverterOption("yyyy-MM-dd HH:mm:ss").Name("Start Time");
			Map(m => m.GotCoin).Name("Coin");
			Map(m => m.OpponentHero).Name("Opponent Hero");
			Map(m => m.OpponentName).Name("Opponent Name");
			Map(m => m.Turns);
			Map(m => m.SortableDuration).Name("Duration");
			Map(m => m.Result);
			Map(m => m.WasConceded).TypeConverter<BooleanConverter>().Name("Conceded");
			Map(m => m.GameNote);
			Map(m => m.Archetype);
			Map(m => m.GameId);
		}
	}
}
using CsvHelper.Configuration;
using HDT.Plugins.Common.Models;

namespace HDT.Plugins.StatsConverter.Converters.CSV
{
	public class GameMap : CsvClassMap<Game>
	{
		public GameMap()
		{
			References<DeckMap>(m => m.Deck);
			Map(m => m.DeckVersion)
				.TypeConverter<VersionConverter>()
				.Name("Version");
			Map(m => m.PlayerClass)
				.TypeConverter<EnumConverter>()
				.TypeConverterOption("PlayerClass")
				.Name("Class");
			Map(m => m.Mode)
				.TypeConverter<EnumConverter>()
				.TypeConverterOption("Mode");
			Map(m => m.Region);
			Map(m => m.Rank);
			Map(m => m.StartTime)
				.TypeConverterOption("yyyy-MM-dd HH:mm:ss")
				.Name("Start Time");
			Map(m => m.PlayerGotCoin)
				.TypeConverter<BooleanConverter>()
				.Name("Coin");
			Map(m => m.OpponentClass)
				.TypeConverter<EnumConverter>()
				.TypeConverterOption("PlayerClass")
				.Name("Opponent Class");
			Map(m => m.OpponentName).Name("Opponent Name");
			Map(m => m.Turns);
			Map(m => m.Minutes).Name("Duration");
			Map(m => m.Result)
				.TypeConverter<EnumConverter>()
				.TypeConverterOption("GameResult");
			Map(m => m.WasConceded)
				.TypeConverter<BooleanConverter>()
				.Name("Conceded");
			References<NoteMap>(m => m.Note);
			Map(m => m.Id);
		}
	}
}
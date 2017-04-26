using CsvHelper.Configuration;
using HDT.Plugins.Common.Models;

namespace HDT.Plugins.StatsConverter.Converters.CSV
{
	public class NoteMap : CsvClassMap<Note>
	{
		public NoteMap()
		{
			Map(m => m.Text).Name("Note");
			Map(m => m.Archetype);
		}
	}
}
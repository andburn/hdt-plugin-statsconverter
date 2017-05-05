using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using HDT.Plugins.Common.Enums;
using HDT.Plugins.Common.Models;

namespace HDT.Plugins.StatsConverter.Converters.XML
{
	public class OpenXMLConverter : IStatsConverter
	{
		private static readonly EnumStringConverter _converter = new EnumStringConverter();

		private string[] _header = new string[] {
			"Deck", "Version", "Class", "Mode", "Region", "Rank", "Start Time",
			"Coin", "Opponent Class", "Opponent Name", "Turns", "Duration",
			"Result", "Conceded", "Archetype", "Note", "Id"
		};

		public string Name => "Excel";

		public string FileExtension => "xlsx";

		public string Description => "Excel files";

		public List<Game> From(Stream stream)
		{
			throw new NotImplementedException();
		}

		public Stream To(List<Game> stats)
		{
			var workbook = new XLWorkbook();
			var worksheet = workbook.Worksheets.Add("HDT Stats");

			var row = 1;
			// add header row
			for (int i = 0; i < _header.Length; i++)
			{
				worksheet.Cell(row, i + 1).Value = _header[i];
			}
			// add data rows
			foreach (var s in stats)
			{
				WriteRow(worksheet, s, ++row);
			}
			// save to a stream and return
			using (var memoryStream = new MemoryStream())
			{
				workbook.SaveAs(memoryStream);
				return new MemoryStream(memoryStream.ToArray());
			}
		}

		private void WriteRow(IXLWorksheet sheet, Game game, int row)
		{
			var props = new object[] {
				game.Deck?.Name,
				$"{game.DeckVersion.Major}.{game.DeckVersion.Minor}",
				EnumStringConverter.ToTitleCase(game.PlayerClass),
				EnumStringConverter.ToTitleCase(game.Mode),
				game.Region,
				game.Rank,
				game.StartTime,
				game.PlayerGotCoin ? "Yes" : "No",
				EnumStringConverter.ToTitleCase(game.OpponentClass),
				game.OpponentName,
				game.Turns,
				game.Minutes,
				EnumStringConverter.ToTitleCase(game.Result),
				game.WasConceded ? "Yes" : "No",
				game.Note?.Archetype,
				game.Note?.Text,
				game.Id
			};
			for (var i = 0; i < props.Length; i++)
			{
				sheet.Cell(row, i + 1).Value = props[i];
			}
		}
	}
}
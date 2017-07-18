using ClosedXML.Excel;
using HDT.Plugins.Common.Enums;
using HDT.Plugins.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace HDT.Plugins.StatsConverter.Converters.XML
{
	public class OpenXMLConverter : IStatsConverter
	{
		private static readonly EnumStringConverter _converter = new EnumStringConverter();

		private string[] propNames = new string[] {
			"Deck", "Version", "Class", "Mode", "Region", "Rank", "Start Time",
			"Coin", "Opponent Class", "Opponent Name", "Turns", "Duration",
			"Result", "Conceded", "Note", "Archetype", "Id"
		};

		public string Name => "Excel";

		public string FileExtension => "xlsx";

		public string Description => "Excel files";

		public List<Game> ConvertFromStream(Stream stream)
		{
			var games = new List<Game>();
			var workbook = new XLWorkbook(stream);
			var worksheet = workbook.Worksheet(1);

			// get worksheet dimensions
			var range = worksheet.RangeUsed();
			var rows = range.RowCount();
			var cols = range.ColumnCount();

			if (rows <= 0)
			{
				throw new ConverterException("The sheet is empty");
			}
			else if (cols > propNames.Length)
			{
				throw new ConverterException(
					$"Too many columns in sheet: {cols} instead of {propNames.Length}");
			}

			// ASSUMPTIONS:
			// - header is in row 1
			// - games starts at row 2
			// - colunmn 1 is the first value

			// create a game obj from each row (skip header)
			for (int i = 2; i <= rows; i++)
			{
				var rowData = new object[cols];
				for (int j = 1; j <= cols; j++)
				{
					rowData[j - 1] = worksheet.Cell(i, j).Value;
				}
				games.Add(ReadRow(rowData));
			}

			return games;
		}

		public Stream ConvertToStream(List<Game> stats)
		{
			var workbook = new XLWorkbook();
			var worksheet = workbook.Worksheets.Add("HDT Stats");

			var row = 1;
			// add header row
			for (int i = 0; i < propNames.Length; i++)
			{
				worksheet.Cell(row, i + 1).Value = propNames[i];
			}
			// add data rows
			foreach (var s in stats)
			{
				WriteRow(worksheet, s, ++row);
			}
			// save to a stream and return
			using (var memoryStream = new MemoryStream())
			{
				worksheet.Columns().AdjustToContents();
				workbook.SaveAs(memoryStream);
				return new MemoryStream(memoryStream.ToArray());
			}
		}

		private Game ReadRow(object[] props)
		{
			var game = new Game();
			
			game.Deck = new Deck();
			var deckName = props[0].ToString();
			if (!string.IsNullOrWhiteSpace(deckName))
				game.Deck.Name = deckName;

			Version.TryParse(props[1].ToString(), out Version version);
			game.DeckVersion = version;

			game.PlayerClass = Common.Enums.Convert.ToHeroClass(props[2].ToString());

			Enum.TryParse(StringUpper(3, props), out GameMode gmode);
			game.Mode = gmode;

			Enum.TryParse(StringUpper(4, props), out Region region);
			game.Region = region;

			int.TryParse(props[5].ToString(), out int rank);
			game.Rank = rank;

			DateTime.TryParse(props[6].ToString(), out DateTime stime);
			game.StartTime = stime;

			game.PlayerGotCoin = props[7].ToString() == "Yes";

			Enum.TryParse(StringUpper(8, props), out PlayerClass oclass);
			game.OpponentClass = oclass;

			game.OpponentName = props[9].ToString();

			int.TryParse(props[10].ToString(), out int turns);
			game.Turns = turns;

			int.TryParse(props[11].ToString(), out int mins);
			game.Minutes = mins;

			Enum.TryParse(StringUpper(12, props), out GameResult result);
			game.Result = result;

			game.WasConceded = props[13].ToString() == "Yes";

			game.Note = new Note(props[14].ToString());
			var arch = props[15].ToString();
			game.Note.Archetype = string.IsNullOrEmpty(arch) ? null : arch;

			game.Id = new Guid(props[16].ToString());

			return game;
		}

		private void WriteRow(IXLWorksheet sheet, Game game, int row)
		{
			var props = new CellValue[] {
				new CellValue(game.Deck?.Name),
				new CellValue(game.DeckVersion),
				new CellValue(EnumStringConverter.ToTitleCase(game.PlayerClass)),
				new CellValue(EnumStringConverter.ToTitleCase(game.Mode)),
				new CellValue(game.Region),
				new CellValue(game.Rank, XLCellValues.Number),
				new CellValue(game.StartTime, XLCellValues.DateTime),
				new CellValue(game.PlayerGotCoin ? "Yes" : "No"),
				new CellValue(EnumStringConverter.ToTitleCase(game.OpponentClass)),
				new CellValue(game.OpponentName),
				new CellValue(game.Turns, XLCellValues.Number),
				new CellValue(game.Minutes, XLCellValues.Number),
				new CellValue(EnumStringConverter.ToTitleCase(game.Result)),
				new CellValue(game.WasConceded ? "Yes" : "No"),
				new CellValue(game.Note?.Text),
				new CellValue(game.Note?.Archetype),
				new CellValue(game.Id),
			};
			for (var i = 0; i < props.Length; i++)
			{
				sheet.Cell(row, i + 1).Value = props[i].Value;
				sheet.Cell(row, i + 1).SetDataType(props[i].Type);
				//sheet.Cell(row, i + 1).SetValue<string>(props[i].Value?.ToString());
				//sheet.Cell(row, i + 1).SetValue(props[i]);				
			}
		}

		private string StringUpper(int index, object[] props)
		{
			return props[index].ToString().ToUpperInvariant();
		}

		private class CellValue
		{
			public object Value { get; }
			public XLCellValues Type { get; }

			public CellValue()
			{
				Value = null;
				Type = XLCellValues.Text;
			}

			public CellValue(object v)
				: this()
			{
				Value = v;
			}

			public CellValue(object v, XLCellValues t)
			{
				Value = v;
				Type = t;
			}
		}
	}
}
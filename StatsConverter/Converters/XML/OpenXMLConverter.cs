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

		private string[] propNames = new string[] {
			"Deck", "Version", "Class", "Mode", "Region", "Rank", "Start Time",
			"Coin", "Opponent Class", "Opponent Name", "Turns", "Duration",
			"Result", "Conceded", "Archetype", "Note", "Id"
		};

		public string Name => "Excel";

		public string FileExtension => "xlsx";

		public string Description => "Excel files";

		public List<Game> From(Stream stream)
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
					rowData[j] = worksheet.Cell(i, j).Value;
				}
				games.Add(ReadRow(rowData));
			}

			return games;
		}

		public Stream To(List<Game> stats)
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
				workbook.SaveAs(memoryStream);
				return new MemoryStream(memoryStream.ToArray());
			}
		}

		private Game ReadRow(object[] props)
		{
			var game = new Game();
			// TODO is adding a deck a good idea?
			//game.Deck = new Deck() { Name = props[0].ToString() };
			game.DeckVersion = new Version(props[1].ToString());
			Enum.TryParse(props[2].ToString(), out PlayerClass pclass);
			game.PlayerClass = pclass;
			Enum.TryParse(props[3].ToString(), out GameMode gmode);
			game.Mode = gmode;
			Enum.TryParse(props[4].ToString(), out Region region);
			game.Region = region;
			int.TryParse(props[5].ToString(), out int rank);
			game.Rank = rank;
			DateTime.TryParse(props[6].ToString(), out DateTime stime);
			game.StartTime = stime;
			game.PlayerGotCoin = props[7].ToString() == "Yes";
			Enum.TryParse(props[8].ToString(), out PlayerClass oclass);
			game.OpponentClass = oclass;
			game.OpponentName = props[9].ToString();
			int.TryParse(props[10].ToString(), out int turns);
			game.Turns = turns;
			int.TryParse(props[11].ToString(), out int mins);
			game.Minutes = mins;
			Enum.TryParse(props[12].ToString(), out GameResult result);
			game.Result = result;
			game.WasConceded = props[13].ToString() == "Yes";
			game.Note = new Note() {
				Archetype = props[14].ToString(),
				Text = props[15].ToString()
			};
			game.Id = new Guid(props[16].ToString());

			return game;
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
using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper.TypeConversion;
using HDT.Plugins.Common.Enums;

namespace HDT.Plugins.StatsConverter.Converters.CSV
{
	// "Trouble with CSV-Helper not converting bool values" http://stackoverflow.com/a/22998705
	public class BooleanConverter : DefaultTypeConverter
	{
		public override string ConvertToString(TypeConverterOptions options, object value)
		{
			if (value == null)
				return "No";

			var boolValue = (bool)value;

			return boolValue ? "Yes" : "No";
		}

		public override object ConvertFromString(TypeConverterOptions options, string text)
		{
			if (string.IsNullOrEmpty(text))
				return false;

			return text.ToLowerInvariant() == "yes" ? true : false;
		}

		public override bool CanConvertFrom(Type type)
		{
			return type == typeof(string);
		}
	}

	public class VersionConverter : DefaultTypeConverter
	{
		public override string ConvertToString(TypeConverterOptions options, object value)
		{
			if (value == null)
				return string.Empty;

			var version = value as Version;
			if (version == null)
				return string.Empty;

			return $"{version.Major}.{version.Minor}";
		}

		public override object ConvertFromString(TypeConverterOptions options, string text)
		{
			if (string.IsNullOrEmpty(text))
				return null;

			Version result = null;
			Version.TryParse(text.Trim(), out result);
			return result;
		}

		public override bool CanConvertFrom(Type type)
		{
			return type == typeof(string);
		}
	}

	public class EnumConverter : DefaultTypeConverter
	{
		private static readonly EnumStringConverter _converter = new EnumStringConverter();

		public override string ConvertToString(TypeConverterOptions options, object value)
		{
			if (value == null)
				return "";

			return _converter.Convert(value, typeof(string), null, options.CultureInfo).ToString();
		}

		public override object ConvertFromString(TypeConverterOptions options, string text)
		{
			if (string.IsNullOrEmpty(text))
				return null;

			Type type;
			switch (options.Format.ToLowerInvariant())
			{
				case "mode":
					type = typeof(GameMode); break;
				case "region":
					type = typeof(Region); break;
				case "playerclass":
					type = typeof(PlayerClass); break;
				case "gameresult":
					type = typeof(GameResult); break;
				case "format":
					type = typeof(GameFormat); break;
				default:
					throw new CsvTypeConverterException("Unknown type: " + options.Format);
			}

			return _converter.ConvertBack(text, type, null, options.CultureInfo);
		}

		public override bool CanConvertFrom(Type type)
		{
			return type == typeof(string);
		}
	}

	public class CardsConverter : DefaultTypeConverter
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

	public class GameIdConverter : DefaultTypeConverter
	{
		public override string ConvertToString(TypeConverterOptions options, object value)
		{
			if (string.IsNullOrWhiteSpace(value.ToString()))
				return string.Empty;

			var g = value as Guid?;
			if (g == null)
				return string.Empty;

			return g.ToString();
		}

		public override object ConvertFromString(TypeConverterOptions options, string text)
		{
			if (string.IsNullOrEmpty(text))
				return Guid.Empty;

			var ok = Guid.TryParse(text, out Guid g);
			if (ok)
				return g;

			return Guid.Empty;
		}

		public override bool CanConvertFrom(Type type)
		{
			return true;
		}
	}
}
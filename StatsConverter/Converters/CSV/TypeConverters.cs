using System;
using System.IO;
using CsvHelper.TypeConversion;
using HDT.Plugins.Common.Util;

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
			// TODO how handle null?
			if (string.IsNullOrEmpty(text))
				return null;

			// TODO a default type or error?
			Type type = typeof(object);

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
}
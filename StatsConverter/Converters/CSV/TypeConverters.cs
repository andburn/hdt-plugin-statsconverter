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
			{
				return "No";
			}

			var boolValue = (bool)value;

			return boolValue ? "Yes" : "No";
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
	}
}
using System;

namespace HDT.Plugins.StatsConverter.Converters
{
	public class ConverterException : Exception
	{
		public ConverterException()
			: base()
		{
		}

		public ConverterException(string message)
			: base(message)
		{
		}
	}
}
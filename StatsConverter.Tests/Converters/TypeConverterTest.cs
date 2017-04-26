using System;
using System.Linq;
using CsvHelper.TypeConversion;
using HDT.Plugins.Common.Models;
using HDT.Plugins.StatsConverter.Converters.CSV;
using NUnit.Framework;

namespace StatsConverter.Tests.Converters
{
	[TestFixture]
	public class TypeConverterTest
	{
		[Test]
		public void VersionConverter_NotVersionType_IsEmpty()
		{
			var cnv = new VersionConverter();
			Assert.AreEqual(string.Empty, cnv.ConvertToString(new TypeConverterOptions(), 0));
		}

		[Test]
		public void VersionConverter_ConvertsVersionToMajorMinorString()
		{
			var cnv = new VersionConverter();
			Assert.AreEqual("2.1", cnv.ConvertToString(new TypeConverterOptions(), new Version(2, 1, 2)));
		}

		[Test]
		public void VersionConverter_ConvertsStringToVersion()
		{
			var cnv = new VersionConverter();
			Assert.AreEqual(new Version(1, 2, 3), cnv.ConvertFromString(new TypeConverterOptions(), "1.2.3"));
		}

		[Test]
		public void CardsConverter_NotCardType_IsEmpty()
		{
			var cnv = new CardsConverter();
			Assert.AreEqual(string.Empty, cnv.ConvertToString(new TypeConverterOptions(), 0));
		}

		[Test]
		public void CardsConverter_ConvertsCardToString()
		{
			var cnv = new CardsConverter();
			var cards = TestHelper.GetCards().Select(c => c.ToString()).ToList();
			Assert.AreEqual("Boot Hoarder x2|Acolyte of Rain x1", 
				cnv.ConvertToString(new TypeConverterOptions(), cards));
		}
	}
}
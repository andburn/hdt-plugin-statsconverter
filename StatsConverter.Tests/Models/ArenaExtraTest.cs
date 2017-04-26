using System;
using System.Collections.Generic;
using HDT.Plugins.Common.Models;
using HDT.Plugins.Common.Services;
using HDT.Plugins.StatsConverter.Models;
using Moq;
using NUnit.Framework;

namespace StatsConverter.Tests.Models
{
	[TestFixture]
	public class ArenaExtraTest
	{
		[Test]
		public void NullDeckInCtor_ThrowsException()
		{
			var mock = new Mock<IDataRepository>();
			mock.Setup(x => x.GetAllGamesWithDeck(It.IsAny<Guid>()))
				.Returns(TestHelper.GetGameList());
			Assert.Throws<NullReferenceException>(() => new ArenaExtra(mock.Object, null));
		}

		[Test]
		public void ZeroWinLoss_WithNoGames()
		{
			var mock = new Mock<IDataRepository>();
			mock.Setup(x => x.GetAllGamesWithDeck(It.IsAny<Guid>()))
				.Returns(new List<Game>());
			var ae = new ArenaExtra(mock.Object, TestHelper.GetDeck());
			Assert.AreEqual(0, ae.Win);
			Assert.AreEqual(0, ae.Loss);
		}
	}
}
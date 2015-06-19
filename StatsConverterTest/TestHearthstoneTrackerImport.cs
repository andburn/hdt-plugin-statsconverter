using System;
using System.IO;
using System.Linq;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AndBurn.HDT.Plugins.StatsConverter.HearthstoneTracker.Model;

namespace AndBurn.HDT.Plugins.StatsConverter.Test
{
    [TestClass]
    public class TestHearthstoneTrackerImport
    {
        private static HSDbContext dbContext;
        private int count;
        
        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            string connectionString = "Data Source=./Data/hearthstone_tracker_db.sdf";
            dbContext = new HSDbContext(connectionString);            
        }

        [TestInitialize]
        public void TestSetup()
        {
            count = -1;
        }

        [ClassCleanup]
        public static void Teardown()
        {
            dbContext.Dispose();
        }

        [TestMethod]
        public void TestEmptyDB()
        {
            try
            {
                var conn = new HSDbContext("Data Source=./Data/empty_db.sdf");
                var data = conn.GameResults.ToList();
                count = data.Count;
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void TestReadingGames()
        {            
            try
            {
                var data = dbContext.GameResults.ToList();
                count = data.Count;          
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            Assert.AreEqual(9, count);
        }

        [TestMethod]
        public void TestReadingArenaGames()
        {
            try
            {
                var id = new Guid("a3e2a080-dda7-4300-84fa-46d10e2e7ba8");
                var data = dbContext.ArenaSessions.Single(a => a.Id == id);
                count = data.GameResults.Count;
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            Assert.AreEqual(4, count);
        }

        [TestMethod]
        public void TestReadingArenaWithNoGames()
        {
            try
            {
                var id = new Guid("9fe013a8-0c3b-4c7d-bc9d-62b8553d0fa7");
                var data = dbContext.ArenaSessions.Single(a => a.Id == id);
                count = data.GameResults.Count;
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void TestReadingDecks()
        {         
            try
            {
                var data = dbContext.Decks.ToList();
                count = data.Count;
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            Assert.AreEqual(11, count);
        }

        [TestMethod]
        public void TestReadingDeckWithNoGames()
        {
            try
            {
                var data = dbContext.Decks.Single(d => d.Name.Equals("Face Hunter v1.9"));
                count = data.GameResults.Count;
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            Assert.AreEqual(0, count);
        }

    }
}

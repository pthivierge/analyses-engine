using System;
using System.Linq;
using AnalysesEngine.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApplicationTests
{
    [TestClass]
    public class TimeStampsGeneratorTests
    {
        [TestMethod]
        public void TestTimeStampsGeneration()
        {
            // testing that we get 30 timestamps
            var st = DateTime.Today.AddDays(-5);
            var et = DateTime.Today;
            var tsList = TimeStampsGenerator.Get(TimeStampsGenerator.TimeStampsInterval.Daily,st,et );

            Assert.IsTrue(tsList.Count == 6, "has 6 timestamps");
            Assert.IsTrue(tsList[0]==st,"st is there");
            Assert.IsTrue(tsList[5]==et,"et is there");

        }
    }
}

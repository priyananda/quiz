using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RK.Common.Util;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace RK.Common.Tests
{
    [TestClass]
    public class TimeMeasurementTests
    {
        [TestMethod]
        public void TestActivityDurationCalculatorSums()
        {
            List<ActivityDurationResult> results = new List<ActivityDurationResult>();

            ActivityDurationCalculator calculator = new ActivityDurationCalculator(string.Empty, 10);
            calculator.ActivityDurationCalculated += (sender, eArgs) => results.Add(eArgs.ActivityDuration);

            calculator.NotifyActivityDuration(10);
            calculator.NotifyActivityDuration(100);
            calculator.NotifyActivityDuration(50);
            calculator.NotifyActivityDuration(40);
            calculator.NotifyActivityDuration(200);
            calculator.NotifyActivityDuration(60);
            calculator.NotifyActivityDuration(20);
            calculator.NotifyActivityDuration(20);
            calculator.NotifyActivityDuration(50);
            calculator.NotifyActivityDuration(15);

            Assert.IsTrue(results.Count == 1);
            Assert.AreEqual(results[0].SumAverageTicks, 56L);
            Assert.AreEqual(results[0].SumMaxTicks, 200);
            Assert.AreEqual(results[0].SumMinTicks, 10);
        }

        [TestMethod]
        public void TestActivityDurationMeasureTimeBlocks()
        {
            List<ActivityDurationResult> results = new List<ActivityDurationResult>();

            ActivityDurationCalculator calculator = new ActivityDurationCalculator(string.Empty, 10);
            calculator.ActivityDurationCalculated += (sender, eArgs) => results.Add(eArgs.ActivityDuration);

            for (int loop = 0; loop < 15; loop++)
            {
                using (calculator.BeginMeasureActivityDuration())
                {
                    Thread.Sleep(5);
                }
            }

            Assert.IsTrue(results.Count == 2);
            Assert.IsTrue((results[0].SumAverage.TotalMilliseconds > 3.0) &&
                          (results[0].SumAverage.TotalMilliseconds < 7.0));
        }

        [TestMethod]
        public void TestActivityDurationCalculatorTrigger()
        {
            List<ActivityDurationResult> results = new List<ActivityDurationResult>();

            ActivityDurationCalculator calculator = new ActivityDurationCalculator(string.Empty, 10);
            calculator.ActivityDurationCalculated += (sender, eArgs) => results.Add(eArgs.ActivityDuration);

            calculator.NotifyActivityDuration(10);
            calculator.NotifyActivityDuration(100);
            calculator.NotifyActivityDuration(50);
            calculator.NotifyActivityDuration(40);
            calculator.NotifyActivityDuration(200);
            calculator.NotifyActivityDuration(60);
            calculator.NotifyActivityDuration(20);

            Assert.IsFalse(results.Count > 0);
        }
    }
}

using System;

namespace RK.Common.Util
{
    public class ActivityDurationResult
    {
        public ActivityDurationCalculator Calculator { get; set; }

        public long SumMaxTicks { get; set; }

        public long SumMinTicks { get; set; }

        public long SumAverageTicks { get; set; }

        public string Name
        {
            get { return this.Calculator.Name; }
        }

        public TimeSpan SumMax
        {
            get { return TimeSpan.FromTicks(SumMaxTicks); }
        }

        public long SumMaxMS
        {
            get { return (long)Math.Round(this.SumMax.TotalMilliseconds); }
        }

        public TimeSpan SumMin
        {
            get { return TimeSpan.FromTicks(SumMinTicks); }
        }

        public long SumMinMS
        {
            get { return (long)Math.Round(this.SumMin.TotalMilliseconds); }
        }

        public TimeSpan SumAverage
        {
            get { return TimeSpan.FromTicks(SumAverageTicks); }
        }

        public long SumAverageMS
        {
            get { return (long)Math.Round(this.SumAverage.TotalMilliseconds); }
        }

        public int Fps
        {
            get
            {
                if (this.SumAverageTicks == 0) { return 0; }
                return (int)(10000000L / this.SumAverageTicks);
            }
        }
    }
}
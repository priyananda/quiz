using System;

namespace RK.Common.Util
{
    public class ActivityCountPerTimeunitResult
    {
        private ActivityCountPerTimeunitCalculator m_calculator;
        private DateTime[] m_timeStamps;

        public ActivityCountPerTimeunitResult(ActivityCountPerTimeunitCalculator calculator, DateTime[] timeStamps)
        {
            m_calculator = calculator;
            m_timeStamps = timeStamps;
        }

        /// <summary>
        /// Gets total count of items per timeunit (average value).
        /// </summary>
        /// <param name="timeUnit">The time unit for which to query the result.</param>
        public double GetItemCountPerTimeunit(TimeSpan timeUnit)
        {
            if (m_timeStamps.Length == 0) { return 0.0; }

            DateTime firstTimeStamp = m_timeStamps[0];
            DateTime lastTimeStamp = m_timeStamps[m_timeStamps.Length - 1];

            //Make some checks here
            if (lastTimeStamp <= firstTimeStamp) { return double.NaN; }
            if (m_timeStamps.Length < 2) { return double.NaN; }

            //Find out how many timeunits are touched by local array
            int timespanOccurencesFull = 0;
            DateTime currentTimeStamp = firstTimeStamp;
            TimeSpan differenceToLast = TimeSpan.Zero;
            while (currentTimeStamp < lastTimeStamp)
            {
                timespanOccurencesFull++;
                differenceToLast = lastTimeStamp - currentTimeStamp;
                currentTimeStamp += timeUnit;
            }
            timespanOccurencesFull--;
            double timeSpanOccurences = (double)timespanOccurencesFull + ((double)differenceToLast.Ticks / (double)timeUnit.Ticks);

            //Now calculate final result
            return (double)m_timeStamps.Length / timeSpanOccurences;
        }

        /// <summary>
        /// Gets the name of the activity.
        /// </summary>
        public string Name
        {
            get { return m_calculator.Name; }
        }

        public double ItemsPerSecond
        {
            get { return GetItemCountPerTimeunit(new TimeSpan(0, 0, 1)); }
        }

        public double ItemsPerMinute
        {
            get { return GetItemCountPerTimeunit(new TimeSpan(0, 1, 0)); }
        }

        public double ItemsPerHour
        {
            get { return GetItemCountPerTimeunit(new TimeSpan(1, 0, 0)); }
        }
    }
}
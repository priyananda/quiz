using System;

namespace RK.Common.Util
{
    public class ActivityDurationCalculator
    {
        //Values used for calculation
        private string m_calculatorName;
        private long[] m_lastDurationItems;
        private int m_lastDurationIndex;

        public event EventHandler<ActivityDurationCalculatedArgs> ActivityDurationCalculated;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityDurationCalculator" /> class.
        /// </summary>
        public ActivityDurationCalculator(string calculatorName)
            : this(calculatorName, 20)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityDurationCalculator" /> class.
        /// </summary>
        public ActivityDurationCalculator(string calculatorName, int durationBlockSize)
        {
            if (durationBlockSize < 10) { throw new CommonLibraryException("DurationBlockSize can not be smaller than 10!"); }

            m_calculatorName = calculatorName;
            m_lastDurationItems = new long[durationBlockSize];
            m_lastDurationIndex = 0;
        }

        /// <summary>
        /// Notifies a new duration.
        /// </summary>
        /// <param name="durationTicks">The ticks measured.</param>
        public void NotifyActivityDuration(long durationTicks)
        {
            m_lastDurationItems[m_lastDurationIndex] = durationTicks;
            m_lastDurationIndex++;

            //Check for finished block
            if (m_lastDurationIndex >= m_lastDurationItems.Length)
            {
                ActivityDurationResult resultObject = new ActivityDurationResult();

                //Calculate sums
                long sumMax = long.MinValue;
                long sumMin = long.MaxValue;
                long sumTotal = 0;
                for (int loop = 0; loop < m_lastDurationItems.Length; loop++)
                {
                    long actDuration = m_lastDurationItems[loop];

                    sumTotal += actDuration;
                    if (actDuration > sumMax) { sumMax = actDuration; }
                    if (actDuration < sumMin) { sumMin = actDuration; }
                }
                resultObject.Calculator = this;
                resultObject.SumAverageTicks = sumTotal / (long)m_lastDurationItems.Length;
                resultObject.SumMaxTicks = sumMax;
                resultObject.SumMinTicks = sumMin;

                //Update state values
                m_lastDurationIndex = m_lastDurationItems.Length / 2;
                Array.Copy(m_lastDurationItems, m_lastDurationIndex, m_lastDurationItems, 0, m_lastDurationItems.Length - m_lastDurationIndex);

                this.ActivityDurationCalculated.Raise(this, new ActivityDurationCalculatedArgs(resultObject));
            }
        }

        /// <summary>
        /// Begins a new activity block. This activity is registered after a call to Dispose.
        /// </summary>
        public IDisposable BeginMeasureActivityDuration()
        {
            long startTimeStamp = DateTime.UtcNow.Ticks;
            return new DummyDisposable(() => NotifyActivityDuration(DateTime.UtcNow.Ticks - startTimeStamp));
        }

        /// <summary>
        /// Gets the name of this calculator.
        /// </summary>
        public string Name
        {
            get { return m_calculatorName; }
        }
    }
}

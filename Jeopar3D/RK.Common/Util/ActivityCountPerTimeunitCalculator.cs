using System;

namespace RK.Common.Util
{
    public class ActivityCountPerTimeunitCalculator
    {
        private string m_name;
        private DateTime[] m_timestamps;
        private int m_currentArrayIndex;
        private int m_itemCountToMeasure;
        private Func<DateTime> m_timeStampGetter;

        public event EventHandler<ActivityCountPerTimeunitCalculatedArgs> ActivityCountCalculated;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityCountPerTimeunitCalculator" /> class.
        /// </summary>
        /// <param name="name">The name of the calculator.</param>
        public ActivityCountPerTimeunitCalculator(string name)
            : this(name, 100, () => DateTime.UtcNow)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityCountPerTimeunitCalculator" /> class.
        /// </summary>
        /// <param name="name">The name of the calculator.</param>
        /// <param name="itemCountToMeasure">The item count to measure.</param>
        /// <param name="timeStampGetter">The time stamp getter.</param>
        public ActivityCountPerTimeunitCalculator(string name, int itemCountToMeasure, Func<DateTime> timeStampGetter)
        {
            m_name = name;
            m_timestamps = new DateTime[itemCountToMeasure];
            m_itemCountToMeasure = itemCountToMeasure;
            m_currentArrayIndex = 0;

            //Apply timestamp getter
            m_timeStampGetter = timeStampGetter;
            if (m_timeStampGetter == null) { m_timeStampGetter = () => DateTime.UtcNow; }
        }

        /// <summary>
        /// Notifies a done activity.
        /// </summary>
        public void NotifyDoneActivity()
        {
            m_timestamps[m_currentArrayIndex] = m_timeStampGetter();
            m_currentArrayIndex++;

            //Handle maximum count of items reached
            if (m_currentArrayIndex >= m_itemCountToMeasure)
            {
                //Create the result object
                ActivityCountPerTimeunitResult actResult = new ActivityCountPerTimeunitResult(
                    this,
                    m_timestamps.Clone() as DateTime[]);

                //Update state values
                m_currentArrayIndex = m_timestamps.Length / 2;
                Array.Copy(m_timestamps, m_currentArrayIndex, m_timestamps, 0, m_timestamps.Length - m_currentArrayIndex);

                //Fire calculated event
                ActivityCountCalculated.Raise(this, new ActivityCountPerTimeunitCalculatedArgs(actResult));
            }
        }

        /// <summary>
        /// Gets the name of this calculator.
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace RK.Common.Util
{
    public class ActivityPerformanceValueContainer
    {
        private SynchronizationContext m_syncContext;

        //Collections for ActivityDurationCalculators
        private Dictionary<string, ActivityDurationCalculator> m_durationCalculators;

        private ObservableCollection<ActivityDurationResult> m_collectedDurationItems;

        //Collections for ActivityCountPerTimeunitCalculators
        private Dictionary<string, ActivityCountPerTimeunitCalculator> m_countPerTimeunitCalculators;

        private ObservableCollection<ActivityCountPerTimeunitResult> m_collectedCountPerTimeunitItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityPerformanceValueContainer" /> class.
        /// </summary>
        public ActivityPerformanceValueContainer()
        {
            m_durationCalculators = new Dictionary<string, ActivityDurationCalculator>();
            m_collectedDurationItems = new ObservableCollection<ActivityDurationResult>();

            m_countPerTimeunitCalculators = new Dictionary<string, ActivityCountPerTimeunitCalculator>();
            m_collectedCountPerTimeunitItems = new ObservableCollection<ActivityCountPerTimeunitResult>();
        }

        /// <summary>
        /// Appends the given ActivityCountPerTimeunitCalculator object.
        /// </summary>
        /// <param name="calculatorToAdd">The object to add.</param>
        public void AppendActivityCountPerTimeunitCalculator(ActivityCountPerTimeunitCalculator calculatorToAdd)
        {
            if (m_countPerTimeunitCalculators.ContainsKey(calculatorToAdd.Name))
            {
                throw new CommonLibraryException("There is already a ActivityCountPerTimeunitCalculator with the name " + calculatorToAdd.Name + "!");
            }
            m_countPerTimeunitCalculators[calculatorToAdd.Name] = calculatorToAdd;

            //Set result structure index to initial state
            int indexOfCalculator = -1;

            //Add a new dummy measure result and remember the index on which it is
            m_syncContext.PostAlsoIfNull(() =>
                {
                    indexOfCalculator = m_collectedCountPerTimeunitItems.Count;
                    m_collectedCountPerTimeunitItems.Add(new ActivityCountPerTimeunitResult(calculatorToAdd, new DateTime[0]));
                },
                ActionIfSyncContextIsNull.InvokeSynchronous);

            //Handle activity count result
            calculatorToAdd.ActivityCountCalculated += (sender, eArgs) =>
            {
                m_syncContext.PostAlsoIfNull(() =>
                    {
                        if (indexOfCalculator >= 0)
                        {
                            m_collectedCountPerTimeunitItems[indexOfCalculator] = eArgs.ActivityCount;
                        }
                    },
                    ActionIfSyncContextIsNull.InvokeSynchronous);
            };
        }

        /// <summary>
        /// Appends the given ActivityDurationCalculator object.
        /// </summary>
        /// <param name="calculatorToAdd">The object to add.</param>
        public void AppendActivityDurationCalculator(ActivityDurationCalculator calculatorToAdd)
        {
            if (m_durationCalculators.ContainsKey(calculatorToAdd.Name))
            {
                throw new CommonLibraryException("There is already a ActivityDurationCalculator with the name " + calculatorToAdd.Name + "!");
            }
            m_durationCalculators[calculatorToAdd.Name] = calculatorToAdd;

            //Set result structure index to initial state
            int indexOfCalculator = -1;

            //Add a new dummy measure result and remember the index on which it is
            m_syncContext.PostAlsoIfNull(() =>
                {
                    indexOfCalculator = m_collectedDurationItems.Count;
                    m_collectedDurationItems.Add(new ActivityDurationResult() { Calculator = calculatorToAdd });
                },
                ActionIfSyncContextIsNull.InvokeSynchronous);

            //Handle activity duration calculated event
            calculatorToAdd.ActivityDurationCalculated += (sender, eArgs) =>
            {
                m_syncContext.PostAlsoIfNull(() =>
                    {
                        if (indexOfCalculator >= 0)
                        {
                            m_collectedDurationItems[indexOfCalculator] = eArgs.ActivityDuration;
                        }
                    },
                    ActionIfSyncContextIsNull.InvokeSynchronous);
            };
        }

        public ActivityCountPerTimeunitCalculator GetCountPerTimeunitCalculator(string activityName)
        {
            if (m_countPerTimeunitCalculators.ContainsKey(activityName)) { return m_countPerTimeunitCalculators[activityName]; }
            else
            {
                ActivityCountPerTimeunitCalculator result = new ActivityCountPerTimeunitCalculator(activityName);
                AppendActivityCountPerTimeunitCalculator(result);
                return result;
            }
        }

        /// <summary>
        /// Gets the duration calculator for the given activity name.
        /// </summary>
        /// <param name="activityName">The name of the activity to get.</param>
        public ActivityDurationCalculator GetDurationCalculator(string activityName)
        {
            if (m_durationCalculators.ContainsKey(activityName)) { return m_durationCalculators[activityName]; }
            else
            {
                //Create and register the new duration calculator
                ActivityDurationCalculator result = new ActivityDurationCalculator(activityName);
                AppendActivityDurationCalculator(result);
                return result;
            }
        }

        /// <summary>
        /// Notifies a done activity.
        /// </summary>
        /// <param name="activityName">The activity that has been done.</param>
        public void NotifyDoneActivity(string activityName)
        {
            ActivityCountPerTimeunitCalculator calculator = GetCountPerTimeunitCalculator(activityName);
            calculator.NotifyDoneActivity();
        }

        /// <summary>
        /// Starts measuring an activity.
        /// </summary>
        /// <param name="activityName">The name of the activity to measure.</param>
        public IDisposable BeginMeasureActivityDuration(string activityName)
        {
            ActivityDurationCalculator calculator = GetDurationCalculator(activityName);
            return calculator.BeginMeasureActivityDuration();
        }

        /// <summary>
        /// Measures the duration of the given activity.
        /// </summary>
        /// <param name="activityName">The name of the activity.</param>
        /// <param name="activity">The activity to measure.</param>
        public void MeasureActivityDuration(string activityName, Action activity)
        {
            using (var measureBlock = BeginMeasureActivityDuration(activityName))
            {
                activity();
            }
        }

        /// <summary>
        /// Gets or sets the SynchronizationContext used for updating local collections.
        /// </summary>
        public SynchronizationContext SynchronizationContext
        {
            get { return m_syncContext; }
            set
            {
                m_syncContext = value;
            }
        }

        /// <summary>
        /// Gets a collection containing all current collected duration items.
        /// (older items get thrown away)
        /// </summary>
        public ObservableCollection<ActivityDurationResult> CollectedDurationItems
        {
            get { return m_collectedDurationItems; }
        }

        /// <summary>
        /// Gets a collection containing all current collected ActivityCountPerTimeunit results.
        /// (older items get thrown away)
        /// </summary>
        public ObservableCollection<ActivityCountPerTimeunitResult> CollectedActivityCountPerTimeunitItems
        {
            get { return m_collectedCountPerTimeunitItems; }
        }
    }
}
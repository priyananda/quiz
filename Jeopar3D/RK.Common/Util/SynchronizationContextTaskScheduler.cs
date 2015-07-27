using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RK.Common.Util
{
    public class SynchronizationContextTaskScheduler
    {
        private bool m_isTaskExecuting;
        private Dispatcher m_mainDispatcher;
        private SynchronizationContext m_mainSyncContext;
        private List<IEnumerator<SyncTaskContinuation>> m_tasks;
        private int m_maxSingleTaskDuration;
        private int m_waitTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizationContextTaskScheduler" /> class.
        /// </summary>
        public SynchronizationContextTaskScheduler(int maxSingleTaskDuration = 100, int waitTime = 100)
        {
            m_mainSyncContext = SynchronizationContext.Current;
            m_mainDispatcher = Dispatcher.CurrentDispatcher;

            m_tasks = new List<IEnumerator<SyncTaskContinuation>>();
            m_isTaskExecuting = false;

            m_maxSingleTaskDuration = maxSingleTaskDuration;
            m_waitTime = waitTime;
        }

        /// <summary>
        /// Cancels all current tasks.
        /// </summary>
        public void CancelTask()
        {
            //Check current thread
            CheckCurrentThread();

            //Clear task list
            m_tasks.Clear();
        }

        /// <summary>
        /// Executes the given task.
        /// </summary>
        /// <param name="taskEnumeration">The enumerable containing the task.</param>
        public void ExecuteTask(IEnumerable<SyncTaskContinuation> taskEnumeration)
        {
            //Check current thread
            CheckCurrentThread();

            //Append given task
            m_tasks.Add(taskEnumeration.GetEnumerator());

            //Trigger task loop
            TriggerTaskLoop();
        }

        /// <summary>
        /// Checks whether current calling thread is the correct one.
        /// </summary>
        private void CheckCurrentThread()
        {
            if (m_mainDispatcher != null)
            {
                if (Dispatcher.CurrentDispatcher != m_mainDispatcher)
                {
                    throw new ApplicationException("Call 'ExecuteTask' from another thread!");
                }
            }
            else if (SynchronizationContext.Current != m_mainSyncContext)
            {
                throw new ApplicationException("Call 'ExecuteTask' from another thread!");
            }
        }
        /// <summary>
        /// Triggers current task loop.
        /// </summary>
        private void TriggerTaskLoop()
        {
            if (!m_isTaskExecuting)
            {
                m_isTaskExecuting = true;
                m_mainSyncContext.InvokeDelayedWhile(
                    () => m_tasks.Count > 0,
                    () =>
                    {
                        //Execute all tasks
                        List<IEnumerator<SyncTaskContinuation>> tasksToDelete = new List<IEnumerator<SyncTaskContinuation>>();
                        foreach (IEnumerator<SyncTaskContinuation> actTask in m_tasks)
                        {
                            Stopwatch actStopwatch = new Stopwatch();
                            actStopwatch.Start();
                            while (actStopwatch.ElapsedMilliseconds < m_maxSingleTaskDuration)
                            {
                                bool nextAvailable = actTask.MoveNext();

                                if (!nextAvailable)
                                {
                                    //Task finished
                                    tasksToDelete.Add(actTask);
                                    break;
                                }
                                else if (actTask.Current == SyncTaskContinuation.Break)
                                {
                                    //Task breaks it self
                                    tasksToDelete.Add(actTask);
                                    break;
                                }
                                else if (actTask.Current == SyncTaskContinuation.TryContinue)
                                {
                                    //Nothing to do here.. should be standard
                                }
                                else if (actTask.Current == SyncTaskContinuation.WaitTick)
                                {
                                    //Task wants a little wait time.. gets call on next loop pass again
                                    continue;
                                }
                            }
                        }

                        //Remove all that need removing
                        foreach (var actTask in tasksToDelete)
                        {
                            actTask.Dispose();
                            m_tasks.Remove(actTask);
                        }
                    },
                    TimeSpan.FromMilliseconds(m_waitTime),
                    () => m_isTaskExecuting = false,
                    InvokeDelayedMode.FixedWaitTime);
            }
        }
    }
}

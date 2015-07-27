using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RK.Common.Util
{
    public class ObjectThread
    {
        private const int STANDARD_HEARTBEAT = 500;

        //Members for thread configuration
        private string m_name;

        private int m_heartBeat;

        //Members for thread runtime
        private volatile ObjectThreadState m_currentState;

        //private Stopwatch m_stopWatch;
        private ObjectThreadTimer m_timer;

        //Threading resources
        private ObjectThreadSynchronizationContext m_syncContext;

        private ConcurrentQueue<Action> m_taskQueue;
        private SemaphoreSlim m_mainLoopSynchronizeObject;

        /// <summary>
        /// Called when the thread ist starting.
        /// </summary>
        public event EventHandler Starting;

        /// <summary>
        /// Called on each heartbeat.
        /// </summary>
        public event EventHandler Tick;

        /// <summary>
        /// Called when the thread is stopping.
        /// </summary>
        public event EventHandler Stopping;

        /// <summary>
        /// Called when an unhandled exception occurred.
        /// </summary>
        public event ObjectThreadExceptionEventHandler ThreadException;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectThread"/> class.
        /// </summary>
        public ObjectThread()
            : this(string.Empty, STANDARD_HEARTBEAT)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectThread"/> class.
        /// </summary>
        /// <param name="name">The name of the generated thread.</param>
        protected ObjectThread(string name, int heartBeat)
        {
            m_taskQueue = new ConcurrentQueue<Action>();
            m_mainLoopSynchronizeObject = new SemaphoreSlim(1);

            m_name = name;
            m_heartBeat = heartBeat;

            m_timer = new ObjectThreadTimer();
        }

        /// <summary>
        /// Starts the thread.
        /// </summary>
        public void Start()
        {
            if (m_currentState != ObjectThreadState.None) { throw new InvalidOperationException("Unable to start thread: Illegal state: " + m_currentState.ToString() + "!"); }

            //Ensure that one single pass of the main loop is made at once
            m_mainLoopSynchronizeObject.Release();

            //Go into starting state
            m_currentState = ObjectThreadState.Starting;
            Task.Factory.StartNew(ObjectThreadMainMethod);
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            if (m_currentState != ObjectThreadState.Running) { throw new InvalidOperationException("Unable to stop thread: Illegal state: " + m_currentState.ToString() + "!"); }
            m_currentState = ObjectThreadState.Stopping;

            //ThreadPool.RegisterWaitForSingleObject()
            Action dummyAction = null;
            while (m_taskQueue.TryDequeue(out dummyAction)) ;

            //Trigger next update
            this.Trigger();
        }

        /// <summary>
        /// Triggers a new heartbeat.
        /// </summary>
        public virtual void Trigger()
        {
            SemaphoreSlim synchronizationObject = m_mainLoopSynchronizeObject;
            if (synchronizationObject != null)
            {
                synchronizationObject.Release();
            }
        }

        /// <summary>
        /// Invokes the given delegate within the thread of this object.
        /// </summary>
        /// <param name="actionToInvoke">The delegate to invoke.</param>
        public Task InvokeAsync(Action actionToInvoke)
        {
            if (actionToInvoke == null) { throw new ArgumentNullException("actionToInvoke"); }

            //Enqueues the given action
            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
            m_taskQueue.Enqueue(() =>
            {
                try
                {
                    actionToInvoke();
                    taskCompletionSource.SetResult(null);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });
            Task result = taskCompletionSource.Task;

            //Triggers the main loop
            this.Trigger();

            //Returns the result
            return result;
        }

        /// <summary>
        /// Thread is starting.
        /// </summary>
        protected virtual void OnStarting(EventArgs eArgs)
        {
            if (Starting != null) { Starting(this, eArgs); }
        }

        /// <summary>
        /// Called on each tick.
        /// </summary>
        protected virtual void OnTick(EventArgs eArgs)
        {
            if (Tick != null) { Tick(this, eArgs); }
        }

        /// <summary>
        /// Called on each occurred exception.
        /// </summary>
        protected virtual void OnThreadException(ObjectThreadExceptionEventArgs eArgs)
        {
            if (ThreadException != null) { ThreadException(this, eArgs); }
        }

        /// <summary>
        /// Thread is stopping.
        /// </summary>
        protected virtual void OnStopping(EventArgs eArgs)
        {
            if (Stopping != null) { Stopping(this, eArgs); }
        }

        /// <summary>
        /// The thread's main method.
        /// </summary>
        private async void ObjectThreadMainMethod()
        {
            try
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                //Set synchronization context for this thread
                m_syncContext = new ObjectThreadSynchronizationContext(this);

                //Notify start process
                try { OnStarting(EventArgs.Empty); }
                catch (Exception ex)
                {
                    OnThreadException(new ObjectThreadExceptionEventArgs(m_currentState, ex));
                    m_currentState = ObjectThreadState.None;
                    return;
                }

                //Run main-thread
                if (m_currentState != ObjectThreadState.None)
                {
                    m_currentState = ObjectThreadState.Running;
                    while (m_currentState == ObjectThreadState.Running)
                    {
                        try
                        {
                            //Wait for next action to perform
                            await m_mainLoopSynchronizeObject.WaitAsync(m_heartBeat);

                            //Measure current time
                            stopWatch.Stop();
                            m_timer.Add(stopWatch.Elapsed);
                            stopWatch.Reset();
                            stopWatch.Start();

                            //Get current taskqueue
                            List<Action> localTaskQueue = new List<Action>();
                            Action dummyAction = null;
                            while (m_taskQueue.TryDequeue(out dummyAction))
                            {
                                localTaskQueue.Add(dummyAction);
                            }

                            //Execute all tasks
                            foreach (Action actTask in localTaskQueue)
                            {
                                try { actTask(); }
                                catch (Exception ex)
                                {
                                    OnThreadException(new ObjectThreadExceptionEventArgs(m_currentState, ex));
                                }
                            }

                            //Perfoms a tick
                            OnTick(EventArgs.Empty);
                        }
                        catch (Exception ex)
                        {
                            OnThreadException(new ObjectThreadExceptionEventArgs(m_currentState, ex));
                        }
                    }

                    //Notify stop process
                    try { OnStopping(EventArgs.Empty); }
                    catch (Exception ex)
                    {
                        OnThreadException(new ObjectThreadExceptionEventArgs(m_currentState, ex));
                    }
                }

                //Reset state to none
                m_currentState = ObjectThreadState.None;

                stopWatch.Stop();
                stopWatch = null;
            }
            catch (Exception ex)
            {
                OnThreadException(new ObjectThreadExceptionEventArgs(m_currentState, ex));
                m_currentState = ObjectThreadState.None;
            }
        }

        /// <summary>
        /// Gets current thread time.
        /// </summary>
        public DateTime ThreadTime
        {
            get { return m_timer.Now; }
        }

        /// <summary>
        /// Gets current timer of the thread.
        /// </summary>
        public ObjectThreadTimer Timer
        {
            get { return m_timer; }
        }

        /// <summary>
        /// Gets the current SynchronizationContext object.
        /// </summary>
        public SynchronizationContext SyncContext
        {
            get { return m_syncContext; }
        }

        /// <summary>
        /// Gets or sets the thread's heartbeat.
        /// </summary>
        protected int HeartBeat
        {
            get { return m_heartBeat; }
            set { m_heartBeat = value; }
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        /// <summary>
        /// Synchronization object for threads within ObjectThread class.
        /// </summary>
        private class ObjectThreadSynchronizationContext : SynchronizationContext
        {
            private ObjectThread m_owner;

            /// <summary>
            /// Initializes a new instance of the <see cref="ObjectThreadSynchronizationContext"/> class.
            /// </summary>
            /// <param name="owner">The owner of this context.</param>
            public ObjectThreadSynchronizationContext(ObjectThread owner)
            {
                m_owner = owner;
            }

            /// <summary>
            /// When overridden in a derived class, dispatches an asynchronous message to a synchronization context.
            /// </summary>
            /// <param name="d">The <see cref="T:System.Threading.SendOrPostCallback"/> delegate to call.</param>
            /// <param name="state">The object passed to the delegate.</param>
            public override void Post(SendOrPostCallback d, object state)
            {
                m_owner.InvokeAsync(() => d(state));
            }

            /// <summary>
            /// When overridden in a derived class, dispatches a synchronous message to a synchronization context.
            /// </summary>
            /// <param name="d">The <see cref="T:System.Threading.SendOrPostCallback"/> delegate to call.</param>
            /// <param name="state">The object passed to the delegate.</param>
            public override void Send(SendOrPostCallback d, object state)
            {
                throw new InvalidOperationException("Synchronous messages not supported on ObjectThreads!");
            }
        }
    }
}
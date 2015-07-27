﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RK.Common.Util
{
    public delegate void ObjectThreadExceptionEventHandler(object sender, ObjectThreadExceptionEventArgs e);

    public enum SyncTaskContinuation
    {
        WaitTick,

        TryContinue,

        Break
    }

    public class ActivityCountPerTimeunitCalculatedArgs : EventArgs
    {
        public ActivityCountPerTimeunitCalculatedArgs(ActivityCountPerTimeunitResult activityCount)
        {
            this.ActivityCount = activityCount;
        }

        public ActivityCountPerTimeunitResult ActivityCount
        {
            get;
            private set;
        }
    }

    public class ActivityDurationCalculatedArgs : EventArgs
    {
        public ActivityDurationCalculatedArgs(ActivityDurationResult durationResult)
        {
            this.ActivityDuration = durationResult;
        }

        public ActivityDurationResult ActivityDuration
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// Enumeration containing all possible states of a ObjectThread object.
    /// </summary>
    public enum ObjectThreadState
    {
        /// <summary>
        /// Threre is no thread created at the moment.
        /// </summary>
        None,

        /// <summary>
        /// The thread is starting.
        /// </summary>
        Starting,

        /// <summary>
        /// The thread is running.
        /// </summary>
        Running,

        /// <summary>
        /// The thread is stopping.
        /// </summary>
        Stopping
    }

    //*************************************************************************
    //*************************************************************************
    //*************************************************************************
    public class ObjectThreadExceptionEventArgs : EventArgs
    {
        private Exception m_innerException;
        private ObjectThreadState m_threadState;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectThreadExceptionEventArgs"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public ObjectThreadExceptionEventArgs(ObjectThreadState threadState, Exception innerException)
        {
            m_innerException = innerException;
            m_threadState = threadState;
        }

        /// <summary>
        /// Gets the occurred exception.
        /// </summary>
        public Exception Exception
        {
            get { return m_innerException; }
        }

        /// <summary>
        /// Gets current state of the thread.
        /// </summary>
        public ObjectThreadState State
        {
            get { return m_threadState; }
        }
    }
}

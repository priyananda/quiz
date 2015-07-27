using System;

namespace RK.Common
{
    public class UnhandledExceptionArgs : EventArgs
    {
        public UnhandledExceptionArgs(Exception ex)
        {
            this.Exception = ex;
        }

        public Exception Exception
        {
            get;
            private set;
        }
    }

    public enum InvokeDelayedMode
    {
        FixedWaitTime,

        EnsuredTimerInterval
    }

    public enum ActionIfSyncContextIsNull
    {
        InvokeSynchronous,

        InvokeUsingNewTask
    }
}
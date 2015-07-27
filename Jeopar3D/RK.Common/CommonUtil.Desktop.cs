using System;
using System.Threading;

namespace RK.Common
{
    public static partial class CommonUtil
    {
        /// <summary>
        /// Gets the attribute object attached to the given type.
        /// </summary>
        /// <typeparam name="T">The type of the attribute</typeparam>
        /// <param name="sourceType"></param>
        public static T GetCustomAttribute<T>(Type sourceType)
            where T : Attribute
        {
            T result = Attribute.GetCustomAttribute(sourceType, typeof(T)) as T;
            if (result == null) { throw new InvalidOperationException("Attribute " + typeof(T).FullName + " not found on type " + sourceType.FullName + "!"); }
            return result;
        }

        /// <summary>
        /// Executes the given action after the given amount of time.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="delayTime">The delay time.</param>
        public static void InvokeDelayed(Action action, TimeSpan delayTime)
        {
            //Gets current Synchronization context
            SynchronizationContext syncContext = SynchronizationContext.Current;
            if (syncContext == null)
            {
                throw new InvalidOperationException("No SynchronizationContext is available on current thread!");
            }
            if (syncContext.GetType() == typeof(SynchronizationContext))
            {
                throw new InvalidOperationException("This method is not available on default synchronization context!");
            }

            //Start a timer object wich fires an event after delay time
            System.Threading.Timer delayTimer = null;
            delayTimer = new System.Threading.Timer(
                new TimerCallback((arg) =>
                {
                    //Stop the delay timer
                    delayTimer.Dispose();

                    //Check if the control is still available
                    syncContext.Send(new SendOrPostCallback((innerArg) =>
                    {
                        //Handle exception by global exception handler
                        action();
                    }), null);
                }),
                null,
                delayTime,
                TimeSpan.FromDays(5));
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

//using Windows.System.Threading;
//using Windows.UI.Xaml;

namespace RK.Common
{
    public static partial class CommonExtensions
    {
        public static void Raise(this EventHandler eventHandler, object sender, EventArgs eventArgs)
        {
            if (eventHandler != null) { eventHandler(sender, eventArgs); }
        }

        public static void Raise<T>(this EventHandler<T> eventHandler, object sender, T eventArgs)
            where T : EventArgs
        {
            if (eventHandler != null) { eventHandler(sender, eventArgs); }
        }

        public static IEnumerable<TTarget> ConvertAllTo<TSource, TTarget>(this IEnumerable<TSource> enumeration, Func<TSource, TTarget> converter)
        {
            foreach (TSource actEnumElement in enumeration)
            {
                yield return converter(actEnumElement);
            }
        }

        /// <summary>
        /// Posts the given action to the given synchronization context also if it is null.
        /// If it is null, then a new task will be started.
        /// </summary>
        /// <param name="syncContext">The context to send the action to.</param>
        /// <param name="actionToSend">The action to send.</param>
        public static void PostAlsoIfNull(this SynchronizationContext syncContext, Action actionToSend, ActionIfSyncContextIsNull actionIfNull)
        {
            if (syncContext != null) { syncContext.Post((arg) => actionToSend(), null); }
            else
            {
                switch (actionIfNull)
                {
                    case ActionIfSyncContextIsNull.InvokeSynchronous:
                        actionToSend();
                        break;

                    case ActionIfSyncContextIsNull.InvokeUsingNewTask:
                        Task.Factory.StartNew(actionToSend);
                        break;

                    default:
                        throw new ArgumentException("actionIfNull", "Action " + actionIfNull + " unknown!");
                }
            }
        }

        /// <summary>
        /// Reads all bytes from the given stream.
        /// </summary>
        /// <param name="inStream">The stream to read all the data from.</param>
        public static byte[] ReadAllBytes(this Stream inStream)
        {
            if (inStream.Length > Int32.MaxValue) { throw new NotSupportedException("Given stream is to big!"); }

            byte[] result = new byte[inStream.Length];
            inStream.Read(result, 0, (int)inStream.Length);
            return result;
        }

        /// <summary>
        /// Gets the given integer value as seconds in TimeSpan format.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        public static TimeSpan Seconds(this int value)
        {
            return new TimeSpan(0, 0, value);
        }

        /// <summary>
        /// Gets the given integer value as minutes in TimeSpan format.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        public static TimeSpan Minutes(this int value)
        {
            return new TimeSpan(0, value, 0);
        }

        /// <summary>
        /// Gets the given integer value as hours in TimeSpan format.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        public static TimeSpan Hours(this int value)
        {
            return new TimeSpan(value, 0, 0);
        }

        /// <summary>
        /// Gets the given integer value as milliseconds in TimeSpan format.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        public static TimeSpan Milliseconds(this int value)
        {
            return new TimeSpan(0, 0, 0, 0, value);
        }

        /// <summary>
        /// Gets the given integer value as days in TimeSpan format.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        public static TimeSpan Days(this int value)
        {
            return new TimeSpan(value, 0, 0, 0);
        }

        /// <summary>
        /// Gets the timestamp the given time ago.
        /// </summary>
        public static DateTime Ago(this TimeSpan timeSpan)
        {
            return DateTime.Now.Subtract(timeSpan);
        }

        /// <summary>
        /// Is the given string null or empty?
        /// </summary>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Is the given string null, empty or just whitespace?
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
    }
}
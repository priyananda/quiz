using System;
using System.Collections.Generic;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using System.Threading.Tasks;
using System.IO;
#endif

namespace RK.Common
{
#if WINRT

    public static partial class CommonExtensions
    {
        private static Dictionary<DispatcherTimer, object> s_threadPoolDict;

        static CommonExtensions()
        {
            s_threadPoolDict = new Dictionary<DispatcherTimer, object>();
        }

        /// <summary>
        /// A safe register on the tick event on the given DispatcherTimer. If a exception is raised, it gets redirected to CommonUtil.UnhandledException.
        /// </summary>
        /// <param name="dispatcherTimer"></param>
        /// <param name="tickEventHandler"></param>
        public static void RegisterSafeTick(this DispatcherTimer dispatcherTimer, EventHandler<object> tickEventHandler)
        {
            dispatcherTimer.Tick += (sender, args) =>
            {
                try
                {
                    tickEventHandler(sender, args);
                }
                catch (Exception ex)
                {
                    CommonUtil.RaiseUnhandledException(dispatcherTimer, ex);
                }
            };
        }

        /// <summary>
        /// Is there any element of the given type in the given collection?
        /// </summary>
        /// <param name="elementCollection">The element collection to check all elements in.</param>
        /// <param name="controlType">The control type to look for.</param>
        public static bool ContainsAny(this UIElementCollection elementCollection, Type controlType)
        {
            foreach (var actElement in elementCollection)
            {
                if (actElement.GetType() == controlType) { return true; }
            }
            return false;
        }

        /// <summary>
        /// Removes all controls of the given type from the given element collection.
        /// </summary>
        /// <param name="elementCollection">The collection to modify.</param>
        /// <param name="controlType">Control type to remove.</param>
        public static void RemoveAll(this UIElementCollection elementCollection, Type controlType)
        {
            for (int loop = 0; loop < elementCollection.Count; loop++)
            {
                if (elementCollection[loop].GetType() == controlType)
                {
                    elementCollection.RemoveAt(loop);
                    loop--;
                }
            }
        }

        /// <summary>
        /// Gets the file with the given name or returns null if the file does not exist.
        /// </summary>
        /// <param name="storageFolder">The folder from which to get the file.</param>
        /// <param name="fileName">The name of the file.</param>
        public static async Task<StorageFile> GetOrReturnNull(this StorageFolder storageFolder, string fileName)
        {
            try
            {
                return await storageFolder.GetFileAsync(fileName);
            }
            catch (FileNotFoundException) { }

            return null;
        }

        /// <summary>
        /// Executes the given action after the given amount of time.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="delayTime">The delay time.</param>
        /// <param name="control">The used for the incoke call.</param>
        public static void InvokeDelayed(this UIElement control, Action action, TimeSpan delayTime)
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = delayTime;
            dispatcherTimer.RegisterSafeTick((sender, eArgs) =>
            {
                dispatcherTimer.Stop();
                s_threadPoolDict.Remove(dispatcherTimer);

                action();
            });

            s_threadPoolDict[dispatcherTimer] = null;
            dispatcherTimer.Start();
        }

        /// <summary>
        /// Executes the given method more times until "condition" returns false.
        /// </summary>
        /// <param name="control">The control used for dispatching.</param>
        /// <param name="condition">The condition to be checked before each call of <paramref name="loopAction"/>.</param>
        /// <param name="loopAction">The action to perform more times.</param>
        /// <param name="delayTime">The interval between callings of <paramref name="loopAction"/>.</param>
        public static void InvokeDelayedWhile(this UIElement control, Func<bool> condition, Action loopAction, TimeSpan delayTime)
        {
            InvokeDelayedWhile(control, condition, loopAction, delayTime, null);
        }

        /// <summary>
        /// Executes the given method more times until "condition" returns false.
        /// </summary>
        /// <param name="control">The control used for dispatching.</param>
        /// <param name="condition">The condition to be checked before each call of <paramref name="loopAction"/>.</param>
        /// <param name="loopAction">The action to perform more times.</param>
        /// <param name="delayTime">The interval between callings of <paramref name="loopAction"/>.</param>
        /// <param name="finishingAction">This action is called once when <paramref name="condition"/> returns false.</param>
        public static void InvokeDelayedWhile(this UIElement control, Func<bool> condition, Action loopAction, TimeSpan delayTime, Action finishingAction)
        {
            InvokeDelayedWhile(control, condition, loopAction, delayTime, null, InvokeDelayedMode.FixedWaitTime);
        }

        /// <summary>
        /// Executes the given method more times until "condition" returns false.
        /// </summary>
        /// <param name="control">The control used for dispatching.</param>
        /// <param name="condition">The condition to be checked before each call of <paramref name="loopAction"/>.</param>
        /// <param name="loopAction">The action to perform more times.</param>
        /// <param name="delayTime">The interval between callings of <paramref name="loopAction"/>.</param>
        /// <param name="finishingAction">This action is called once when <paramref name="condition"/> returns false.</param>
        public static void InvokeDelayedWhile(this UIElement control, Func<bool> condition, Action loopAction, TimeSpan delayTime, Action finishingAction, InvokeDelayedMode mode)
        {
            Func<int, TimeSpan> getDelayTimeFunc = null;
            switch (mode)
            {
                case InvokeDelayedMode.EnsuredTimerInterval:
                    getDelayTimeFunc = (neededTime) =>
                    {
                        int remainingMilliseconds = (int)delayTime.TotalMilliseconds - neededTime;

                        if (remainingMilliseconds < 5) { return TimeSpan.FromMilliseconds(5.0); }
                        else { return TimeSpan.FromMilliseconds(remainingMilliseconds); }
                    };
                    break;

                case InvokeDelayedMode.FixedWaitTime:
                    getDelayTimeFunc = (neededTime) => delayTime;
                    break;
            }

            Action outerLoopAction = null;
            outerLoopAction = () =>
            {
                //Stopwatch stopWatch = new Stopwatch();
                //stopWatch.Start();
                DateTime startTime = DateTime.UtcNow;
                if (condition())
                {
                    loopAction();

                    //stopWatch.Stop();

                    InvokeDelayed(control, outerLoopAction, getDelayTimeFunc((int)(DateTime.UtcNow - startTime).TotalMilliseconds));//(int)stopWatch.ElapsedMilliseconds));
                }
                else
                {
                    if (finishingAction != null) { finishingAction(); }
                }
            };
            InvokeDelayed(control, outerLoopAction, delayTime);
        }
    }

#endif
}
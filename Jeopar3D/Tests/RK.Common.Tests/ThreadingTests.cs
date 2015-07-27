using Microsoft.VisualStudio.TestTools.UnitTesting;
using RK.Common.Util;

namespace RK.Common.Tests
{
    [TestClass]
    public class ThreadingTests
    {
        //[TestMethod]
        //public void TestObjectThread()
        //{
        //    DummyObjectThread objThread = new DummyObjectThread();
        //    objThread.Start();

        //    Thread.Sleep(3000);
        //    objThread.Trigger();
        //    objThread.Trigger();
        //    Thread.Sleep(200);
        //    objThread.Trigger();
        //    Thread.Sleep(100);
        //    objThread.Trigger();

        //    Thread.Sleep(10000);
        //}

        //public class DummyObjectThread : ObjectThread
        //{
        //    /// <summary>
        //    /// Initializes a new instance of the <see cref="DummyObjectThread" /> class.
        //    /// </summary>
        //    public DummyObjectThread()
        //        : base("Dummy", 1000)
        //    {
        //        this.HeartBeat = 1000;
        //    }

        //    protected override void OnTick(EventArgs eArgs)
        //    {
        //        base.OnTick(eArgs);

        //        Debug.WriteLine("Tick on " + DateTime.Now.TimeOfDay.TotalMilliseconds.ToString("N0"));
        //    }
        //}
    }
}
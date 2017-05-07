namespace Michonne.Tests
{
    using System;
    using System.Threading;
    using Implementation;
    using NFluent;
    using NUnit.Framework;

    [TestFixture]
    public class ThreadUnitOfExecutionTests
    {
        [Test]
        public void Should_Not_Leak_Threads()
        {
            var unit = TestHelpers.GetThread();
            var done = false;

            unit.Dispatch(() =>
            {
                lock (this)
                {
                    done = true;
                    Monitor.Pulse(this);
                }
            });

            lock (this)
            {
                if (!done)
                {
                    Monitor.Wait(this, 500);
                }
            }
            Check.That(done).IsTrue();
            var tester = new WeakReference(unit);
            unit = null;
            GC.Collect(2, GCCollectionMode.Forced, true);
            Check.That(tester.IsAlive).IsFalse();
            GC.WaitForPendingFinalizers();
        }
    }
}
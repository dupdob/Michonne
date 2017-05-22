using System;
using System.Collections.Generic;
using System.Text;

namespace Michonne.Sources.Tests
{
    using System.Threading;

    using Michonne.Implementation;

    using NFluent;

    using NUnit.Framework;

    class ThreadedUnit
    {
        [Test]
        public void should_execute_async()
        {
            var runner = TestHelpers.GetThread();
            using (runner)
            {
                int init = Thread.CurrentThread.ManagedThreadId;
                int second = -1;
                object synch = new object();
                bool done = false;
                runner.Dispatch(
                    () =>
                        {
                            second = Thread.CurrentThread.ManagedThreadId;
                            lock (synch)
                            {
                                done = true;
                                Monitor.Pulse(synch);
                            }
                        });
                lock (synch)
                {
                    if (!done) Monitor.Wait(synch, 500);
                }
                Check.That(init).IsDistinctFrom(second);
            }
        }
    }
}

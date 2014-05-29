using System.Threading;
using NFluent;
using NUnit.Framework;

namespace SequencerAiiiight.Tests
{
    [TestFixture]
    public class ContinuationTasksBasedSequencerTests
    {
        private ContinuationTasksBasedSequencer sequencer;

        [SetUp]
        public void Setup()
        {
            this.sequencer = new ContinuationTasksBasedSequencer();
        }

        [Test]
        public void ShouldNotRunActionsConcurrently()
        {
            const int actionCount = 1000 * 1000;

            var dispatchLock = new object();
            var wasRunConcurrently = false;

            for (var actionIndex = 0; actionIndex < actionCount; ++actionIndex)
            {
                this.sequencer.Dispatch(() =>
                {
                    if (!Monitor.TryEnter(dispatchLock))
                    {
                        wasRunConcurrently = true;
                        return;
                    }
                    try
                    {
                        Thread.SpinWait(25);
                    }
                    finally
                    {
                        Monitor.Exit(dispatchLock);
                    }
                });
            }

            WaitForActions();

            Check.That(wasRunConcurrently).IsFalse();
        }

        [Test]
        public void ShouldRunActionsInOrder()
        {
            const int actionCount = 1000 * 1000;

            var dispatchIndex = 0;
            var wasOutOfOrder = false;

            for (var actionIndex = 0; actionIndex < actionCount; ++actionIndex)
            {
                var localActionIndex = actionIndex;
                this.sequencer.Dispatch(() =>
                {
                    if (dispatchIndex != localActionIndex)
                    {
                        wasOutOfOrder = true;
                        return;
                    }
                    ++dispatchIndex;
                });
            }

            var completedSignal = new ManualResetEvent(false);
            this.sequencer.Dispatch(() => completedSignal.Set());
            completedSignal.WaitOne();

            Check.That(wasOutOfOrder).IsFalse();
        }

        private void WaitForActions()
        {
            var flushSignal = new ManualResetEvent(false);
            this.sequencer.Dispatch(() => flushSignal.Set());
            flushSignal.WaitOne();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using SequencerAiiiight.Interfaces;

namespace SequencerAiiiight.Tests
{
    [TestFixture, Explicit("Manual tests")]
    public class PerformanceTests
    {
        [Test]
        public void MeasureThroughput()
        {
            // TestCaseSource or ValueSource would be better but it does not play well with the last R# version

            foreach (var sequencer in GetSequencers())
            {
                MeasureThroughput(sequencer);
            }
        }

        private static void MeasureThroughput(ISequencer sequencer)
        {
            const int actionsCount = 1000 * 1000;

            var stopSignal = new ManualResetEvent(false);
            var stopwatch = Stopwatch.StartNew();
            for (var actionIndex = 0; actionIndex < actionsCount; ++actionIndex)
            {
                sequencer.Dispatch(() => { });
            }
            sequencer.Dispatch(() => stopSignal.Set());

            stopSignal.WaitOne();
            stopwatch.Stop();

            Console.WriteLine("Sequencer: {0,-35}, Throughput: {1,10:N0} action / sec", sequencer.GetType().Name, actionsCount / stopwatch.Elapsed.TotalSeconds);
        }

        private static IEnumerable<ISequencer> GetSequencers()
        {
            yield return new Sequencer(new DotNetThreadPoolDispatcher());
            yield return new ContinuationTasksBasedSequencer();
        }
    }
}
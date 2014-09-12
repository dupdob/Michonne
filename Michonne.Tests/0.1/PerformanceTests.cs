// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="PerformanceTests.cs" company="">
// //   Copyright 2014 Olivier COANET
// //   Licensed under the Apache License, Version 2.0 (the "License");
// //   you may not use this file except in compliance with the License.
// //   You may obtain a copy of the License at
// //       http://www.apache.org/licenses/LICENSE-2.0
// //   Unless required by applicable law or agreed to in writing, software
// //   distributed under the License is distributed on an "AS IS" BASIS,
// //   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// //   See the License for the specific language governing permissions and
// //   limitations under the License.
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace Michonne.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using Michonne.Interfaces;
    using NUnit.Framework;

    // TODO: (TPI) even if microbenchmarks usually sucks ;-) let's use Vance Morrisson based microbenchmark tool here
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
            const int ActionsCount = 1000 * 1000;

            var stopSignal = new ManualResetEvent(false);
            var stopwatch = Stopwatch.StartNew();
            for (var actionIndex = 0; actionIndex < ActionsCount; ++actionIndex)
            {
                sequencer.Dispatch(() => { });
            }

            sequencer.Dispatch(() => stopSignal.Set());
            stopSignal.WaitOne();
            stopwatch.Stop();

            Console.WriteLine("Sequencer: {0,-35}, Throughput: {1,10:N0} actions / sec", sequencer.GetType().Name, ActionsCount / stopwatch.Elapsed.TotalSeconds);
        }

        private static IEnumerable<ISequencer> GetSequencers()
        {
            yield return new Sequencer(new DotNetThreadPoolUnitOfExecution());
            yield return new ContinuationTasksBasedSequencer();
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="SequencerTests.cs" company="No lock... no deadlock" product="Michonne">
//     Copyright 2014 Cyrille DUPUYDAUBY (@Cyrdup), Thomas PIERRAIN (@tpierrain)
//     Licensed under the Apache License, Version 2.0 (the "License");
//     you may not use this file except in compliance with the License.
//     You may obtain a copy of the License at
//         http://www.apache.org/licenses/LICENSE-2.0
//     Unless required by applicable law or agreed to in writing, software
//     distributed under the License is distributed on an "AS IS" BASIS,
//     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//     See the License for the specific language governing permissions and
//     limitations under the License.
//   </copyright>
//   --------------------------------------------------------------------------------------------------------------------
namespace Michonne.Tests
{
    using System.Collections.Generic;
    using System.Threading;

    using Michonne.Implementation;

    using NFluent;

    using NUnit.Framework;

    [TestFixture]
    public class SequencerTests
    {
        #region Constants

        private const int SecondInMsec = 1000;
        private const int ThreeSecondsMax = 3 * SecondInMsec;

        #endregion

        #region Public Methods and Operators

        [Test]
        public void SequencerExecutesTasksInTheOrderOfTheirDispatch()
        {
            var rootDispatcher = new DotNetThreadPoolUnitOfExecution();
            var sequencer = new Sequencer(rootDispatcher);
            const int TasksNumber = 100000;
            var tasksOutput = new List<int>();

            using (var dispatchingFinishedEvent = new AutoResetEvent(false))
            {
                // Dispatches tasks to the sequencer
                for (int i = 0; i < TasksNumber; i++)
                {
                    int antiClosureSideEffectNumber = i;
                    sequencer.Dispatch(() => tasksOutput.Add(antiClosureSideEffectNumber));
                }

                // Indicates the end of the sequence with a final task
                sequencer.Dispatch(() => dispatchingFinishedEvent.Set());

                // Waits for sequence completion
                Check.That(dispatchingFinishedEvent.WaitOne(ThreeSecondsMax)).IsTrue();

                // Checks that everything was properly processed in sequence
                for (int k = 0; k < TasksNumber; k++)
                {
                    Check.That(tasksOutput[k]).IsEqualTo(k);
                }
            }
        }

        [Test]
        public void DoesNotLoseAnyTaskWithTwoWriterThreadsOnTheSameSequencer()
        {
            var rootDispatcher = new DotNetThreadPoolUnitOfExecution();
            var sequencer = new Sequencer(rootDispatcher);
            const int NumberOfWritesPerThread = 10000;
            var tasksOutput = new List<int>();

            using (var unleashThreadsEvent = new AutoResetEvent(false))
            using (var firstWriterFinishedEvent = new AutoResetEvent(false))
            using (var secondWriterFinishedEvent = new AutoResetEvent(false))
            {
                var firstWriter = new Thread(() => this.WriterRoutine(sequencer, unleashThreadsEvent, 0, NumberOfWritesPerThread, tasksOutput, firstWriterFinishedEvent));
                firstWriter.Start();

                var secondWriter = new Thread(() => this.WriterRoutine(sequencer, unleashThreadsEvent, 0, NumberOfWritesPerThread, tasksOutput, secondWriterFinishedEvent));
                secondWriter.Start();

                // ready, set, mark
                unleashThreadsEvent.Set();

                // Waits until the two writers have finished their writes
                Check.That(firstWriterFinishedEvent.WaitOne(2 * ThreeSecondsMax) && secondWriterFinishedEvent.WaitOne(2 * ThreeSecondsMax)).IsTrue();

                // Checks that no write has been missing
                Check.That(tasksOutput).HasSize(2 * NumberOfWritesPerThread);
            }
        }

        private void WriterRoutine(Sequencer sequencer, AutoResetEvent readySetMarkEvent, int firstIndex, int lastIndex, List<int> tasksOutput, AutoResetEvent lastTaskEvent)
        {
            readySetMarkEvent.WaitOne(ThreeSecondsMax);

            for (int i = firstIndex; i < lastIndex; i++)
            {
                int antiClosureSideEffectNumber = i;
                sequencer.Dispatch(() => tasksOutput.Add(antiClosureSideEffectNumber));
            }

            // Indicates the end of the sequence with a final task
            sequencer.Dispatch(() => lastTaskEvent.Set());
        }

        #endregion

        // TODO: test the dispatching concurrency (to check that no task is missing)
    }
}
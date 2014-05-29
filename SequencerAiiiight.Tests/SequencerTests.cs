// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="SequencerTests.cs" company="">
// //   Copyright 2014 Thomas PIERRAIN
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
    using System.Collections.Generic;
    using System.Threading;

    using Michonne;

    using NFluent;

    using NUnit.Framework;

    [TestFixture]
    public class SequencerTests
    {
        #region Constants

        private const int SecondInMsec = 1000;
        private const int ThreeSecondsMax = 3 * SecondInMsec;

        #endregion

        #region Fields

        private AutoResetEvent sequenceFinished;
        private List<int> tasksOutput;

        #endregion

        #region Public Methods and Operators

        [Test]
        public void SequencerExecutesTasksInTheOrderOfTheirDispatch()
        {
            var rootDispatcher = new DotNetThreadPoolDispatcher();
            var sequencer = new Sequencer(rootDispatcher);
            const int TasksNumber = 100000;

            this.tasksOutput = new List<int>(TasksNumber);

            // Dispatches tasks to the sequencer
            for (int i = 0; i < TasksNumber; i++)
            {
                int antiClosureSideEffectNumber = i;
                sequencer.Dispatch(() => this.tasksOutput.Add(antiClosureSideEffectNumber));
            }

            // Indicates the end of the sequence with a final task
            sequencer.Dispatch(() => this.sequenceFinished.Set());

            // Waits for sequence completion
            Check.That(this.sequenceFinished.WaitOne(ThreeSecondsMax)).IsTrue();

            // Checks that everything was properly processed in sequence
            for (int k = 0; k < TasksNumber; k++)
            {
                Check.That(this.tasksOutput[k]).IsEqualTo(k);
            }
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this.sequenceFinished = new AutoResetEvent(false);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            if (this.sequenceFinished != null)
            {
                this.sequenceFinished.Dispose();
                this.sequenceFinished = null;
            }
        }

        #endregion

        // TODO: test the dispatching concurrency (to check that no task is missing)
    }
}
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

        private AutoResetEvent _sequenceFinished;
        private List<int> _tasksOutput;

        #endregion

        #region Public Methods and Operators

        [Test]
        public void SequencerExecutesTasksInTheOrderOfTheirDispatch()
        {
            var rootDispatcher = new DotNetThreadPoolUnitOfExecution();
            var sequencer = new Sequencer(rootDispatcher);
            const int tasksNumber = 100000;

            this._tasksOutput = new List<int>(tasksNumber);

            // Dispatches tasks to the sequencer
            for (int i = 0; i < tasksNumber; i++)
            {
                int antiClosureSideEffectNumber = i;
                sequencer.Dispatch(() => this._tasksOutput.Add(antiClosureSideEffectNumber));
            }

            // Indicates the end of the sequence with a final task
            sequencer.Dispatch(() => this._sequenceFinished.Set());

            // Waits for sequence completion
            Check.That(this._sequenceFinished.WaitOne(ThreeSecondsMax)).IsTrue();

            // Checks that everything was properly processed in sequence
            for (int k = 0; k < tasksNumber; k++)
            {
                Check.That(this._tasksOutput[k]).IsEqualTo(k);
            }
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this._sequenceFinished = new AutoResetEvent(false);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            if (this._sequenceFinished != null)
            {
                this._sequenceFinished.Dispose();
                this._sequenceFinished = null;
            }
        }

        #endregion

        // TODO: test the dispatching concurrency (to check that no task is missing)
    }
}
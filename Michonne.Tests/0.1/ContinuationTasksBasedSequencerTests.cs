// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContinuationTasksBasedSequencerTests.cs" company="No lock... no deadlock">
//   Copyright 2014 Olivier COANET
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Michonne.Tests
{
    using System.Threading;
    using NFluent;
    using NUnit.Framework;

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
            const int ActionCount = 200 * 1000;

            var dispatchLock = new object();
            var wasRunConcurrently = false;

            for (var actionIndex = 0; actionIndex < ActionCount; ++actionIndex)
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

            this.WaitForActions();

            Check.That(wasRunConcurrently).IsFalse();
        }

        [Test]
        public void ShouldRunActionsInOrder()
        {
            const int ActionCount = 200 * 1000;

            var dispatchIndex = 0;
            var wasOutOfOrder = false;

            for (var actionIndex = 0; actionIndex < ActionCount; ++actionIndex)
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
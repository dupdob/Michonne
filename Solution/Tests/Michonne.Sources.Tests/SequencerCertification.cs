// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="SequencerCertification.cs" company="No lock... no deadlock" product="Michonne">
//     Copyright 2014 Cyrille DUPUYDAUBY (@Cyrdup)
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

using NUnit.Compatibility;

namespace Michonne.Tests
{
    using System;
    using System.Reflection;
    using Implementation;
    using Interfaces;

    using Michonne.Sources.Tests;

    using NFluent;
    using NUnit.Framework;

    /// <summary>
    /// The sequencer certification.
    /// </summary>
    /// <typeparam name="T"> type of sequencer
    /// </typeparam>
    //// [TestFixture(typeof(TaskContinuationSequencer))]
    [TestFixture(typeof(Sequencer))]
    internal class SequencerCertification<T>
    {
        // specify here the 
        #region Fields

        /// <summary>
        /// The _sequencer type.
        /// </summary>
        private readonly Type sequencerType = typeof(T);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the constructor.
        /// </summary>
        private ConstructorInfo Constructor
        {
            get
            {
                var constructor = this.sequencerType.GetTypeInfo().GetConstructor(new[] { typeof(IExecutor) });
                return constructor;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The sequencer should process tasks fairly.
        /// </summary>
        /// <remarks>Here the underlying unit of execution is a dedicated thread. So, all tasks are processed sequentially.
        /// This test dispatches tasks alternatively through the sequencer or directly through the unit of execution.
        /// Actual execution order should not be changed!</remarks>
        [Test]
        public void Sequencer_should_process_fairly()
        {
            var factory = new ExecutorFactory();
            IExecutor thread = factory.GetDedicatedThread();
            var sequencer = this.BuildSequencer(thread);
            var context = new RaceConditionDetector();

            // we inject delay deliberately, to ensure queueing happens
            sequencer.Dispatch(() => context.Delay(20));
            var current = 0;
            var failed = false;
            for (var i = 0; i < 1000; i++)
            {
                var targetCount = i;
                var executor = ((i % 2) == 0) ? thread : sequencer;
                executor.Dispatch(
                    () =>
                        {
                            // we check if the task is executed at the proper rank
                            if (targetCount != current)
                            {
                                failed = true;
                            }

                            current++;
                        });
            }

            //thread.Dispose();
            Check.That(context.WaitForTasks(1)).IsFalse();
            Check.That(failed).IsFalse();
        }

        /// <summary>
        /// The should_ execute_ tasks_ non_ concurrently.
        /// </summary>
        [Test]
        public void Should_Execute_Tasks_Non_Concurrently()
        {
            var poolExec = TestHelpers.GetPool();
            ISequencer sequencer = this.BuildSequencer(poolExec);
            var context = new RaceConditionDetector();

            // first task inject delay
            sequencer.Dispatch(() => context.Delay(20));

            // second task check non concurrence
            sequencer.Dispatch(() => context.Delay(20));
            
            // wait for the two tasks to be executed
            context.WaitForTasks(2);
            Check.That(context.RaceConditionDetected).IsFalse();
        }

        /// <summary>
        /// The should_ execute_ tasks_ sequentially.
        /// </summary>
        [Test]
        public void Should_Execute_Tasks_Sequentially()
        {
            var poolExec = TestHelpers.GetPool();
            var sequencer = this.BuildSequencer(poolExec);
            var context = new RaceConditionDetector();
            sequencer.Dispatch(() => context.Delay(20));
            var current = 0;
            var failed = false;
            for (var i = 0; i < 1000; i++)
            {
                var targetCount = i;
                sequencer.Dispatch(
                    () =>
                        {
                            // we check if the task is executed at the proper rank
                            if (targetCount != current)
                            {
                                failed = true;
                            }

                            current++;
                        });
            }

            Check.That(context.WaitForTasks(1)).IsFalse();
            Check.That(failed).IsFalse();
        }

        /// <summary>
        /// The should_ support_ injection_ of_ unit_ of_ execution.
        /// </summary>
        [Test]
        public void Should_Support_Injection_Of_Unit_Of_Execution()
        {
            ConstructorInfo constructor = this.Constructor;
            Check.That(constructor).IsNotNull();
        }

        /// <summary>
        /// The should_ use_ provided_ unit_ of_ execution.
        /// </summary>
        [Test]
        public void Should_Use_Provided_Unit_Of_Execution()
        {
            var synchExec = new SynchroCall();
            var sequencer = this.BuildSequencer(synchExec);
            sequencer.Dispatch(() => { });

            Check.That(synchExec.DoneTasks).Equals(1);
        }

        //[Test]
        public void Should_Be_Fast()
        {
            var factory = new ExecutorFactory();
            var unitOfExec = factory.GetSynchronousUnitOfExecution();
            var sequencer = this.BuildSequencer(unitOfExec);

            Check.ThatCode(() =>
            {
                for (var i = 0; i < 100000; i++)
                {
                    sequencer.Dispatch(() => { });
                }
            }).LastsLessThan(400, TimeUnit.Milliseconds);
 
        }

        [Test]
        public void ShouldNotExhibitKnownBugs()
        {
            var stepper = new StepperUnit();
            var count = 0;
            var sequencer = this.BuildSequencer(stepper);

            sequencer.Dispatch(() => count++);
            sequencer.Dispatch(() => count++);

            stepper.Step();
            Check.That(count).IsStrictlyGreaterThan(0);
            stepper.Step();
            Check.That(count).IsEqualTo(2);
        }

        #endregion

        // using a single thread unit of execution should lead to a sequential execution of Actions

        private ISequencer BuildSequencer(IExecutor synchExec)
        {
            var sequencer = this.Constructor.Invoke(new object[] { synchExec }) as ISequencer;
            Check.That(sequencer).IsNotNull();
            return sequencer;
        }

    }
}
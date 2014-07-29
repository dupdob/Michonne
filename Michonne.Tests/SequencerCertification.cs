// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SequencerCertification.cs" company="">
//   
// </copyright>
// <summary>
//   The sequencer certification.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Michonne.Tests
{
    using System;
    using System.Reflection;

    using Michonne.Interfaces;

    using NFluent;

    using NUnit.Framework;

    using Seq;

    /// <summary>
    /// The sequencer certification.
    /// </summary>
    /// <typeparam name="T"> type of sequencer
    /// </typeparam>
    [TestFixture(typeof(TaskContinuationSequencer))]
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
                ConstructorInfo constructor = this.sequencerType.GetConstructor(new[] { typeof(IUnitOfExecution) });
                return constructor;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The sequencer_should_process_fairly.
        /// </summary>
        [Test]
        public void Sequencer_should_process_fairly()
        {
            var thread = new ThreadUnitOfExecution();
            var sequencer = this.BuildSequencer(thread);
            var context = new TaskContext();

            // we inject delay deliberately, to ensure queueing happens
            sequencer.Dispatch(() => context.Delay(20));
            var current = 0;
            var failed = false;
            for (var i = 0; i < 1000; i++)
            {
                var targetCount = i;
                var executor = ((i % 2) == 0) ? thread : (IUnitOfExecution)sequencer;
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

            thread.Dispose();
            Check.That(context.WaitForTasks(1)).IsFalse();
            Check.That(failed).IsFalse();
        }

        /// <summary>
        /// The should_ execute_ tasks_ non_ concurrently.
        /// </summary>
        [Test]
        public void Should_Execute_Tasks_Non_Concurrently()
        {
            var poolExec = new DotNetThreadPoolUnitOfExecution();
            ISequencer sequencer = this.BuildSequencer(poolExec);
            var context = new TaskContext();

            // first task inject delay
            sequencer.Dispatch(() => context.Delay(20));

            // second task check non concurrence
            sequencer.Dispatch(() => context.Delay(20));

            // wait for the two tasks to be executed
            Check.That(context.WaitForTasks(2)).IsFalse();
        }

        /// <summary>
        /// The should_ execute_ tasks_ sequentially.
        /// </summary>
        [Test]
        public void Should_Execute_Tasks_Sequentially()
        {
            var poolExec = new DotNetThreadPoolUnitOfExecution();
            ISequencer sequencer = this.BuildSequencer(poolExec);
            var context = new TaskContext();
            sequencer.Dispatch(() => context.Delay(20));
            int current = 0;
            bool failed = false;
            for (int i = 0; i < 1000; i++)
            {
                int targetCount = i;
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
            ISequencer sequencer = this.BuildSequencer(synchExec);
            sequencer.Dispatch(() => { });

            Check.That(synchExec.DoneTasks).Equals(1);
        }

        #endregion

        // using a single thread unit of execution should lead to a sequential execution of Actions
        #region Methods

        /// <summary>
        /// The build sequencer.
        /// </summary>
        /// <param name="synchExec">
        /// The synch exec.
        /// </param>
        /// <returns>
        /// The <see cref="ISequencer"/>.
        /// </returns>
        private ISequencer BuildSequencer(IUnitOfExecution synchExec)
        {
            var sequencer = this.Constructor.Invoke(new object[] { synchExec }) as ISequencer;
            Check.That(sequencer).IsNotNull();
            return sequencer;
        }

        #endregion
    }
}
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Michonne.Interfaces;
using NFluent;
using NUnit.Framework;

namespace Michonne.Tests
{
    [TestFixture(typeof(Sequencer))]
    internal class SequencerCertification<T>
    {
        // specify here the 
        private readonly Type _sequencerType = typeof (T);

        [Test]
        public void Should_Support_Injection_Of_Unit_Of_Execution()
        {
            var constructor = this.Constructor;
            Check.That(constructor).IsNotNull();
        }

        private ConstructorInfo Constructor
        {
            get
            {
                var constructor = this._sequencerType.GetConstructor(new[] {typeof (IUnitOfExecution)});
                return constructor;
            }
        }

        [Test]
        public void Should_Use_Provided_Unit_Of_Execution()
        {
            var synchExec = new SynchroCall();
            var sequencer = this.BuildSequencer(synchExec);
            sequencer.Dispatch( () => {});

            Check.That(synchExec.DoneTasks).Equals(1);
        }

        [Test]
        public void Should_Execute_Tasks_Non_Concurrently()
        {
            var poolExec = new DotNetThreadPoolUnitOfExecution();
            var sequencer = this.BuildSequencer(poolExec);
            var context = new TaskContext();

            // first task inject delay
            sequencer.Dispatch(() => context.Delay(20));
            // second task check non concurrence
            sequencer.Dispatch(() => context.Delay(20));
            // wait for the two tasks to be executed

            Check.That(context.WaitForTasks(2)).IsFalse();
        }

        [Test]
        public void Should_Execute_Tasks_Sequentially()
        {
            var poolExec = new DotNetThreadPoolUnitOfExecution();
            var sequencer = this.BuildSequencer(poolExec);
            var context = new TaskContext();
            sequencer.Dispatch( () => context.Delay(20));
            var current = 0;
            var failed = false;
            for (var i = 0; i < 1000; i++)
            {
                var targetCount = i;
                sequencer.Dispatch(()=>
                {
                    // we check if the task is executed at the proper rank
                    if (targetCount != current)
                        failed = true;
                    current++;
                });
            }
            Check.That(context.WaitForTasks(1)).IsFalse();
            Check.That(failed).IsFalse();
        }

        // using a single thread unit of execution should lead to a sequential execution of Actions
        [Test]
        public void Sequencer_should_process_fairly()
        {
            var thread = new ThreadUnitOfExecution();
            var sequencer = this.BuildSequencer(thread);
            var context = new TaskContext();
            sequencer.Dispatch(() => context.Delay(20));
            var current = 0;
            var failed = false;
            for (var i = 0; i < 1000; i++)
            {
                var targetCount = i;
                var executor = ((i%2) == 0) ? thread : (IUnitOfExecution) sequencer;
                executor.Dispatch(() =>
                {
                    // we check if the task is executed at the proper rank
                    if (targetCount != current)
                        failed = true;
                    current++;
                });
            }
            Check.That(context.WaitForTasks(1)).IsFalse();
            Check.That(failed).IsFalse();
        }


        private ISequencer BuildSequencer(IUnitOfExecution synchExec)
        {
            var sequencer = this.Constructor.Invoke(new object[] {synchExec}) as ISequencer;
            Check.That(sequencer).IsNotNull();
            return sequencer;
        }
       
    }
}


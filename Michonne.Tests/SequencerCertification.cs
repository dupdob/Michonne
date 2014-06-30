using System;
using System.Reflection;
using System.Threading;
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

        private class SynchroCall : IUnitOfExecution
        {
            public int DoneTasks;

            #region IUnitOfExecution Members

            public void Dispatch(Action action)

            {
                action();
                this.DoneTasks++;
            }

            #endregion
        }

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
            var lck = new object();
            var done = false;
            var success = true;
            // first task inject delay
            sequencer.Dispatch(() => { lock(lck) Thread.Sleep(30);});
            // second task check non concurrence
            sequencer.Dispatch(() =>
            {
                if (Monitor.TryEnter(lck))
                {
                    try
                    {
                        done = true;
                        Monitor.Pulse(lck);
                    }
                    finally
                    {
                        Monitor.Exit(lck);
                    }
                }
                else
                {
                    success = false;
                    done = true;
                }
            });
            // wait for the two tasks to be executed
            lock (lck)
            {
                if (!done)
                {
                    Check.That(Monitor.Wait(lck, 100));
                }
            }
            Check.That(done).IsTrue();
            Check.That(success).IsTrue();
        }

        private ISequencer BuildSequencer(IUnitOfExecution synchExec)
        {
            var sequencer = this.Constructor.Invoke(new object[] {synchExec}) as ISequencer;

            Check.That(sequencer).IsNotNull();
            return sequencer;
        }
    }
}


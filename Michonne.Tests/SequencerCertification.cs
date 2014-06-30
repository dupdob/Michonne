using System;
using System.Reflection;
using Michonne.Interfaces;
using NFluent;
using NUnit.Framework;

namespace Michonne.Tests
{
    [TestFixture]
    internal class SequencerCertification
    {
        // specify here the 
        private readonly Type _sequencerType = typeof (Sequencer);

        private class SynchroCall : IUnitOfExecution
        {
            public int _doneTasks;

            #region IUnitOfExecution Members

            public void Dispatch(Action action)

            {
                action();
                this._doneTasks++;
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
            var sequencer = this.Constructor.Invoke(new object[] {synchExec}) as ISequencer;

            Check.That(sequencer).IsNotNull();

            sequencer.Dispatch( () => {});

            Check.That(synchExec._doneTasks).Equals(1);
        }
    }
}


#region File header
// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="SynchonousUnitOfExecutionTests.cs" company="No lock... no deadlock" product="Michonne">
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
#endregion
namespace Michonne.Tests
{
    using System.Threading;

    using Implementation;
    using NFluent;

    using NUnit.Framework;

    [TestFixture]
    public class SynchonousUnitOfExecutionTests
    {
//        [Test]
        public void ShouldExecuteSynchro()
        {
            var factory = new UnitOfExecutionsFactory();
            var synchronousUnitOfExec = factory.GetSynchronousUnitOfExecution();
            var synchro = new object();

            lock (synchro)
            {
                synchronousUnitOfExec.Dispatch(
                    () =>
                        {
                            Check.That(Monitor.TryEnter(synchro)).IsTrue();
                            Monitor.Exit(synchro);
                        });
            }
        }
    }
}
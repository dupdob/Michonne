#region File header

// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ConflationTests.cs" company="No lock... no deadlock" product="Michonne">
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

    using Michonne.Implementation;

    using NFluent;

    using NUnit.Framework;

    /// <summary>
    /// The conflation tests.
    /// </summary>
    [TestFixture]
    public class ConflationTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The should conflate actions.
        /// </summary>
        [Test]
        public void ShouldConflateActions()
        {
            var dedicated = new ThreadUnitOfExecution();
            var synchroRoot = new object();
            bool go = true;
            int ranTasks = 0;

            var conflator = new Conflator(dedicated);

            dedicated.Dispatch(
                () =>
                    {
                        lock (synchroRoot)
                        {
                            while (go)
                            {
                                Monitor.Wait(synchroRoot, 1000);
                            }
                        }
                    });

            conflator.Conflate(() => Interlocked.Increment(ref ranTasks));
            conflator.Conflate(() => Interlocked.Increment(ref ranTasks));
            conflator.Conflate(() => Interlocked.Increment(ref ranTasks));

            lock (synchroRoot)
            {
                go = false;
                Monitor.Pulse(synchroRoot);
            }

            Thread.Sleep(100);

            // tasks should be conflated
            Check.That(ranTasks).IsEqualTo(1);
        }

        #endregion
    }
}
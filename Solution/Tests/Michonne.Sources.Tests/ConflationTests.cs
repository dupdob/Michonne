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
    using System;
    using System.Threading;

    using Michonne.Implementation;
    using Michonne.Sources.Tests;

    using NFluent;

    using NUnit.Framework;

    /// <summary>
    ///     The conflation tests.
    /// </summary>
    public class ConflationTests
    {
        /// <summary>
        ///     The should conflate actions.
        /// </summary>
        [Test]
        public void ShouldConflateActions()
        {
            var dedicated = new StepperUnit();
            var go = true;
            var ranTasks = 0;

            var conflator = new ActionConflator(dedicated);

            conflator.Conflate(() => Interlocked.Increment(ref ranTasks));
            conflator.Conflate(() => Interlocked.Increment(ref ranTasks));
            conflator.Conflate(() => Interlocked.Increment(ref ranTasks));
            dedicated.Step();
 

            // tasks should be conflated
            Check.That(ranTasks).IsEqualTo(1);
        }

        [Test]
        public void ShouldHaveANiceApi()
        {
            // conflating is more about data than about calls
            // the API should reflect that
            var factory = new ExecutorFactory();
            var dedicated = factory.GetDedicatedThread();
            var act = 0;
            Action<int> processing = _ => act++;

            var conflatedProcessor = dedicated.BuildConflator(processing);

            dedicated.Dispatch(() => Thread.Sleep(10));
            conflatedProcessor(1);
            conflatedProcessor(2);
            conflatedProcessor(3);
            conflatedProcessor(4);
            Thread.Sleep(30);

            Check.That(act).IsEqualTo(1);
        }

        [Test]
        public void ShouldHaveANiceDataApi()
        {
            // conflating is more about data than about calls
            // the API should reflect that
            var factory = new ExecutorFactory();
            var dedicated = factory.GetDedicatedThread();
            var act = 0;
            Action<int> processing = _ => act++;

            var conflatedProcessor = dedicated.BuildProcessor(processing, true);

            dedicated.Dispatch(() => Thread.Sleep(10));
            conflatedProcessor.Post(1);
            conflatedProcessor.Post(2);
            conflatedProcessor.Post(3);
            conflatedProcessor.Post(4);
            Thread.Sleep(30);

            Check.That(act).IsEqualTo(1);
        }
    }
}
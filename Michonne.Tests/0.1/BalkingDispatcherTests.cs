// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BalkingDispatcherTests.cs" company="No lock... no deadlock">
//    Copyright 2014 Cyrille  DUPUYDAUBY (@Cyrdup), Thomas PIERRAIN (@tpierrain)
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//        http://www.apache.org/licenses/LICENSE-2.0
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
namespace Michonne.Tests
{
    using System.Collections.Generic;

    using NFluent;

    using NUnit.Framework;

    [TestFixture]
    public class BalkingDispatcherTests
    {
        private List<int> processedValues;

        [SetUp]
        public void SetUp()
        {
            this.processedValues = new List<int>();
        }

        [Test]
        public void TasksDispatchedWhileBusyAreDiscarded()
        {
            var pollingRootDispatcher = new PollingDispatcher();
            var balkingDispatcher = new BalkingDispatcher(pollingRootDispatcher);

            balkingDispatcher.Dispatch(() => this.processedValues.Add(1));
            balkingDispatcher.Dispatch(() => this.processedValues.Add(2));
            
            balkingDispatcher.Dispatch(() => this.processedValues.Add(3));
            pollingRootDispatcher.ExecuteNextTask();
            
            // Should have executed only the latest task before the ExecuteNextTask
            Check.That(this.processedValues).HasSize(1).And.ContainsExactly(3);

            balkingDispatcher.Dispatch(() => this.processedValues.Add(4));
            pollingRootDispatcher.ExecuteNextTask();

            balkingDispatcher.Dispatch(() => this.processedValues.Add(5));
            balkingDispatcher.Dispatch(() => this.processedValues.Add(6));
            pollingRootDispatcher.ExecuteNextTask();

            Check.That(this.processedValues).HasSize(3).And.ContainsExactly(3, 4, 6);
        }

        [Test]
        public void TheLastDispatchedTaskIsNotExecutedMoreThanOnce()
        {
            var pollingRootDispatcher = new PollingDispatcher();
            var balkingDispatcher = new BalkingDispatcher(pollingRootDispatcher);

            balkingDispatcher.Dispatch(() => this.processedValues.Add(1));
            balkingDispatcher.Dispatch(() => this.processedValues.Add(2));

            pollingRootDispatcher.ExecuteNextTask();
            pollingRootDispatcher.ExecuteNextTask();

            Check.That(this.processedValues).HasSize(1).And.ContainsExactly(2);
        }
    }
}

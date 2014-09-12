// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PollingDispatcherTests.cs" company="No lock... no deadlock">
//   Copyright 2014 Thomas PIERRAIN
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
    using System.Collections.Generic;

    using NFluent;

    using NUnit.Framework;

    [TestFixture]
    public class PollingDispatcherTests
    {
        private List<int> processedValues;

        [SetUp]
        public void SetUp()
        {
            this.processedValues = new List<int>();
        }

        [Test]
        public void DispatchedTasksAreExecutedOnlyWhenCallingExecuteNextTask()
        {
            var pollingDispatcher = new PollingDispatcher();
            pollingDispatcher.Dispatch(() => this.processedValues.Add(1));
            Check.That(this.processedValues).HasSize(0);
            
            pollingDispatcher.ExecuteNextTask();
            Check.That(this.processedValues).HasSize(1).And.ContainsExactly(1);
        }

        [Test]
        public void CallingExecuteNextTaskDoesNotThrowWhenNoTaskIsDispatched()
        {
            var pollingDispatcher = new PollingDispatcher();
            pollingDispatcher.ExecuteNextTask();
        }
    }
}

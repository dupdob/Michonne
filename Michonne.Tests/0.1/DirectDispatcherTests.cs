// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectDispatcherTests.cs" company="No lock... no deadlock">
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
    using System.Threading;

    using NFluent;

    using NUnit.Framework;

    [TestFixture]
    public class DirectDispatcherTests
    {
        private string executingThreadName;

        [SetUp]
        public void SetUp()
        {
            this.executingThreadName = null;
        }

        [Test]
        public void DirectDispatcherExecutesTasksInTheCallerThread()
        {
            var directDispatcher = new DirectDispatcher();
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "unit Test thread";                
            }
            
            directDispatcher.Dispatch(() => this.executingThreadName = Thread.CurrentThread.Name);
            
            Check.That(this.executingThreadName).IsEqualTo(Thread.CurrentThread.Name).And.IsNotNull();
        }
    }
}

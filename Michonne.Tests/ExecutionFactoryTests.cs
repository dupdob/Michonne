#region File header
// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="ExecutionFactoryTests.cs" company="" product="Michonne">
// //   Copyright 2014 Cyrille Dupuydauby
// //   Licensed under the Apache License, Version 2.0 (the "License");
// //   you may not use this file except in compliance with the License.
// //   You may obtain a copy of the License at
// //       http://www.apache.org/licenses/LICENSE-2.0
// //   Unless required by applicable law or agreed to in writing, software
// //   distributed under the License is distributed on an "AS IS" BASIS,
// //   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// //   See the License for the specific language governing permissions and
// //   limitations under the License.
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
#endregion
namespace Michonne.Tests
{
    using System;

    using Michonne.Implementation;

    using NFluent;

    using NUnit.Framework;

    [TestFixture]
    public class ExecutionFactoryTests
    {
        [Test]
        public void Factory_should_detect_number_of_cores()
        {
            var factory = new UnitOfExecutionsFactory();

            Check.That(factory.CoreCount).IsEqualTo(Environment.ProcessorCount);
        }

        [Test]
        public void Factory_should_create_pool_unit()
        {
            var factory = new UnitOfExecutionsFactory();
            var poolExec = factory.GetPool();
            Check.That(poolExec).IsNotNull();
        }

        [Test]
        public void Factory_should_create_worker_unit()
        {
            var factory = new UnitOfExecutionsFactory();

            var threadExec = factory.GetDedicatedThread();

            Check.That(threadExec).IsNotNull();
        }

        [Test]
        public void Factory_should_decrease_core_count()
        {
            var factory = new UnitOfExecutionsFactory();
            var initialCount = factory.AvailableCore;
            var threadExec = factory.GetDedicatedThread();

            Check.That(threadExec).IsNotNull();
            Check.That(factory.AvailableCore).IsEqualTo(initialCount - 1);
        }
    }
}
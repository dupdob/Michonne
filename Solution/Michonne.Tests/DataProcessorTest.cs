#region File header
// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="DataProcessor.cs" company="No lock... no deadlock" product="Michonne">
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
    using System.Collections.Generic;

    using Implementation;

    using NFluent;

    using NUnit.Framework;

    internal class DataProcessorTest
    {
        [Test]
        public void ShouldHaveANiceAPI()
        {
            var factory = new UnitOfExecutionsFactory();
            var executor = factory.GetDedicatedThread();
            var processed = new List<int>();

            var processor = executor.BuildProcessor<int>( processed.Add, false);

            processor.Post(4);
            processor.Post(5);

            Check.That(processed).HasSize(2);
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="PastaCalculatorTests.cs" company="No lock... no deadlock" product="Michonne">
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

namespace PastaPricer.Tests
{
    using NFluent;

    using NUnit.Framework;

    [TestFixture]
    public class PastaCalculatorTests
    {
        [Test]
        public void Should_compute_expected_price()
        {
            var pastaCalculator = new PastaCalculator();
            Check.That(PastaCalculator.Compute(flourPrice: 1.3m, eggsPrice: 2.4m, flavorPrice: 1.2m)).IsEqualTo(2.52m);
            Check.That(PastaCalculator.Compute(flourPrice: 1.9m, eggsPrice: 2.4m, flavorPrice: 1.2m)).IsEqualTo(3.12m);
        }
    }
}

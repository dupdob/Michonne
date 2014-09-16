// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="PastaPricingAgentTests.cs" company="No lock... no deadlock" product="Michonne">
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
    using System.Collections.Generic;
    using System.Threading;

    using Michonne;
    using Michonne.Implementation;

    using NFluent;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class PastaPricingAgentTests
    {
        [Test]
        public void Should_publish_pasta_price_as_soon_as_all_needed_RawMaterialMarketData_are_received()
        {
            // spaghetti(eggs-flour)
            // Prepares the marketdata mocks
            var eggsMarketDataMock = Substitute.For<IRawMaterialMarketData>();
            eggsMarketDataMock.RawMaterialName.Returns("eggs");

            var flourMarketDataMock = Substitute.For<IRawMaterialMarketData>();
            flourMarketDataMock.RawMaterialName.Returns("flour");
            
            // setup the pricing agent
            var sequencer = new Sequencer(new DirectDispatcher());
            var pricingAgent = new PastaPricingAgent(sequencer, "spaghetti");
            pricingAgent.SubscribeToMarketData(new List<IRawMaterialMarketData>() { eggsMarketDataMock, flourMarketDataMock });
            var pastaPriceHasChanged = false;
            pricingAgent.PastaPriceChanged += (o, args) => { pastaPriceHasChanged = true; };

            Check.That(pastaPriceHasChanged).IsFalse();

            // Raises event for "eggs"
            eggsMarketDataMock.PriceChanged += Raise.EventWith(new object(), new RawMaterialPriceChangedEventArgs("eggs", 0));
            Check.That(pastaPriceHasChanged).IsFalse();

            // Raises event for "flour" => all needed market data has been received, the price must have been published now
            flourMarketDataMock.PriceChanged += Raise.EventWith(new object(), new RawMaterialPriceChangedEventArgs("flour", 0));
            Check.That(pastaPriceHasChanged).IsTrue();
        }

        [Test]
        public void Should_Compute_price_from_market_data_inputs()
        {
            // flour
            // 
        }
    }
}

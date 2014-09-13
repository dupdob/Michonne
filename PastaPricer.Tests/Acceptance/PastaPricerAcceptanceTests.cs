// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="PastaPricerAcceptanceTests.cs" company="No lock... no deadlock" product="Michonne">
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
namespace PastaPricer.Tests.Acceptance
{
    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class PastaPricerAcceptanceTests
    {
        [Test]
        public void Should_Publish_Price_Once_Started_And_When_MarketData_Is_Available()
        {
            // Mock and dependencies setup
            var publisher = Substitute.For<IPastaPricerPublisher>();
            var marketDataProvider = new MarketDataProvider(new [] { "eggs"});
            
            var pastaPricer = new PastaPricerEngine(new [] { "gnocchi"}, marketDataProvider, publisher);
            
            CheckThatNoPriceHasBeenPublished(publisher);
            
            pastaPricer.Start();

            CheckThatNoPriceHasBeenPublished(publisher);

            // Turns on market data
            marketDataProvider.Start();

            // It has publish a price now!
            publisher.ReceivedWithAnyArgs().Publish(string.Empty, 0);
        }

        private static void CheckThatNoPriceHasBeenPublished(IPastaPricerPublisher publisher)
        {
            publisher.DidNotReceiveWithAnyArgs().Publish(string.Empty, 0);
        }
    }
}
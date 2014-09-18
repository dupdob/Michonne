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
    using System.Threading;

    using Michonne.Implementation;

    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class PastaPricerAcceptanceTests
    {
        [Test]
        public void Should_publish_price_once_started_and_when_MarketData_is_available()
        {
            // Mock and dependencies setup
            var unitOfExecutionsFactory = new UnitOfExecutionsFactory();
            
            var publisher = Substitute.For<IPastaPricerPublisher>();
            var marketDataProvider = new MarketDataProvider();

            var pastaPricer = new PastaPricerEngine(unitOfExecutionsFactory.GetPool(), new[] { "gnocchi(eggs-potatoes-flour)" }, marketDataProvider, publisher);
            
            CheckThatNoPriceHasBeenPublished(publisher);
            
            pastaPricer.Start();

            CheckThatNoPriceHasBeenPublished(publisher);

            // Turns on market data (note: make the pasta pricer start its dependencies instead?)
            marketDataProvider.Start();

            // A sleep?!? There should be a better way ;-)
            Thread.Sleep(60);

            // It has publish a price now!
            publisher.Received().Publish("gnocchi", 0.50m);
        }

        private static void CheckThatNoPriceHasBeenPublished(IPastaPricerPublisher publisher)
        {
            publisher.DidNotReceiveWithAnyArgs().Publish("whatever the pasta name here", 0);
        }

        [Test]
        public void Should_publish_price_for_every_registered_pasta()
        {
            // Mock and dependencies setup
            var publisher = Substitute.For<IPastaPricerPublisher>();
            var marketDataProvider = new MarketDataProvider();
            var pastasConfiguration = new[]
                                      {
                                          "gnocchi(eggs-potatoes-flour)",
                                          "spaghetti(eggs-flour)",
                                          "organic spaghetti(organic eggs-flour)",
                                          "spinach farfalle(eggs-flour-spinach)",
                                          "tagliatelle(eggs-flour)",
                                      };

            var unitOfExecutionsFactory = new UnitOfExecutionsFactory();

            var pastaPricer = new PastaPricerEngine(unitOfExecutionsFactory.GetPool(), pastasConfiguration, marketDataProvider, publisher);
            pastaPricer.Start();

            // Turns on market data (note: make the pasta pricer start its dependencies instead?)
            marketDataProvider.Start();

            // A sleep?!? There should be a better way ;-)
            Thread.Sleep(100);

            publisher.Received().Publish("gnocchi", 0.5m);
            publisher.Received().Publish("spaghetti", 0);
            publisher.Received().Publish("organic spaghetti", 0);
            publisher.Received().Publish("spinach farfalle", 0.5m);
            publisher.Received().Publish("tagliatelle", 0);
        }
    }
}
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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    using Michonne.Implementation;
    using NFluent;
    using NUnit.Framework;

    [TestFixture]
    public class PastaPricerAcceptanceTests: IPastaPricerPublisher
    {
        private IDictionary<string, decimal> lastPrices = new Dictionary<string, decimal>();

        [Test]
        public void Should_publish_price_once_started_and_when_MarketData_is_available()
        {
            // Mock and dependencies setup
            var unitOfExecutionsFactory = new UnitOfExecutionsFactory();
            
            var marketDataProvider = new MarketDataProvider();

            var pastaPricer = new PastaPricerEngine(unitOfExecutionsFactory.GetPool(),
                new[] { "gnocchi(eggs-potatoes-flour)" }, 
                marketDataProvider, 
                this);
            this.ClearDico();
            Check.That(this.lastPrices).IsEmpty();
            
            pastaPricer.Start();

            Check.That(this.lastPrices).IsEmpty();

            // Turns on market data (note: make the pasta pricer start its dependencies instead?)
            marketDataProvider.Start();

            // A sleep?!? There should be a better way ;-)
            Thread.Sleep(1000);

            // It has publish a price now!
            Check.That(this.lastPrices.Keys).Contains("gnocchi");
        }

        [Test]
        public void Should_publish_price_for_every_registered_pasta()
        {
            this.ClearDico();
            // Mock and dependencies setup
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

            var pastaPricer = new PastaPricerEngine(unitOfExecutionsFactory.GetPool(), pastasConfiguration, marketDataProvider, this);
            pastaPricer.Start();

            // Turns on market data (note: make the pasta pricer start its dependencies instead?)
            marketDataProvider.Start();

            // A sleep?!? There should be a better way ;-)
            Stopwatch timer = new Stopwatch();
            timer.Start();
            while (timer.ElapsedMilliseconds < 20000)
            {
                lock (this.lastPrices)
                {
                    if (pastasConfiguration.Length == this.lastPrices.Count)
                    {
                        // all price received
                        break;
                    }
                    Monitor.Wait(this.lastPrices, 100);
                }
            }

            Check.That(this.lastPrices.Keys).Contains("gnocchi");
            Check.That(this.lastPrices.Keys).Contains("spaghetti");
            Check.That(this.lastPrices.Keys).Contains("organic spaghetti");
            Check.That(this.lastPrices.Keys).Contains("spinach farfalle");
            Check.That(this.lastPrices.Keys).Contains("tagliatelle");
        }

        private void ClearDico()
        {
            lock (this.lastPrices)
            {
                this.lastPrices.Clear();
            }
        }

        public void Publish(string pastaIdentifier, decimal price)
        {
            lock (this.lastPrices)
            {
                this.lastPrices[pastaIdentifier] = price;
                Monitor.Pulse(this.lastPrices);
            }
        }
    }
}
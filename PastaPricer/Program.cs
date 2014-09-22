// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="Program.cs" company="No lock... no deadlock" product="Michonne">
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
namespace PastaPricer
{
    using System;
    using System.Threading;

    using Michonne.Implementation;

    /// <summary>
    /// Pasta pricer program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the pasta pricer. Type Enter to stop market data inputs.");

            var publisher = new ConsolePublisher();

            var marketDataProvider = new AggresiveMarketDataProvider();
            const bool ConflationEnabled = false;

            //var marketDataProvider = new MarketDataProvider();
            var pastasConfiguration = new[]
                                      {
                                          "gnocchi(eggs-potatoes-flour)",
                                          "spaghetti(eggs-flour)",
                                          "organic spaghetti(organic eggs-flour)",
                                          "spinach farfalle(eggs-flour-spinach)",
                                          "tagliatelle(eggs-flour)",
                                      };

            var unitOfExecutionsFactory = new UnitOfExecutionsFactory();

            var pastaPricer = new PastaPricerEngine(unitOfExecutionsFactory.GetPool(), pastasConfiguration, marketDataProvider, publisher, ConflationEnabled);
            pastaPricer.Start();

            // Turns on market data (note: make the pasta pricer start its dependencies instead?)
            marketDataProvider.Start();

            // A sleep?!? There should be a better way ;-)
            Console.ReadLine();
            
            marketDataProvider.Stop();
            publisher.CountPublish();

            Console.WriteLine("Stopping market data. Type Enter to exit.");

            Console.ReadLine();

            Console.WriteLine("{0} late prices have been published since we stopped the market data.", publisher.PublicationCounter);

            Console.WriteLine("Type Enter to exit.");
        }
    }

    public class ConsolePublisher : IPastaPricerPublisher
    {
        private volatile bool startCounting;

        private long publicationCounter;

        public long PublicationCounter
        {
            get
            {
                return Interlocked.Read(ref this.publicationCounter);
            }
        }

        public void Publish(string pastaIdentifier, decimal price)
        {
            if (this.startCounting)
            {
                Interlocked.Increment(ref this.publicationCounter);
            }

            Console.WriteLine("{0} = {1} €", pastaIdentifier, price);
        }

        public void CountPublish()
        {
            this.startCounting = true;
        }
    }
}

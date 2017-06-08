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
    public class Programa
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main()
        {
            ThreadPool.SetMinThreads(Environment.ProcessorCount * 2, 0);
            Console.WriteLine("Welcome to the pasta pricer (powered by the Michonne library).");
            Console.WriteLine("Conflation Y/N?");
            var option = Console.ReadLine();
            bool conflationEnabled = false;

            if (option.ToUpper().StartsWith("Y"))
            {
                conflationEnabled = true;
                Console.WriteLine("Conflation enabled!\n");
            }
            else
            {
                Console.WriteLine("NO Conflation\n");
            }

            Console.WriteLine("         Type 'Enter' to start market data inputs.");
            Console.WriteLine("         Then type 'Enter' again, to stop market data inputs.");

            Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;

            var publisher = new ConsolePastaPricerPublisher();

            var marketDataProvider = new AggresiveMarketDataProvider(aggressionFactor: 10, timerPeriodInMsec: 1);

            var unitOfExecutionsFactory = new ExecutorFactory();
            ThreadPool.SetMaxThreads(Environment.ProcessorCount * 2, 0);
            ThreadPool.SetMinThreads(Environment.ProcessorCount * 2, 0);

            var pastaPricer = new PastaPricerEngine(unitOfExecutionsFactory.GetPool(), RecipeHelper.GenerateConfigurations(), marketDataProvider, publisher, conflationEnabled);
            pastaPricer.Start();

            // Turns on market data
            marketDataProvider.Start();

            Console.ReadLine();

            Console.WriteLine("----------------\nMarket data is stopping.\n----------------\n");

            marketDataProvider.Stop();
            publisher.CountPublish();

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("----------------\nMarket data stopped. Wait a while, and type 'Enter' to exit.\n----------------\n");

            Console.ReadLine();

            Console.WriteLine("{0} late prices have been published since we stopped the market data.", publisher.PublicationCounter);

            Console.WriteLine("Type Enter to exit the program.");
        
            Console.ReadLine();
        }
    }
}

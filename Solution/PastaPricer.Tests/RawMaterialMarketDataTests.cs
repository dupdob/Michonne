﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="RawMaterialMarketDataTests.cs" company="No lock... no deadlock" product="Michonne">
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
    using System.Threading;
    using NFluent;
    using NUnit.Framework;

    [TestFixture]
    public class RawMaterialMarketDataTests
    {
        [Test]
        public void Should_not_raise_event_after_having_called_Stop()
        {
            ThreadPool.SetMaxThreads(10, 10);
            const int VeryAggressiveTimerIntervalForMarketDataPublicationInMsec = 1;
            var marketData = new RawMaterialMarketData("eggs", VeryAggressiveTimerIntervalForMarketDataPublicationInMsec);
            
            long counter = 0;
            marketData.PriceChanged += (o, args) =>
            {
                lock (this)
                {
                    if (args.Price > 0m)
                        Interlocked.Increment(ref counter);
                    Monitor.PulseAll(this);
                }
            };

            marketData.Start();

            lock (this)
            {
                if (counter == 0)
                    Monitor.Wait(this, 50);
            }

            Check.That(counter).IsStrictlyGreaterThan(0);

            var counterBeforeStop = Interlocked.Read(ref counter);
            marketData.Stop();

            Thread.Sleep(50);
            
            var counterAfterStop = Interlocked.Read(ref counter);

            Check.That(counterAfterStop).IsEqualTo(counterBeforeStop);
        }
    }
}

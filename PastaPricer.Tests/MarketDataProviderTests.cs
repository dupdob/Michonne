// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="MarketDataProviderTests.cs" company="No lock... no deadlock" product="Michonne">
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
    using System;
    using System.Threading;
    using NFluent;
    using NUnit.Framework;

    [TestFixture]
    public class MarketDataProviderTests
    {
        private AutoResetEvent priceChangedRaisedEvent;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this.priceChangedRaisedEvent = new AutoResetEvent(false);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            if (this.priceChangedRaisedEvent != null)
            {
                this.priceChangedRaisedEvent.Dispose();
                this.priceChangedRaisedEvent = null;
            }
        }

        [Test]
        public void Should_provide_MarketData_for_eggs()
        {
            var marketDataProvider = new MarketDataProvider();
            marketDataProvider.RegisterAssets(new[] { "eggs" });

            Check.That(marketDataProvider.Get("eggs")).IsInstanceOf<MarketData>();
        }

        [Test]
        public void Should_return_the_same_instance_of_MarketData_from_the_same_name()
        {
            var marketDataProvider = new MarketDataProvider();
            marketDataProvider.RegisterAssets(new[] { "eggs" });

            Check.That(marketDataProvider.Get("eggs")).IsSameReferenceThan(marketDataProvider.Get("eggs"));
        }

        [Test]
        public void Should_only_get_MarketData_for_registered_assets()
        {
            var marketDataProvider = new MarketDataProvider();
            marketDataProvider.RegisterAssets(new[] { "eggs", "flour" });

            Check.That(marketDataProvider.Get("flour")).IsNotNull();

            Check.ThatCode(() => marketDataProvider.Get("banana")).Throws<InvalidOperationException>();

            Check.That(marketDataProvider.Get("eggs")).IsNotNull();
        }

        [Test]
        public void Should_receive_price_for_registered_assets_once_started()
        {
            var marketDataProvider = new MarketDataProvider();
            marketDataProvider.RegisterAssets(new[] { "eggs", "flour" });
            
            marketDataProvider.Get("eggs").PriceChanged += (o, args) => this.priceChangedRaisedEvent.Set();

            marketDataProvider.Start();

            var hasReceivedEvent = this.priceChangedRaisedEvent.WaitOne(100);

            Check.That(hasReceivedEvent).IsTrue();
            marketDataProvider.Stop();
        }
    }
}

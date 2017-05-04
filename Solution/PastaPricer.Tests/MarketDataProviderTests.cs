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

        #region setup/teardown

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            this.priceChangedRaisedEvent = new AutoResetEvent(false);
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            if (this.priceChangedRaisedEvent != null)
            {
                this.priceChangedRaisedEvent.Dispose();
                this.priceChangedRaisedEvent = null;
            }
        }

        #endregion

        [Test]
        public void Should_provide_MarketData_for_eggs()
        {
            var marketDataProvider = new MarketDataProvider();
            using (marketDataProvider)
            {
                marketDataProvider.RegisterRawMaterial("eggs");

                Check.That(marketDataProvider.GetRawMaterial("eggs")).IsInstanceOf<RawMaterialMarketData>();
            }
        }

        [Test]
        public void Should_return_the_same_instance_of_MarketData_given_the_same_rawMaterial_name()
        {
            var marketDataProvider = new MarketDataProvider();
            using (marketDataProvider)
            {
                marketDataProvider.RegisterRawMaterial("eggs");

                Check.That(marketDataProvider.GetRawMaterial("eggs")).IsSameReferenceThan(marketDataProvider.GetRawMaterial("eggs"));
            }
        }

        [Test]
        public void Should_only_get_MarketData_for_registered_rawMaterials_or_throw_an_exception_otherwise()
        {
            var marketDataProvider = new MarketDataProvider();
            marketDataProvider.RegisterRawMaterial("eggs");
            marketDataProvider.RegisterRawMaterial("flour");

            Check.That(marketDataProvider.GetRawMaterial("flour")).IsNotNull();

            Check.ThatCode(() => marketDataProvider.GetRawMaterial("banana")).Throws<InvalidOperationException>();

            Check.That(marketDataProvider.GetRawMaterial("eggs")).IsNotNull();
        }

        [Test]
        public void Should_receive_price_for_registered_RawMaterials_once_started()
        {
            var marketDataProvider = new MarketDataProvider();
            marketDataProvider.RegisterRawMaterial("eggs");

            void Handler(object o, RawMaterialPriceChangedEventArgs args) => this.priceChangedRaisedEvent.Set();
            marketDataProvider.GetRawMaterial("eggs").PriceChanged += Handler;
            using (marketDataProvider)
            {
                marketDataProvider.Start();

                const int TimeoutInMsec = 500;
                var hasReceivedEvent = this.priceChangedRaisedEvent.WaitOne(TimeoutInMsec);

                marketDataProvider.GetRawMaterial("eggs").PriceChanged -= Handler;
                Check.That(hasReceivedEvent).IsTrue();
            }
        }
    }
}

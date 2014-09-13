// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="MarketDataProvider.cs" company="No lock... no deadlock" product="Michonne">
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
    using System.Collections.Generic;

    public class MarketDataProvider : IMarketDataProvider
    {
        private Dictionary<string, MarketData> marketDatas;
        private IEnumerable<string> registeredAssetNames;

        public MarketDataProvider()
        {
            this.marketDatas = new Dictionary<string, MarketData>();
        }

        public void RegisterAssets(IEnumerable<string> registeredAssetNames)
        {
            this.registeredAssetNames = registeredAssetNames;
            this.InstantiateMarketDataForRegisteredAssets();
        }

        public void Start()
        {
            // TODO: make it thread-safe
            foreach (var marketData in this.marketDatas.Values)
            {
                marketData.Start();
            }
        }

        private void InstantiateMarketDataForRegisteredAssets()
        {
            foreach (var registeredAsset in this.registeredAssetNames)
            {
                var marketDataForThisAsset = new MarketData();
                this.marketDatas[registeredAsset] = marketDataForThisAsset;
                marketDataForThisAsset.Start();
            }
        }

        public MarketData Get(string assetName)
        {
            //TODO: make it thread-safe
            MarketData marketData;
            
            if (!this.marketDatas.TryGetValue(assetName, out marketData))
            {
                throw new InvalidOperationException(string.Format("Asset with name '{0}' is not registered for market data.", assetName));
            }

            return marketData;
        }

        public void Stop()
        {
            foreach (var marketData in this.marketDatas.Values)
            {
                marketData.Stop();
            }
        }
    }
}
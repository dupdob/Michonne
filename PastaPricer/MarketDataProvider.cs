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

    /// <summary>
    /// Provides <see cref="StapleMarketData"/> instances giving staple names.
    /// </summary>
    public class MarketDataProvider : IMarketDataProvider
    {
        private readonly Dictionary<string, StapleMarketData> stapleMarketDatas;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketDataProvider"/> class.
        /// </summary>
        public MarketDataProvider()
        {
            this.stapleMarketDatas = new Dictionary<string, StapleMarketData>();
        }

        /// <summary>
        /// Registers the specified staple, so that it can be started and retrieved afterwards.
        /// </summary>
        /// <param name="stapleNameToRegister">The staple name to register.</param>
        public void Register(string stapleNameToRegister)
        {
            // TODO: make it thread-safe
            if (!this.stapleMarketDatas.ContainsKey(stapleNameToRegister))
            {
                this.stapleMarketDatas.Add(stapleNameToRegister, new StapleMarketData(stapleNameToRegister));
            }
        }

        /// <summary>
        /// Starts all the registered <see cref="StapleMarketData" /> instances.
        /// </summary>
        public void Start()
        {
            // TODO: make it thread-safe
            foreach (var stapleMarketData in this.stapleMarketDatas.Values)
            {
                stapleMarketData.Start();
            }
        }

        /// <summary>
        /// Gets the <see cref="StapleMarketData" /> instance corresponding to this staple name.
        /// </summary>
        /// <param name="stapleName">StapleName of the staple.</param>
        /// <returns>
        /// The <see cref="StapleMarketData" /> instance corresponding to this staple name.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">When the staple is not registered yet to receive market data.</exception>
        public StapleMarketData Get(string stapleName)
        {
            // TODO: make it thread-safe
            StapleMarketData stapleMarketData;
            
            if (!this.stapleMarketDatas.TryGetValue(stapleName, out stapleMarketData))
            {
                throw new InvalidOperationException(string.Format("Staple with name '{0}' is not registered yet for market data. Call the Register method for it before you get it.", stapleName));
            }

            return stapleMarketData;
        }

        /// <summary>
        /// Stops all the registered <see cref="StapleMarketData" /> instances.
        /// </summary>
        public void Stop()
        {
            foreach (var stapleMarketData in this.stapleMarketDatas.Values)
            {
                stapleMarketData.Stop();
            }
        }
    }
}
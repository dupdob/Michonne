// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="PastaPricingAgent.cs" company="No lock... no deadlock" product="Michonne">
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
    /// Computes prices for a given pasta.
    /// </summary>
    public class PastaPricingAgent
    {
        private IEnumerable<IStapleMarketData> marketDatas;

        private decimal price;

        private List<string> marketDataToBeReceivedBeforeBeingAbleToPrice;

        private bool canPublishPrice;

        /// <summary>
        /// Initializes a new instance of the <see cref="PastaPricingAgent"/> class.
        /// </summary>
        /// <param name="pastaName">Name of the pasta.</param>
        public PastaPricingAgent(string pastaName)
        {
            this.PastaName = pastaName;
        }

        /// <summary>
        /// Occurs when the price of the pasta changed.
        /// </summary>
        public event EventHandler<PastaPriceChangedEventArgs> PastaPriceChanged;

        /// <summary>
        /// Gets the name of the pasta to price.
        /// </summary>
        /// <value>
        /// The name of the pasta to price.
        /// </value>
        public string PastaName { get; private set; }

        public void SubscribeToMarketData(IEnumerable<IStapleMarketData> marketDatas)
        {
            this.marketDatas = marketDatas;
            this.marketDataToBeReceivedBeforeBeingAbleToPrice = new List<string>();

            foreach (var stapleMarketData in this.marketDatas)
            {
                this.marketDataToBeReceivedBeforeBeingAbleToPrice.Add(stapleMarketData.StapleName);
                stapleMarketData.StaplePriceChanged += this.StapleMarketData_StaplePriceChanged;
            }
        }

        private void StapleMarketData_StaplePriceChanged(object sender, StaplePriceChangedEventArgs e)
        {
            // TODO: thread-safe this!
            if (!this.canPublishPrice)
            {
                if (this.marketDataToBeReceivedBeforeBeingAbleToPrice.Remove(e.StapleName))
                {
                    if (this.marketDataToBeReceivedBeforeBeingAbleToPrice.Count == 0)
                    {
                        this.canPublishPrice = true;
                    }
                }
            }

            if (this.canPublishPrice)
            {
                // Compute price
                this.price = e.Price;

                if (this.PastaPriceChanged != null)
                {
                    this.PastaPriceChanged(this, new PastaPriceChangedEventArgs(this.PastaName, this.price));
                }
            }
        }
    }
}
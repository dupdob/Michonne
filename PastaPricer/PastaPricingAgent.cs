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

    using Michonne;

    /// <summary>
    /// Computes prices for a given pasta.
    /// </summary>
    public sealed class PastaPricingAgent
    {
        private readonly Sequencer sequencer;

        private IEnumerable<IRawMaterialMarketData> marketDatas;

        private decimal price;

        private List<string> marketDataToBeReceivedBeforeBeingAbleToPrice;

        private bool canPublishPrice;

        private EventHandler<PastaPriceChangedEventArgs> pastaPriceChangedObservers;

        /// <summary>
        /// Initializes a new instance of the <see cref="PastaPricingAgent"/> class.
        /// </summary>
        /// <param name="sequencer">The sequencer to use for this agent (sequencer: race condition killer).</param>
        /// <param name="pastaName">Name of the pasta.</param>
        public PastaPricingAgent(Sequencer sequencer, string pastaName)
        {
            this.sequencer = sequencer;
            this.PastaName = pastaName;
        }

        /// <summary>
        /// Occurs when the price of the pasta changed.
        /// </summary>
        /// <remarks>The event subscription is thread-safe thanks to the instance's sequencer.</remarks>
        public event EventHandler<PastaPriceChangedEventArgs> PastaPriceChanged
        {
            add
            {
                this.sequencer.Dispatch(() => this.pastaPriceChangedObservers += value);
            }

            remove
            {
                this.sequencer.Dispatch(() => this.pastaPriceChangedObservers -= value);
            }
        }

        /// <summary>
        /// Gets the name of the pasta to price.
        /// </summary>
        /// <value>
        /// The name of the pasta to price.
        /// </value>
        public string PastaName { get; private set; }

        public void SubscribeToMarketData(IEnumerable<IRawMaterialMarketData> marketDatas)
        {
            this.marketDatas = marketDatas;
            this.marketDataToBeReceivedBeforeBeingAbleToPrice = new List<string>();

            foreach (var rawMaterialMarketData in this.marketDatas)
            {
                this.marketDataToBeReceivedBeforeBeingAbleToPrice.Add(rawMaterialMarketData.RawMaterialName);
                rawMaterialMarketData.PriceChanged += this.MarketData_PriceChanged;
            }
        }

        private void RaisePastaPriceChanged(decimal newPrice)
        {
            if (this.pastaPriceChangedObservers != null)
            {
                this.pastaPriceChangedObservers(this, new PastaPriceChangedEventArgs(this.PastaName, newPrice));
            }
        }

        /// <summary>
        /// Handles the PriceChanged event of the subscribed MarketData instances.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RawMaterialPriceChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This call back will be dispatched into the agent's sequencer. Thus, it is thread-safe.
        /// </remarks>
        private void MarketData_PriceChanged(object sender, RawMaterialPriceChangedEventArgs e)
        {
            this.sequencer.Dispatch(() =>
            {
                // TODO: code smells => refactor this code.
                if (!this.canPublishPrice)
                {
                    if (this.marketDataToBeReceivedBeforeBeingAbleToPrice.Remove(e.RawMaterialName))
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

                    this.RaisePastaPriceChanged(this.price);
                }    
            });
        }
    }
}
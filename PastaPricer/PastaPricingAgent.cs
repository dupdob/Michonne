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
    using System.Linq;

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

        private readonly IDictionary<RawMaterialRole, decimal> rawMaterialLatestPrices;

        private int numberOfRawMaterialInvolved;

        /// <summary>
        /// Initializes a new instance of the <see cref="PastaPricingAgent"/> class.
        /// </summary>
        /// <param name="sequencer">The sequencer to use for this agent (sequencer: race condition killer).</param>
        /// <param name="pastaName">Name of the pasta.</param>
        public PastaPricingAgent(Sequencer sequencer, string pastaName)
        {
            this.rawMaterialLatestPrices = new Dictionary<RawMaterialRole, decimal>();
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
            this.numberOfRawMaterialInvolved = marketDatas.Count();

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
                RawMaterialRole role;
                switch (e.RawMaterialName)
                {
                    case "flour":
                        role =RawMaterialRole.Flour;
                        break;
                    case "eggs":
                    case "organic eggs":
                        role =RawMaterialRole.Egg;
                        break;
                    case "tomato":
                    case "potatoes":
                    case "spinach":
                        role =RawMaterialRole.Flavor;
                        break;
                    default:
                        throw new ApplicationException(e.RawMaterialName+" unknown ingredient");
                }
                // Keeps the last value for this raw material
                this.rawMaterialLatestPrices[role] = e.Price;

                if (this.rawMaterialLatestPrices.Count < this.numberOfRawMaterialInvolved)
                {
                    return;
                }
                
                // Compute price
                var pastaCalculator = new PastaCalculator();

                if (this.numberOfRawMaterialInvolved == 3)
                {
                    this.price = pastaCalculator.Compute(this.rawMaterialLatestPrices[RawMaterialRole.Flour],
                        this.rawMaterialLatestPrices[RawMaterialRole.Egg],
                        this.rawMaterialLatestPrices[RawMaterialRole.Flavor]);
                }
                else
                {
                    this.price = 0;
                }

                this.RaisePastaPriceChanged(this.price);
            });
        }
    }
}
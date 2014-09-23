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
    using Michonne.Interfaces;

    /// <summary>
    ///     Computes prices for a given pasta.
    /// </summary>
    public sealed class PastaPricingAgent
    {
        #region Fields

        /// <summary>
        /// The sequencer.
        /// </summary>
        private readonly ISequencer pastaSequencer;

        private readonly IUnitOfExecution eggUnitOfExecution;
        private readonly IUnitOfExecution flourUnitOfExecution;
        private readonly IUnitOfExecution flavorUnitOfExecution;

        /// <summary>
        /// The egg price.
        /// </summary>
        private decimal? eggPrice;

        /// <summary>
        /// The flavor price.
        /// </summary>
        private decimal? flavorPrice;

        /// <summary>
        /// The flour price.
        /// </summary>
        private decimal? flourPrice;

        /// <summary>
        /// The market data.
        /// </summary>
        private IEnumerable<IRawMaterialMarketData> marketDatas;

        /// <summary>
        /// The pasta calculator.
        /// </summary>
        private PastaCalculator pastaCalculator;

        /// <summary>
        /// The pasta price changed observers.
        /// </summary>
        private EventHandler<PastaPriceChangedEventArgs> pastaPriceChangedObservers;

        /// <summary>
        /// The price.
        /// </summary>
        private decimal price;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PastaPricingAgent"/> class.
        /// </summary>
        /// <param name="pastaSequencer">
        ///     The sequencer to use for this agent (sequencer: race condition killer).
        /// </param>
        /// <param name="pastaName">
        ///     Name of the pasta.
        /// </param>
        /// <param name="conflationEnabled"></param>
        public PastaPricingAgent(ISequencer pastaSequencer, string pastaName, bool conflationEnabled = false)
        {
            this.pastaSequencer = pastaSequencer;

            if (conflationEnabled)
            {
                // Conflation with balking strategy
                this.eggUnitOfExecution = new BalkingDispatcher(this.pastaSequencer);
                this.flourUnitOfExecution = new BalkingDispatcher(this.pastaSequencer);
                this.flavorUnitOfExecution = new BalkingDispatcher(this.pastaSequencer);    
            }
            else
            {
                // All events processed
                this.eggUnitOfExecution = this.pastaSequencer;
                this.flourUnitOfExecution = this.pastaSequencer;
                this.flavorUnitOfExecution = this.pastaSequencer;
            }
            
            this.PastaName = pastaName;
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when the price of the pasta changed.
        /// </summary>
        /// <remarks>The event subscription is thread-safe thanks to the instance's sequencer.</remarks>
        public event EventHandler<PastaPriceChangedEventArgs> PastaPriceChanged
        {
            add
            {
                this.pastaSequencer.Dispatch(() => this.pastaPriceChangedObservers += value);
            }

            remove
            {
                this.pastaSequencer.Dispatch(() => this.pastaPriceChangedObservers -= value);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the name of the pasta to price.
        /// </summary>
        /// <value>
        ///     The name of the pasta to price.
        /// </value>
        public string PastaName { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// The subscribe to market data.
        /// </summary>
        /// <param name="marketDatas">
        /// The market data to subscribe to.
        /// </param>
        public void SubscribeToMarketData(IEnumerable<IRawMaterialMarketData> marketDatas)
        {
            this.pastaCalculator = new PastaCalculator();
            this.marketDatas = marketDatas;
            
            // ingredient prices are set at 0
            this.eggPrice = 0;
            this.flourPrice = 0;
            this.flavorPrice = 0;
            foreach (var rawMaterialMarketData in this.marketDatas)
            {
                // indentify the argument family, subscribe to it
                // and invalidate the current value.
                var role = PastaCalculator.ParseRawMaterialRole(rawMaterialMarketData.RawMaterialName);
                switch (role)
                {
                    case RawMaterialRole.Flour:
                        this.flourPrice = null;
                        rawMaterialMarketData.PriceChanged += this.MarketData_FlourPriceChanged;
                        break;
                    case RawMaterialRole.Egg:
                        this.eggPrice = null;
                        rawMaterialMarketData.PriceChanged += this.MarketData_EggPriceChanged;
                        break;
                    case RawMaterialRole.Flavor:
                        this.flavorPrice = null;
                        rawMaterialMarketData.PriceChanged += this.MarketData_FlavorPriceChanged;
                        break;
                }
            }
        }

        /// <summary>
        ///     Handles the PriceChanged event of the subscribed MarketData instances.
        /// </summary>
        /// <remarks>
        ///     This will be called from the agent's sequencer. Thus, it is thread-safe.
        /// </remarks>
        private void Compute()
        {
            if (this.HasAllRequestedInputsForComputation())
            {
                this.price = this.pastaCalculator.Compute(this.flourPrice.Value, this.eggPrice.Value, this.flavorPrice.Value);

                this.RaisePastaPriceChanged(this.price);
            }
            else
            {
                // this pasta price is stale

            }
        }

        /// <remarks>
        ///     This will be called from the agent's sequencer. Thus, it is thread-safe.
        /// </remarks>
        private bool HasAllRequestedInputsForComputation()
        {
            // TODO: cache the "true" value 
            return this.flourPrice.HasValue && this.eggPrice.HasValue && this.flavorPrice.HasValue;
        }

        /// <summary>
        /// The market data_ egg price changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MarketData_EggPriceChanged(object sender, RawMaterialPriceChangedEventArgs e)
        {
            this.eggUnitOfExecution.Dispatch(
                () =>
                    {
                        this.eggPrice = e.Price;
                        this.Compute();
                    });
        }

        /// <summary>
        /// The market data_ flavor price changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MarketData_FlavorPriceChanged(object sender, RawMaterialPriceChangedEventArgs e)
        {
            this.flavorUnitOfExecution.Dispatch(
                () =>
                    {
                        this.flavorPrice = e.Price;
                        this.Compute();
                    });
        }

        /// <summary>
        /// The market data_ flour price changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MarketData_FlourPriceChanged(object sender, RawMaterialPriceChangedEventArgs e)
        {
            this.flourUnitOfExecution.Dispatch(
                () =>
                    {
                        this.flourPrice = e.Price;
                        this.Compute();
                    });
        }

        /// <summary>
        /// The raise pasta price changed.
        /// </summary>
        /// <param name="newPrice">
        /// The new price.
        /// </param>
        /// /// <remarks>
        ///     This will be called from the agent's sequencer. Thus, it is thread-safe.
        /// </remarks>
        private void RaisePastaPriceChanged(decimal newPrice)
        {
            if (this.pastaPriceChangedObservers != null)
            {
                this.pastaPriceChangedObservers(this, new PastaPriceChangedEventArgs(this.PastaName, newPrice));
            }
        }

        #endregion
    }
}
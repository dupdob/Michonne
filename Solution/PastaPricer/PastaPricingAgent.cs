// ---------------------------------------------------------------------------------------------------------------------
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
        private readonly IUnitOfExecution packagingUnitOfExecution;
        private readonly IUnitOfExecution sizeUnitOfExecution;

        /// <summary>
        /// The ingredient prices.
        /// </summary>
        private decimal? eggPrice;
        private decimal? flavorPrice;
        private decimal? flourPrice;
        private decimal? sizePrice;
        private decimal? packagingPrice;

        private IEnumerable<IRawMaterialMarketData> marketDatas;

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
        /// <param name="conflationEnabled">Set to true to activate conflation.</param>
        public PastaPricingAgent(ISequencer pastaSequencer, string pastaName, bool conflationEnabled = false)
        {
            this.pastaSequencer = pastaSequencer;

            if (conflationEnabled)
            {
                // Conflation with balking strategy
                this.eggUnitOfExecution = new BalkingDispatcher(this.pastaSequencer);
                this.flourUnitOfExecution = new BalkingDispatcher(this.pastaSequencer);
                this.flavorUnitOfExecution = new BalkingDispatcher(this.pastaSequencer);
                this.packagingUnitOfExecution = new BalkingDispatcher(this.pastaSequencer);
                this.sizeUnitOfExecution = new BalkingDispatcher(this.pastaSequencer);
            }
            else
            {
                // for testing purposes
                var exec = pastaSequencer;

                // All events processed
                this.eggUnitOfExecution = exec;
                this.flourUnitOfExecution = exec;
                this.flavorUnitOfExecution = exec;
                this.packagingUnitOfExecution = exec;
                this.sizeUnitOfExecution = exec;
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
                // ReSharper disable once DelegateSubtraction
                this.pastaSequencer.Dispatch(() => this.pastaPriceChangedObservers -= value);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the pasta to price.
        /// </summary>
        /// <value>
        ///     The name of the pasta to price.
        /// </value>
        private string PastaName { get; set; }

        #endregion

        #region Methods

        #region Constructor

        #endregion

        /// <summary>
        /// The subscribe to market data.
        /// </summary>
        public void SubscribeToMarketData(IEnumerable<IRawMaterialMarketData> sourceMarketDatas)
        {
            this.marketDatas = sourceMarketDatas;

            // ingredient prices are set at 0
            this.eggPrice = 0;
            this.flourPrice = 0;
            this.flavorPrice = 0;
            this.sizePrice = null;
            this.packagingPrice = null;
            foreach (var rawMaterialMarketData in this.marketDatas)
            {
                // indentify the argument family, subscribe to it
                // and invalidate the current value.
                var role = RecipeHelper.ParseRawMaterialRole(rawMaterialMarketData.RawMaterialName);
                switch (role)
                {
                    case RawMaterialRole.Flour:
                        this.flourPrice = null;
                        rawMaterialMarketData.PriceChanged += this.MarketDataFlourPriceChanged;
                        break;
                    case RawMaterialRole.Egg:
                        this.eggPrice = null;
                        rawMaterialMarketData.PriceChanged += this.MarketDataEggPriceChanged;
                        break;
                    case RawMaterialRole.Flavor:
                        this.flavorPrice = null;
                        rawMaterialMarketData.PriceChanged += this.MarketDataFlavorPriceChanged;
                        break;
                    case RawMaterialRole.Size:
                        this.sizePrice = null;
                        rawMaterialMarketData.PriceChanged += this.MarketDataSizePriceChanged;
                        break;
                    case RawMaterialRole.Packaging:
                        this.packagingPrice = null;
                        rawMaterialMarketData.PriceChanged += this.MarketDataPackagingPriceChanged;
                        break;
                }
            }
        }


        // price update handles
        private void MarketDataPackagingPriceChanged(object sender, RawMaterialPriceChangedEventArgs e)
        {
            // forward the notification to the sequencer
            this.packagingUnitOfExecution.Dispatch(
                () =>
                {
                    // capture the price
                    this.packagingPrice = e.Price;

                    // computes an updated price
                    this.Compute();
                });
        }

        private void MarketDataSizePriceChanged(object sender, RawMaterialPriceChangedEventArgs e)
        {
            // forward the notification to the sequencer
            this.sizeUnitOfExecution.Dispatch(
                () =>
                {
                    this.sizePrice = e.Price;
                    this.Compute();
                });
        }

        private void MarketDataEggPriceChanged(object sender, RawMaterialPriceChangedEventArgs e)
        {
            // forward the notification to the sequencer
            this.eggUnitOfExecution.Dispatch(
                () =>
                {
                    this.eggPrice = e.Price;
                    this.Compute();
                });
        }

        private void MarketDataFlavorPriceChanged(object sender, RawMaterialPriceChangedEventArgs e)
        {
            this.flavorUnitOfExecution.Dispatch(
                () =>
                {
                    this.flavorPrice = e.Price;
                    this.Compute();
                });
        }

        private void MarketDataFlourPriceChanged(object sender, RawMaterialPriceChangedEventArgs e)
        {
            this.flourUnitOfExecution.Dispatch(
                () =>
                {
                    this.flourPrice = e.Price;
                    this.Compute();
                });
        }

        // pseudo computation logic
        private void Compute()
        {
            if (!this.HasAllRequestedInputsForComputation())
            {
                return;
            }

            this.price = PastaCalculator.Compute(this.flourPrice.Value, this.eggPrice.Value, this.flavorPrice.Value, this.sizePrice, this.packagingPrice);

            this.RaisePastaPriceChanged(this.price);
        }

        /// <summary>
        /// The has all requested inputs for computation.
        /// </summary>
        /// <remarks>
        /// This will be called from the agent's sequencer. Thus, it is thread-safe.
        /// </remarks>
        private bool HasAllRequestedInputsForComputation()
        {
            // TODO: cache the "true" value 
            return this.flourPrice.HasValue && this.eggPrice.HasValue && this.flavorPrice.HasValue;
        }

        /// <summary>
        /// Notifies pasta price changed.
        /// </summary>
        /// <remarks>
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
        #region Overrides of Object

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.PastaName;
        }

        #endregion

    }
}
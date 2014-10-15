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

    using Michonne.Implementation;
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

        private readonly IDataProcessor<decimal> flourProcessor; 
        private readonly IDataProcessor<decimal> eggProcessor; 
        private readonly IDataProcessor<decimal> flavorProcessor; 
        private readonly IDataProcessor<decimal> packagingProcessor; 
        private readonly IDataProcessor<decimal> sizeProcessor; 
        
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
        /// The pasta price changed observers.
        /// </summary>
        private EventHandler<PastaPriceChangedEventArgs> pastaPriceChangedObservers;

        /// <summary>
        /// The price.
        /// </summary>
        private decimal price;

        private decimal? sizePrice;

        private decimal? packagingPrice;

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

            this.flourProcessor = this.pastaSequencer.BuildProcessor<decimal>(this.FlourUpdate, conflationEnabled);
            this.eggProcessor = this.pastaSequencer.BuildProcessor<decimal>(this.EggUpdate, conflationEnabled);
            this.flavorProcessor = this.pastaSequencer.BuildProcessor<decimal>(this.FlavorUpdate, conflationEnabled);
            this.packagingProcessor = this.pastaSequencer.BuildProcessor<decimal>(this.PackagingUpdate, conflationEnabled);
            this.sizeProcessor = this.pastaSequencer.BuildProcessor<decimal>(this.SizeUpdate, conflationEnabled);
            
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
        /// <param name="sourceMarketDatas">
        /// The market data to subscribe to.
        /// </param>
        public void SubscribeToMarketData(IEnumerable<IRawMaterialMarketData> sourceMarketDatas)
        {
            this.marketDatas = sourceMarketDatas;

            // ingredient prices are set at 0
            this.eggPrice = 0;
            this.flourPrice = 0;
            this.flavorPrice = 0;
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

        private void MarketDataPackagingPriceChanged(object sender, RawMaterialPriceChangedEventArgs e)
        {
            this.packagingProcessor.Post(e.Price);
        }

        private void MarketDataSizePriceChanged(object sender, RawMaterialPriceChangedEventArgs e)
        {
            this.sizeProcessor.Post(e.Price);
        }

        /// <summary>
        /// The egg price change notification handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// Event details.
        /// </param>
        private void MarketDataEggPriceChanged(object sender, RawMaterialPriceChangedEventArgs e)
        {
            this.eggProcessor.Post(e.Price);
        }

        /// <summary>
        ///     Handles the PriceChanged event of the subscribed MarketData instances.
        /// </summary>
        /// <remarks>
        ///     This will be called from the agent's sequencer. Thus, it is thread-safe.
        /// </remarks>
        private void Compute()
        {
            if (!this.HasAllRequestedInputsForComputation())
            {
                return;
            }
            // ReSharper disable once PossibleInvalidOperationException
            this.price = PastaCalculator.Compute(this.flourPrice.Value, this.eggPrice.Value, this.flavorPrice.Value, this.sizePrice, this.packagingPrice);

            this.RaisePastaPriceChanged(this.price);
        }

        /// <summary>
        /// The has all requested inputs for computation.
        /// </summary>
        /// <remarks>
        /// This will be called from the agent's sequencer. Thus, it is thread-safe.
        /// </remarks>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool HasAllRequestedInputsForComputation()
        {
            // TODO: cache the "true" value 
            return this.flourPrice.HasValue && this.eggPrice.HasValue && this.flavorPrice.HasValue;
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

        #region other price handlers

        /// <summary>
        /// The market data_ flavor price changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MarketDataFlavorPriceChanged(object sender, RawMaterialPriceChangedEventArgs e)
        {
            this.flavorProcessor.Post(e.Price);
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
        private void MarketDataFlourPriceChanged(object sender, RawMaterialPriceChangedEventArgs e)
        {
            this.flourProcessor.Post(e.Price);
        }

        private void SizeUpdate(decimal newPrice)
        {
            this.sizePrice = newPrice;
            this.Compute();
        }

        private void PackagingUpdate(decimal newPrice)
        {
            this.packagingPrice = newPrice;
            this.Compute();
        }

        private void FlavorUpdate(decimal newPrice)
        {
            this.flavorPrice = newPrice;
            this.Compute();
        }

        private void FlourUpdate(decimal newPrice)
        {
            this.flourPrice = newPrice;
            this.Compute();
        }

        private void EggUpdate(decimal newPrice)
        {
            this.eggPrice = newPrice;
            this.Compute();
        }
        #endregion

        #endregion
    }
}
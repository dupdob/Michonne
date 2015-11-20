namespace PastaPricer
{
    using System;
    using System.Collections.Generic;

    using Michonne.Implementation;
    using Michonne.Interfaces;

    /// <summary>
    ///     Computes prices for a given pasta.
    /// </summary>
    public sealed class PastaPricingAgentNewAPI
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
        /// The component prices.
        /// </summary>
        private decimal? eggPrice;
        private decimal? flavorPrice;
        private decimal? flourPrice;
        private decimal? sizePrice;
        private decimal? packagingPrice;
        
        private IEnumerable<IRawMaterialMarketData> marketDatas;
        private EventHandler<PastaPriceChangedEventArgs> pastaPriceChangedObservers;

        private decimal price;


        #endregion

        #region Constructors and Destructors

        // constructor
        public PastaPricingAgentNewAPI(ISequencer pastaSequencer, string pastaName, bool conflationEnabled = false)
        {
            this.pastaSequencer = pastaSequencer;

            this.PastaName = pastaName;

            // data processor
            this.flourProcessor = this.pastaSequencer.BuildProcessor<decimal>(this.FlourUpdate, conflationEnabled);
            this.eggProcessor = this.pastaSequencer.BuildProcessor<decimal>(this.EggUpdate, conflationEnabled);
            this.flavorProcessor = this.pastaSequencer.BuildProcessor<decimal>(this.FlavorUpdate, conflationEnabled);
            this.packagingProcessor = this.pastaSequencer.BuildProcessor<decimal>(this.PackagingUpdate, conflationEnabled);
            this.sizeProcessor = this.pastaSequencer.BuildProcessor<decimal>(this.SizeUpdate, conflationEnabled);
        }

        #endregion

        #region Public Events
        // raised when the price is computed
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

        private string PastaName { get; set; }

        #endregion

        #region Methods

        #region Constructor

        #endregion

        // initialization logic
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

        private void MarketDataEggPriceChanged(object sender, RawMaterialPriceChangedEventArgs e)
        {
            this.eggProcessor.Post(e.Price);
        }

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

        private bool HasAllRequestedInputsForComputation()
        {
            // TODO: cache the "true" value 
            return this.flourPrice.HasValue && this.eggPrice.HasValue && this.flavorPrice.HasValue;
        }

        private void RaisePastaPriceChanged(decimal newPrice)
        {
            this.pastaPriceChangedObservers?.Invoke(this, new PastaPriceChangedEventArgs(this.PastaName, newPrice));
        }

        #region other price handlers

        private void MarketDataFlavorPriceChanged(object sender, RawMaterialPriceChangedEventArgs e)
        {
            this.flavorProcessor.Post(e.Price);
        }

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
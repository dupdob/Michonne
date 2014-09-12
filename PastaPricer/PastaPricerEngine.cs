namespace PastaPricer
{
    using System;

    public class PastaPricerEngine
    {
        private readonly IMarketDataProvider marketDataProvider;
        private readonly IPastaPricerPublisher pastaPricerPublisher;

        public PastaPricerEngine(IMarketDataProvider marketDataProvider, IPastaPricerPublisher pastaPricerPublisher)
        {
            this.marketDataProvider = marketDataProvider;
            this.pastaPricerPublisher = pastaPricerPublisher;
        }

        public void Start()
        {
            this.marketDataProvider.Get("eggPrice").PriceChanged += this.PastaPricerEngine_PriceChanged;
        }

        private void PastaPricerEngine_PriceChanged(object sender, EventArgs e)
        {
            this.pastaPricerPublisher.Publish(string.Empty, 0);
        }
    }
}

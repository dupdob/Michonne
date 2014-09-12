namespace PastaPricer.Tests
{
    public class MarketDataProvider : IMarketDataProvider
    {
        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public MarketData Get(string assetName)
        {
            return new MarketData();
        }
    }
}
namespace PastaPricer
{
    public interface IMarketDataProvider
    {
        void Start();

        MarketData Get(string assetName);
    }
}
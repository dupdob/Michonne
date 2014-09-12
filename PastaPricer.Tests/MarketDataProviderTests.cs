namespace PastaPricer.Tests
{
    using NFluent;
    using NUnit.Framework;

    [TestFixture]
    public class MarketDataProviderTests
    {
        [Test]
        public void Should_Provide_MarketData_For_Eggs()
        {
            var marketDataProvider = new MarketDataProvider();

            Check.That(marketDataProvider.Get("eggs")).IsInstanceOf<MarketData>();
        }
    }
}

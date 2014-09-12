namespace PastaPricer.Tests.Acceptance
{
    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class PastaPricerAcceptanceTests
    {
        [Test]
        public void Should_Publish_Pasta_Price_Once_Started_And_MarketData_Is_Available()
        {
            // Mocks instantiation
            var publisher = Substitute.For<IPastaPricerPublisher>();
            var marketDataProvider = new MarketDataProvider();
            
            var pastaPricer = new PastaPricerEngine(marketDataProvider, publisher);
            
            publisher.DidNotReceiveWithAnyArgs().Publish(string.Empty, 0);
            
            pastaPricer.Start();

            publisher.DidNotReceiveWithAnyArgs().Publish(string.Empty, 0);

            marketDataProvider.Start();

            // It publishes!
            publisher.ReceivedWithAnyArgs().Publish(string.Empty, 0);
        }
    }
}
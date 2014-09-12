namespace PastaPricer
{
    public interface IPastaPricerPublisher
    {
        void Publish(string pastaIdentifier, decimal price);
    }
}
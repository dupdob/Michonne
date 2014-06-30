namespace Michonne.Tests
{
    using System.Threading;
    using NFluent;
    using NUnit.Framework;

    [TestFixture]
    public class DirectDispatcherTests
    {
        private string executingThreadName;

        [SetUp]
        public void SetUp()
        {
            this.executingThreadName = null;
        }

        [Test]
        public void DirectDispatcherExecutesTasksInTheCallerThread()
        {
            var directDispatcher = new DirectDispatcher();
            Thread.CurrentThread.Name = "unit Test thread";
            
            directDispatcher.Dispatch( () => this.executingThreadName = Thread.CurrentThread.Name);
            
            Check.That(this.executingThreadName).IsEqualTo(Thread.CurrentThread.Name).And.IsNotNull();
        }
    }
}

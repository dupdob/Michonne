namespace SequencerAiiiight.Tests
{
    using System.Collections.Generic;
    using System.Threading;

    using NFluent;

    using NUnit.Framework;

    [TestFixture]
    public class SequencerTests
    {
        #region Constants

        private const int SecondInMsec = 1000;

        #endregion

        #region Fields

        private readonly object syncRoot = new object();

        private List<int> result;

        private AutoResetEvent sequenceFinished;

        #endregion

        #region SetUp/TearDown

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this.sequenceFinished = new AutoResetEvent(false);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            if (this.sequenceFinished != null)
            {
                this.sequenceFinished.Dispose();
                this.sequenceFinished = null;
            }
        }

        #endregion

        #region Tests

        [Test]
        public void SequencerExecutesTasksInTheOrderOfTheirDispatch()
        {
            var sequencer = new Sequencer();
            const int TasksNumber = 100000;
            
            this.result = new List<int>(TasksNumber);

            // loads the sequencer
            for (var i = 0; i < TasksNumber; i++)
            {
                var antiClosureSideEffectNumber = i;
                sequencer.Dispatch(() => this.result.Add(antiClosureSideEffectNumber));
            }

            // Indicates the end of the sequence
            sequencer.Dispatch(() => this.sequenceFinished.Set()) ;

            // use an autoreset event here instead
            Check.That(this.sequenceFinished.WaitOne(5 * SecondInMsec)).IsTrue();

            // Checks that everything was properly processed in sequence
            for (int k = 0; k < TasksNumber; k++)
            {
                Check.That(this.result[k]).IsEqualTo(k);
            }
        }

        #endregion
    }
}
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

        #region Public Methods and Operators

        [Test]
        public void SequencerExecutesTasksInTheOrderOfTheirDispatch()
        {
            var sequencer = new Sequencer();
            const int MaxNumber = 10000;
            
            this.result = new List<int>(MaxNumber);

            // loads the sequencer
            for (var i = 0; i < MaxNumber; i++)
            {
                var antiClosureSideEffectNumber = i;
                sequencer.Dispatch(() =>
                {
                    lock (this.syncRoot)
                    {
                        this.result.Add(antiClosureSideEffectNumber);
                    }
                });
            }

            // Indicates the end of the sequence
            sequencer.Dispatch(() => this.sequenceFinished.Set()) ;

            // use an autoreset event here instead
            Check.That(this.sequenceFinished.WaitOne(5 * SecondInMsec)).IsTrue();

            // Checks that everything was properly processed in sequence
            for (int k = 0; k < MaxNumber; k++)
            {
                Check.That(this.result[k]).IsEqualTo(k);
            }
        }

        

        #endregion
    }
}
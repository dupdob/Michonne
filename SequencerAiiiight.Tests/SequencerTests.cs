namespace SequencerAiiiight.Tests
{
    using System.Collections.Generic;
    using System.Threading;

    using NFluent;
    using NUnit.Framework;

    [TestFixture]
    public class SequencerTests
    {
        #region Gin and Juice (fields)

        private const int SecondInMsec = 1000;
        private List<int> tasksOutput;
        private AutoResetEvent sequenceFinished;

        #endregion

        #region Bang bang (SetUp/TearDown)

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

        #region Mos Def (Tests)

        [Test]
        public void SequencerExecutesTasksInTheOrderOfTheirDispatch()
        {
            var sequencer = new Sequencer();
            const int TasksNumber = 100000;
            
            this.tasksOutput = new List<int>(TasksNumber);

            // Dispatches tasks to the sequencer
            for (var i = 0; i < TasksNumber; i++)
            {
                var antiClosureSideEffectNumber = i;
                sequencer.Dispatch(() => this.tasksOutput.Add(antiClosureSideEffectNumber));
            }

            // Indicates the end of the sequence with a final task
            sequencer.Dispatch(() => this.sequenceFinished.Set()) ;

            // Waits for sequence completion
            Check.That(this.sequenceFinished.WaitOne(5 * SecondInMsec)).IsTrue();

            // Checks that everything was properly processed in sequence
            for (int k = 0; k < TasksNumber; k++)
            {
                Check.That(this.tasksOutput[k]).IsEqualTo(k);
            }
        }

        #endregion
    }
}
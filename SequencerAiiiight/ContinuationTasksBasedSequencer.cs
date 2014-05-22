namespace SequencerAiiiight
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Sequencer implementation provided by Olivier COANET on Cyrille's blog.
    /// (has to test more and to review the TPL continuation task implementation to check whether 
    /// or not this implementation fulfill all the Sequencer requirements:
    ///     http://dupdob.wordpress.com/2014/05/09/the-sequencer-part-2/
    ///     and 
    ///     http://dupdob.wordpress.com/2014/05/14/sequencer-part-2-1/
    /// </summary>
    public class ContinuationTasksBasedSequencer
    {
        private readonly object _lock = new object();
        private Task _task = Task.FromResult(0);

        public void Dispatch(Action action)
        {
            lock (_lock)
            {
                _task = _task.ContinueWith(_ => action());
            }
        }
    }
}
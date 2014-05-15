namespace SequencerAiiiight
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// Incorrect implementation of the Sequencer from Cyrille's blog (http://dupdob.wordpress.com/2014/05/09/the-sequencer-part-2/)
    /// </summary>
    public class Sequencer
    {
        #region Fields

        private readonly object _lock = new Object();

        private readonly Queue<Action> _pendingTasks = new Queue<Action>();

        private bool _isRunning;

        #endregion

        #region Public Methods and Operators

        public void Dispatch(Action action)
        {
            // he he he... ;-)

            // here we queue a call to our own logic
            ThreadPool.QueueUserWorkItem(x => this.Run((Action)x), action);
        }

        #endregion

        // run when the pool has available cpu time for us.

        #region Methods

        private void Run(Action action)
        {
            lock (this._lock)
            {
                if (this._isRunning)
                {
                    // we need to store and stop
                    this._pendingTasks.Enqueue(action);
                    return;
                }
                // ok, we can run
                this._isRunning = true;
            }

            while (true)
            {
                // execute the next action
                action();
                // we check if others are available
                lock (this._lock)
                {
                    if (this._pendingTasks.Count == 0)
                    {
                        this._isRunning = false;
                        return;
                    }
                    // pop the next task
                    action = this._pendingTasks.Dequeue();
                }
            }
        }

        #endregion
    }
}
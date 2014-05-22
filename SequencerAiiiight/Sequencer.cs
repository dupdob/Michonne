namespace SequencerAiiiight
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    

    /// <summary>
    /// Allows to execute tasks asynchronously, but one by one and in the same order as they have been dispatched.
    /// That means that two tasks from the same dispatcher can be executed by two different threads, but not in parallel. 
    /// Sequencer requirements are presented here: 
    ///     http://dupdob.wordpress.com/2014/05/09/the-sequencer-part-2/
    ///     and
    ///     http://dupdob.wordpress.com/2014/05/14/sequencer-part-2-1/
    /// <remarks>
    ///     Even if using this dispatcher will allow you to erradicate locks from your application code, 
    ///     this particular implementation of the sequencer is not a lock-free.
    /// </remarks>
    /// </summary>
    public class Sequencer
    {
        // TODO: clean up some code and refactor it.

        #region Fields

        private readonly object _lock = new Object();

        private readonly Queue<Action> _orderedDispatchedTasks = new Queue<Action>();
        private readonly Queue<Action> _pendingTasks = new Queue<Action>();

        private bool _isRunning;

        #endregion

        #region Public Methods and Operators

        public void Dispatch(Action action)
        {
            SequencedTask sequencedTask = null;
            // he he he... ;-)
            lock (this._lock)
            {
                sequencedTask = new SequencedTask(this);
                this._orderedDispatchedTasks.Enqueue(action);
            }

            // here we queue a call to our own logic
            ThreadPool.QueueUserWorkItem(x => sequencedTask.Execute());
        }

        #endregion

        /// <summary>
        /// Sequenced task that has been dispatched.
        /// </summary>
        private class SequencedTask
        {
            private Sequencer sequencer;

            public SequencedTask(Sequencer sequencer)
            {
                this.sequencer = sequencer;
            }

            public void Execute()
            {
                // run when the pool has available cpu time for us.
                Action action = null;

                lock (this.sequencer._lock)
                {
                    action = this.sequencer._orderedDispatchedTasks.Dequeue(); 

                    if (this.sequencer._isRunning)
                    {
                        // we need to store and stop
                        this.sequencer._pendingTasks.Enqueue(action);
                        return;
                    }
                    // ok, we can run
                    this.sequencer._isRunning = true;
                }

                while (true)
                {
                    // execute the next action
                    action();
                    // we check if others are available
                    lock (this.sequencer._lock)
                    {
                        if (this.sequencer._pendingTasks.Count == 0)
                        {
                            this.sequencer._isRunning = false;
                            return;
                        }
                        // pop the next task
                        action = this.sequencer._pendingTasks.Dequeue();
                    }
                }
            }
        }
    }
}
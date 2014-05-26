namespace SequencerAiiiight
{
    using System;
    using System.Collections.Generic;
    using SequencerAiiiight.Interfaces;

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
    public class Sequencer : ISequencer
    {
        // TODO: still not optimized in term of concurrency => clean up some code and refactor it.
        #region Fields

        private readonly object syncRoot = new object();
        private readonly IDispatcher rootDispatcher;
        private readonly Queue<Action> orderedDispatchedTasks = new Queue<Action>();
        private readonly Queue<Action> pendingTasks = new Queue<Action>();
        private bool isRunning;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Sequencer"/> class.
        /// </summary>
        /// <param name="rootDispatcher">The root Dispatcher.</param>
        public Sequencer(IDispatcher rootDispatcher)
        {
            this.rootDispatcher = rootDispatcher;
        }

        #region Public Methods and Operators

        /// <summary>
        /// Gives a task/action to the sequencer in order to execute it in an asynchronous manner, but respecting the
        /// order of the dispatch, and without concurrency among the sequencer's tasks.
        /// </summary>
        /// <param name="action"></param>
        public void Dispatch(Action action)
        {
            lock (this.syncRoot)
            {
                this.orderedDispatchedTasks.Enqueue(action);
            }

            // wraps the taks and dispatchs it to the thread pool (TODO: use TPL or I/I completion ports instead)
            var sequencedTask = new SequencedTask(this);

            // Dispatches the sequenced task to the underlying rootDispatcher
            this.rootDispatcher.Dispatch(sequencedTask.Execute);
        }

        #endregion

        /// <summary>
        /// Task that has been dispatched by the sequencer.
        /// </summary>
        private class SequencedTask
        {
            private readonly Sequencer sequencer;

            public SequencedTask(Sequencer sequencer)
            {
                this.sequencer = sequencer;
            }

            /// <summary>
            /// Executes this dispatched task.
            /// </summary>
            public void Execute()
            {
                Action action;

                lock (this.sequencer.syncRoot)
                {
                    action = this.sequencer.orderedDispatchedTasks.Dequeue(); 

                    if (this.sequencer.isRunning)
                    {
                        // we need to store and stop
                        this.sequencer.pendingTasks.Enqueue(action);
                        return;
                    }
                    // ok, we can run
                    this.sequencer.isRunning = true;
                }

                while (true)
                {
                    // execute the next action
                    action();
                    // we check if others are available
                    lock (this.sequencer.syncRoot)
                    {
                        if (this.sequencer.pendingTasks.Count == 0)
                        {
                            this.sequencer.isRunning = false;
                            return;
                        }
                        // pop the next task
                        action = this.sequencer.pendingTasks.Dequeue();
                    }
                }
            }
        }
    }
}
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
    public sealed class Sequencer : ISequencer
    {
        // TODO: Get rid of the Queue, implement lock free algo, etc.
        #region Fields

        private readonly object syncRoot = new object();
        private readonly IDispatcher rootDispatcher;
        private readonly Queue<Action> orderedDispatchedTasks = new Queue<Action>();
        private long numberOfPendingTasksWhileRunning;
        private bool isRunning;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Sequencer"/> class.
        /// </summary>
        /// <param name="rootDispatcher">The root Dispatcher.</param>
        public Sequencer(IDispatcher rootDispatcher)
        {
            this.rootDispatcher = rootDispatcher;
            this.numberOfPendingTasksWhileRunning = 0;
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

            var sequencedTask = new SequencedTask(this);

            // Dispatches the sequenced task to the underlying rootDispatcher
            this.rootDispatcher.Dispatch(sequencedTask.Execute);
        }

        #endregion

        /// <summary>
        /// Task that has been dispatched by the sequencer and that should be executed by its root dispatcher.
        /// </summary>
        private sealed class SequencedTask
        {
            private readonly Sequencer sequencer;

            /// <summary>
            /// Initializes a new instance of the <see cref="SequencedTask"/> class.
            /// </summary>
            /// <param name="sequencer">The sequencer.</param>
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
                    if (this.sequencer.isRunning)
                    {
                        // we need to store and stop
                        this.sequencer.numberOfPendingTasksWhileRunning++;
                        return;
                    }
                    
                    action = this.sequencer.orderedDispatchedTasks.Dequeue(); 

                    // ok, we can run
                    this.sequencer.isRunning = true;
                }

                while (true)
                {
                    // execute the next action
                    action();
                    // we check if others tasks have to be executed during this round
                    lock (this.sequencer.syncRoot)
                    {
                        if (this.sequencer.numberOfPendingTasksWhileRunning == 0)
                        {
                            this.sequencer.isRunning = false;
                            return;
                        }
                        
                        // pop the next task
                        if(this.sequencer.numberOfPendingTasksWhileRunning > 0)
                        {
                            this.sequencer.numberOfPendingTasksWhileRunning--;
                            action = this.sequencer.orderedDispatchedTasks.Dequeue();
                        }
                    }
                }
            }
        }
    }
}
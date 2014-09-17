// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="Sequencer.cs" company="No lock... no deadlock" product="Michonne">
//     Copyright 2014 Cyrille DUPUYDAUBY (@Cyrdup), Thomas PIERRAIN (@tpierrain)
//     Licensed under the Apache License, Version 2.0 (the "License");
//     you may not use this file except in compliance with the License.
//     You may obtain a copy of the License at
//         http://www.apache.org/licenses/LICENSE-2.0
//     Unless required by applicable law or agreed to in writing, software
//     distributed under the License is distributed on an "AS IS" BASIS,
//     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//     See the License for the specific language governing permissions and
//     limitations under the License.
//   </copyright>
//   --------------------------------------------------------------------------------------------------------------------
namespace Michonne.Implementation
{
    using System;
    using System.Collections.Generic;

    using Michonne.Interfaces;

    /// <summary>
    ///     Allows to execute tasks asynchronously, but one by one and in the same order as they have been dispatched.
    ///     That means that two tasks from the same dispatcher can be executed by two different threads, but not in parallel.
    ///     Sequencer requirements are presented here:
    ///     http://dupdob.wordpress.com/2014/05/09/the-sequencer-part-2/
    ///     and
    ///     http://dupdob.wordpress.com/2014/05/14/sequencer-part-2-1/
    ///     <remarks>
    ///         Even if using this dispatcher will allow you to eradicate locks from your application code,
    ///         this particular implementation of the sequencer is not a lock-free.
    ///     </remarks>
    /// </summary>
    public sealed class Sequencer : ISequencer
    {
        // TODO: Get rid of the Queue, implement lock free algo, etc.
        #region Fields
        /// <summary>
        /// In charge of maintaining task order
        /// </summary>
        private readonly Queue<Action> orderedDispatchedTasks = new Queue<Action>();
        
        /// <summary>
        /// Underlying unit of execution that will actually execute tasks.
        /// </summary>
        private readonly IUnitOfExecution rootUnitOfExecution;
        
        /// <summary>
        /// private lock
        /// </summary>
        private readonly object syncRoot = new object();

        /// <summary>
        /// Set to true when tasks are being executed.
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// Number of tasks to be executed (to prevent unfair draining of tasks).
        /// </summary>
        private long numberOfPendingTasksWhileRunning;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Sequencer" /> class.
        /// </summary>
        /// <param name="rootUnitOfExecution">The root Dispatcher.</param>
        public Sequencer(IUnitOfExecution rootUnitOfExecution)
        {
            this.rootUnitOfExecution = rootUnitOfExecution;
            this.numberOfPendingTasksWhileRunning = 0;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gives a task/action to the sequencer in order to execute it in an asynchronous manner, but respecting the
        ///     order of the dispatch, and without concurrency among the sequencer's tasks.
        /// </summary>
        /// <param name="action">The action to be executed</param>
        public void Dispatch(Action action)
        {
            lock (this.syncRoot)
            {
                this.orderedDispatchedTasks.Enqueue(action);
            }

            // Dispatches the sequenced task to the underlying rootUnitOfExecution
            this.rootUnitOfExecution.Dispatch(this.Execute);
        }

        /// <summary>
        ///     Executes this dispatched task.
        /// </summary>
        private void Execute()
        {
            Action action;
            lock (this.syncRoot)
            {
                if (this.isRunning)
                {
                    // We need to store and stop
                    this.numberOfPendingTasksWhileRunning++;
                    return;
                }

                // Ok, we can run
                action = this.orderedDispatchedTasks.Dequeue();
                this.isRunning = true;
            }

            while (true)
            {
                // Execute the next action
                action();

                // We check if others tasks have to be executed during this round
                lock (this.syncRoot)
                {
                    if (this.numberOfPendingTasksWhileRunning == 0)
                    {
                        this.isRunning = false;
                        return;
                    }

                    // Pop the next task we are allowed to execute now (for fairness with dispatcher's other tasks)
                    if (this.numberOfPendingTasksWhileRunning > 0)
                    {
                        this.numberOfPendingTasksWhileRunning--;
                        action = this.orderedDispatchedTasks.Dequeue();
                    }
                }
            }
        }
        #endregion
    }
}
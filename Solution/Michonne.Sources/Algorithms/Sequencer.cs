﻿// --------------------------------------------------------------------------------------------------------------------
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
#if !NET20 && !NET30
    using System;
#endif
#if NET40 || NET45 || NETSTANDARD1_3
    using System.Collections.Concurrent;
#else
    using System.Collections.Generic;
#endif
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    using Interfaces;

#if NET20 || NET30 || NET35

    internal class ConcurrentQueue<T> where T: class
    {
        private readonly Queue<T> queue = new Queue<T>();
        public bool TryDequeue(out T item)
        {
            lock (this.queue)
            {
                if (this.queue.Count == 0)
                {
                    item = null;
                    return false;
                }
                item = this.queue.Dequeue();
                return true;
            }
        }

        public void Enqueue(T item)
        {
            lock (this.queue)
            {
                this.queue.Enqueue(item);
            }
        }
    }
#endif

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
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class Sequencer : ISequencer
    {
        /// <summary>
        /// In charge of maintaining task order
        /// </summary>
        private readonly ConcurrentQueue<Action> orderedDispatchedTasks = new ConcurrentQueue<Action>();

        /// <summary>
        /// Underlying unit of execution that will actually execute tasks.
        /// </summary>
        private readonly IExecutor rootUnitOfExecution;

        /// <summary>
        /// Number of tasks to be executed (to prevent unfair draining of tasks).
        /// </summary>
        private long numberOfPendingTasksWhileRunning;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Sequencer" /> class.
        /// </summary>
        /// <param name="rootUnitOfExecution">The root Dispatcher.</param>
        public Sequencer(IExecutor rootUnitOfExecution)
        {
            this.rootUnitOfExecution = rootUnitOfExecution;
            this.numberOfPendingTasksWhileRunning = 0;
        }

        /// <summary>
        ///     Gets the unit of executions factory.
        /// </summary>
        public IExecutorFactory ExecutorFactory => this.rootUnitOfExecution.ExecutorFactory;

        /// <summary>
        ///     Gives a task/item to the sequencer in order to execute it in an asynchronous manner, but respecting the
        ///     order of the dispatch, and without concurrency among the sequencer s tasks.
        /// </summary>
        /// <param name="action">The item to be executed</param>
        public void Dispatch(Action action)
        {
            this.orderedDispatchedTasks.Enqueue(action);

            this.rootUnitOfExecution.Dispatch(this.Execute);
        }

        private void Execute()
        {
            if (Interlocked.Increment(ref this.numberOfPendingTasksWhileRunning) > 1)
            {   
                return;
            }

            while (true)
            {
                bool mustExit;
                if (!this.orderedDispatchedTasks.TryDequeue(out Action action))
                {
                    throw new System.Exception("Invalidstate");
                }

                try
                {
                    // Execute the next item
                    action();
                }
                finally
                {
                    mustExit = Interlocked.Decrement(ref this.numberOfPendingTasksWhileRunning) == 0;
                }

                if (mustExit)
                {
                    break;
                }
            }
        }
    }
}
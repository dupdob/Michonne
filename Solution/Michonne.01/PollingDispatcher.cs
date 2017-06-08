// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="PollingDispatcher.cs" company="No lock... no deadlock" product="Michonne">
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
namespace Michonne
{
    using System;
    using System.Collections.Generic;

    using Interfaces;

    /// <summary>
    /// Dispatcher that executes dispatched tasks/actions only on demand (i.e. when calling the
    /// ExecuteNext method).
    /// This dispatcher may be useful for unit testing purpose (to control your tasks execution pace)
    /// or to deal with some third-party constraints (e.g. when you need to execute your
    /// callbacks in their threads/message pumps).
    /// </summary>
    public sealed class PollingDispatcher : IExecutor
    {
        private readonly object syncRoot = new object();
        private readonly Queue<Action> dispatchedTasks = new Queue<Action>();

        /// <summary>
        ///     Gets the unit of executions factory.
        /// </summary>
        public IExecutorFactory ExecutorFactory { get; }

        /// <summary>
        /// Dispatch an action to be executed.
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <remarks>
        /// The action will be executed only by the given thread that will call the ExecuteNextTask method.
        /// </remarks>
        public void Dispatch(Action action)
        {
            lock (this.syncRoot)
            {
                this.dispatchedTasks.Enqueue(action);
            }
        }

        /// <summary>
        /// Executes the next dispatched task.
        /// </summary>
        public void ExecuteNextTask()
        {
            Action action = null;
            lock (this.syncRoot)
            {
                try
                {
                    action = this.dispatchedTasks.Dequeue();
                }
                catch (InvalidOperationException)
                {
                }
            }

            action?.Invoke();
        }
    }
}
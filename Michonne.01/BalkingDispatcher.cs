// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="BalkingDispatcher.cs" company="No lock... no deadlock" product="Michonne">
//     Copyright 2014 Cyrille  DUPUYDAUBY (@Cyrdup), Thomas PIERRAIN (@tpierrain)
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

    using Michonne.Interfaces;

    /// <summary>
    /// Dispatcher that keeps only the latest dispatched task, discarding the other dispatched tasks
    /// that couldn't have been executed.
    /// </summary>
    public sealed class BalkingDispatcher : IUnitOfExecution
    {
        private readonly object syncRoot = new object();
        private readonly IUnitOfExecution rootDispatcher;
        private Action lastTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="BalkingDispatcher"/> class.
        /// </summary>
        /// <param name="rootDispatcher">The root dispatcher.</param>
        public BalkingDispatcher(IUnitOfExecution rootDispatcher)
        {
            this.rootDispatcher = rootDispatcher;
        }

        /// <summary>
        /// Dispatch an action to be executed.
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <remarks>
        /// Depending on the concrete implementation of the dispatcher, the action will be
        /// executed asynchronously (most likely) or synchronously (a few exceptions).
        /// </remarks>
        public void Dispatch(Action action)
        {
            lock (this.syncRoot)
            {
                this.lastTask = action;
            }

            this.rootDispatcher.Dispatch(this.ExecuteLastTask);
        }

        private void ExecuteLastTask()
        {
            Action action = null;
            lock (this.syncRoot)
            {
                action = this.lastTask;
                this.lastTask = null;
            }

            if (action != null)
            {
                action();
            }
        }
    }
}
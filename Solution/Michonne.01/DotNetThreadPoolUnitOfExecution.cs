// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="DotNetThreadPoolUnitOfExecution.cs" company="No lock... no deadlock" product="Michonne">
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
    using System.Threading;

    using Michonne.Interfaces;

    /// <summary>
    /// Allow to dispatch actions/tasks for asynchronous execution through the classical .NET thread pool.
    /// </summary>
    public sealed class DotNetThreadPoolUnitOfExecution : IExecutor
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the unit of executions factory.
        /// </summary>
        public IExecutorFactory ExecutorFactory { get; }

        /// <summary>
        /// Dispatch an action to be executed.
        /// </summary>
        /// <remarks>
        ///     With this dispatcher, the action will be executed asynchronously.
        /// </remarks>
        /// <param name="action">The action to be executed asynchronously.</param>
        public void Dispatch(Action action)
        {
            ThreadPool.QueueUserWorkItem((_) => action());
        }

        #endregion
    }
}
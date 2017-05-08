#region File header

// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="SynchronousUnitOfExecution.cs" company="No lock... no deadlock" product="Michonne">
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
#endregion

namespace Michonne.Implementation
{
    using System;
    using Interfaces;

    /// <summary>
    /// This implementation of <see cref="IUnitOfExecution"/> implements synchronous calls.
    ///  This means that submitted <see cref="Action"/>s are immediately executed.
    ///  It does not offer scalability, as it relies on the calling thread.
    /// </summary>
    /// <remarks>Choose the <see cref="SynchronousUnitOfExecution"/> to favor latency against throughput.</remarks>
    internal class SynchronousUnitOfExecution : IUnitOfExecution
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronousUnitOfExecution"/> class.
        /// </summary>
        /// <param name="unitOfExecutionsFactory">
        /// The unit Of Executions Factory.
        /// </param>
        public SynchronousUnitOfExecution(IUnitOfExecutionsFactory unitOfExecutionsFactory)
        {
            this.UnitOfExecutionsFactory = unitOfExecutionsFactory;
        }

        /// <summary>
        ///     Gets the unit of executions factory.
        /// </summary>
        public IUnitOfExecutionsFactory UnitOfExecutionsFactory { get; }

        /// <summary>
        /// Dispatch an action to be executed.
        /// </summary>
        /// <remarks>
        /// Depending on the concrete implementation of the dispatcher, the action will be
        ///     executed asynchronously (most likely) or synchronously (a few exceptions).
        /// </remarks>
        /// <param name="action">
        /// The action to be executed
        /// </param>
        public void Dispatch(Action action)
        {
            action();
        }
    }
}
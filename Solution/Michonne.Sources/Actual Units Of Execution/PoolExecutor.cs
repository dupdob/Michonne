﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PoolExecutor.cs" company="No lock... no deadlock" product="Michonne">
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

// ReSharper disable once CheckNamespace
namespace Michonne.Implementation
{
    using System.Threading;
    using Interfaces;

#if !NET20 && !NET30
    using System;
#endif

    /// <summary>
    ///     The pool unit of execution executes submitted <see cref="Action" /> through the CLR Pool.
    /// </summary>
    internal class PoolExecutor : IExecutor
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PoolExecutor" /> class.
        /// </summary>
        /// <param name="executorFactory">
        ///     The unit of executions factory.
        /// </param>
        public PoolExecutor(IExecutorFactory executorFactory)
        {
            ExecutorFactory = executorFactory;
        }

        /// <summary>
        ///     Gets the unit of executions factory.
        /// </summary>
        public IExecutorFactory ExecutorFactory { get; }

        /// <summary>
        ///     Dispatch an action to be executed.
        /// </summary>
        /// <param name="action">
        ///     <see cref="Action" /> to be eventually executed.
        /// </param>
        public void Dispatch(Action action)
        {
            ThreadPool.QueueUserWorkItem(Execute, action);
        }

        /// <summary>
        ///     Wrapper method for actual execution.
        /// </summary>
        /// <param name="x">
        ///     Actual <see cref="Action" /> to be executed.
        /// </param>
        private static void Execute(object x)
        {
            ((Action)x)();
        }
    }
}
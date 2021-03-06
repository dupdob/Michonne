﻿#region File header

// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ThreadUnitOfExecution.cs" company="No lock... no deadlock" product="Michonne">
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
    using System.Collections.Generic;
    using System.Threading;
    using Interfaces;

    /// <summary>
    ///     This is a <see cref="IExecutor" /> implementation that executes submitted <see cref="Action" /> in a
    ///     dedicated thread.
    /// </summary>
    internal class ThreadExecutor : IDisposableExecutor
    {
        private readonly Thread myThread;
        private readonly object synchRoot = new object();
        private readonly Queue<Action> tasks = new Queue<Action>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadExecutor"/> class.
        /// </summary>
        /// <param name="unitOfExecutionsFactory">
        /// Factory used to build the instance.
        /// </param>
        public ThreadExecutor(IExecutorFactory unitOfExecutionsFactory)
        {
            this.ExecutorFactory = unitOfExecutionsFactory;
            this.myThread = new Thread(this.Process) {IsBackground = true };
            this.myThread.Start();
        }

        /// <summary>
        /// Gets the unit of executions factory.
        /// </summary>
        public IExecutorFactory ExecutorFactory { get; }

        /// <summary>
        /// The dispatch.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        public void Dispatch(Action action)
        {
            lock (this.synchRoot)
            {
                this.tasks.Enqueue(action);
                if (this.tasks.Count == 1)
                {
                    Monitor.Pulse(this.synchRoot);
                }
            }
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispatch(null);

            GC.SuppressFinalize(this);
            this.myThread.Join(500);
        }

        private void Process()
        {
            while (true)
            {
                Action next;
                lock (this.synchRoot)
                {
                    if (this.tasks.Count == 0)
                    {
                        Monitor.Wait(this.synchRoot);
                    }

                    next = this.tasks.Dequeue();
                }

                if (next == null)
                {
                    break;
                }

                next();
            }
        }

    }
}
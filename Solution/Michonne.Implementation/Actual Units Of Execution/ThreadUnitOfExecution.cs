#region File header

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

    using Michonne.Interfaces;

    /// <summary>
    ///     This is a <see cref="IUnitOfExecution" /> implementation that executes submitted <see cref="Action" /> in a
    ///     dedicated thread.
    /// </summary>
    internal class ThreadUnitOfExecution : IUnitOfExecution, IDisposable
    {
        #region Fields

        /// <summary>
        ///     Thread executing the work.
        /// </summary>
        private readonly Thread myThread;

        /// <summary>
        ///     private lock.
        /// </summary>
        private readonly object synchRoot = new object();

        /// <summary>
        ///     Tasks to be executed.
        /// </summary>
        private readonly Queue<Action> tasks = new Queue<Action>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadUnitOfExecution"/> class.
        /// </summary>
        /// <param name="unitOfExecutionsFactory">
        /// Factory used to build the instance.
        /// </param>
        public ThreadUnitOfExecution(IUnitOfExecutionsFactory unitOfExecutionsFactory)
        {
            this.UnitOfExecutionsFactory = unitOfExecutionsFactory;
            this.myThread = new Thread(this.Process);
            this.myThread.Start();
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="ThreadUnitOfExecution" /> class.
        ///     Destructor.
        /// </summary>
        ~ThreadUnitOfExecution()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unit of executions factory.
        /// </summary>
        public IUnitOfExecutionsFactory UnitOfExecutionsFactory { get; private set; }

        #endregion

        #region Public Methods and Operators

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
                if (this.tasks.Count == 0)
                {
                    Monitor.Pulse(this.synchRoot);
                }

                this.tasks.Enqueue(action);
            }
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        private void Dispose(bool disposing)
        {
            this.Dispatch(null);
            if (disposing)
            {
                GC.SuppressFinalize(this);
                this.myThread.Join(500);
            }
        }

        /// <summary>
        ///     The process.
        /// </summary>
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

        #endregion
    }
}
﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="UnitOfExecutionsFactory.cs" company="No lock... no deadlock" product="Michonne">
//     Copyright 2014 Cyrille DUPUYDAUBY (@Cyrdup)
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
    using System.Threading;
    using Interfaces;

    /// <summary>
    ///     This class can create all <see cref="IUnitOfExecution" />.
    /// </summary>
    /// <remarks>This class follows the classic 'Factory' pattern.</remarks>
    public class UnitOfExecutionsFactory : IUnitOfExecutionsFactory
    {
        #region Fields

        /// <summary>
        /// The core count.
        /// </summary>
        private readonly int coreCount;

        /// <summary>
        /// The number of created threads.
        /// </summary>
        private int createdThreadsCount;

        /// <summary>
        /// The pool unit of execution.
        /// </summary>
        private PoolUnitOfExecution poolUnitOfExecution;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfExecutionsFactory"/> class.
        /// </summary>
        public UnitOfExecutionsFactory()
        {
            this.coreCount = Environment.ProcessorCount;
            this.OverAllocationFactor = 2.0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the available core.
        /// </summary>
        public int AvailableCore => Math.Max((int)(this.coreCount * this.OverAllocationFactor) - this.createdThreadsCount, 0);

        /// <summary>
        /// Gets the number of core used by this factory.
        /// </summary>
        public int CoreCount => this.coreCount;

        /// <summary>
        /// Gets or sets the over allocation factor for cores.
        /// </summary>
        private double OverAllocationFactor { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Get an execution unit based on the CLR thread pool.
        /// </summary>
        /// <returns>An instance of <see cref="IUnitOfExecution" /> that executes <see cref="Action" /> on the CLR thread pool.</returns>
        public IUnitOfExecution GetDedicatedThread()
        {
            // we decrease the number of available core.
            this.UseAThread();
            return new ThreadUnitOfExecution(this);
        }

        /// <summary>
        /// Gets a synchronous execution unit.
        /// </summary>
        /// <returns>An instance of <see cref="IUnitOfExecution" /> that executes <see cref="Action" /> synchronously.</returns>
        public IUnitOfExecution GetSynchronousUnitOfExecution()
        {
            return new SynchronousUnitOfExecution(this);
        }
        
        /// <summary>
        /// Get an execution unit based on the CLR thread pool.
        /// </summary>
        /// <returns>An instance of <see cref="IUnitOfExecution" /> that executes <see cref="Action" /> on the CLR thread pool.</returns>
        public IUnitOfExecution GetPool()
        {
            // we use all remaining threads
            return this.poolUnitOfExecution ?? (this.poolUnitOfExecution = new PoolUnitOfExecution(this));
        }

        /// <summary>
        /// Build a <see cref="ISequencer"/> wrapping an <see cref="IUnitOfExecution"/>.
        /// </summary>
        /// <param name="execution"><see cref="IUnitOfExecution"/> that will carry out sequenced task.</param>
        /// <returns>A <see cref="ISequencer"/> instance.</returns>
        public ISequencer GetSequence(IUnitOfExecution execution)
        {
            return new Sequencer(execution);
        }

        /// <summary>
        /// Take into account the loss of one available core.
        /// </summary>
        private void UseAThread()
        {
            this.createdThreadsCount++;
#if !NETSTANDARD1_3
            ThreadPool.SetMaxThreads(this.AvailableCore+this.coreCount, this.coreCount*2);
#endif
        }

        #endregion
    }
}
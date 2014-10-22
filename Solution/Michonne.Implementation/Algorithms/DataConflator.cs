#region File header

// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="DataConflator.cs" company="No lock... no deadlock" product="Michonne">
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
    using System.Threading;

    using Michonne.Interfaces;

    /// <summary>
    /// This class implements a conflation algorithm for asynchronous data processing.
    /// </summary>
    /// <typeparam name="T">Type of processed data.
    /// </typeparam>
    internal class DataConflator<T> : IDataProcessor<T>
    {
        #region Fields

        /// <summary>
        /// The action.
        /// </summary>
        private readonly Action<T> action;

        /// <summary>
        /// The unit of execution.
        /// </summary>
        private readonly IUnitOfExecution unitOfExecution;

        /// <summary>
        /// The data.
        /// </summary>
        private object data;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataConflator{T}"/> class.
        /// </summary>
        /// <param name="unitOfExecution">
        /// The unit of execution.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public DataConflator(IUnitOfExecution unitOfExecution, Action<T> action)
        {
            this.unitOfExecution = unitOfExecution;
            this.action = action;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The post.
        /// </summary>
        /// <param name="newData">
        /// The new data.
        /// </param>
        public void Post(T newData)
        {
            if (Interlocked.Exchange(ref this.data, newData) == null)
            {
                // no data was pending processing, we push a task
                this.unitOfExecution.Dispatch(this.Execute);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The execute.
        /// </summary>
        private void Execute()
        {
            var nextData = Interlocked.Exchange(ref this.data, null);
            this.action((T)nextData);
        }

        #endregion
    }
}
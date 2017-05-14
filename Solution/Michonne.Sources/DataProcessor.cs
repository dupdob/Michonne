#region File header

// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="DataProcessor.cs" company="No lock... no deadlock" product="Michonne">
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
    /// The data processor.
    /// </summary>a
    /// <typeparam name="T">Type of data to be processed.
    /// </typeparam>
    public class DataProcessor<T> : IDataProcessor<T>
    {
        #region Fields

        /// <summary>
        /// The action.
        /// </summary>
        private readonly Action<T> action;

        /// <summary>
        /// The executor.
        /// </summary>
        private readonly IUnitOfExecution executor;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProcessor{T}"/> class.
        /// </summary>
        /// <param name="executor">
        /// The executor.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public DataProcessor(IUnitOfExecution executor, Action<T> action)
        {
            this.executor = executor;
            this.action = action;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The post.
        /// </summary>
        /// <param name="nextData">
        /// Next data to process.
        /// </param>
        public void Post(T nextData)
        {
            this.executor.Dispatch(() => this.action(nextData));
        }

        #endregion
    }
}
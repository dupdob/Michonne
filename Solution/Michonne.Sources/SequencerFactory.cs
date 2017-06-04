#region File header

// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="SequencerFactory.cs" company="No lock... no deadlock" product="Michonne">
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
    ///     Static class hosting the sequencer factory API.
    /// </summary>
    public static class SequencerFactory
    {
#region Public Methods and Operators

        /// <summary>
        /// Build a sequencer on top of an <see cref="ISequencer"/>.
        /// </summary>
        /// <param name="executor">
        /// The executor that will be used to provide sequential execution.
        /// </param>
        /// <returns>
        /// The <see cref="ISequencer"/>.
        /// </returns>
        public static ISequencer BuildSequencer(
            this 
            IUnitOfExecution executor)
        {
            return executor.UnitOfExecutionsFactory.GetSequence(executor);
        }

        /// <summary>
        /// Build a conflated version of an <see cref="Action"/> on top of an <see cref="ISequencer"/>.
        /// </summary>
        /// <typeparam name="T">Parameter type.</typeparam>
        /// <param name="executor">The execution unit that will be used to execute conflated <paramref name="action"/>.</param>
        /// <param name="action">Action to be executed in a conflated fashion.</param>
        /// <returns>A wrapped <see cref="Action"/> that provide conflated execution.</returns>
        public static Action<T> BuildConflator<T>(this IUnitOfExecution executor, Action<T> action)
        {
            var conflator = new DataConflator<T>(executor, action);
            return conflator.Post;
        }

        /// <summary>
        /// Creates <see cref="DataProcessor{T}"/> instance that will process incoming data asynchronously.
        /// </summary>
        /// <param name="executor">
        /// An <see cref="IUnitOfExecution"/> instance that will ultimately execute the task.
        /// </param>
        /// <param name="action">
        /// <see cref="Action{T}"/> instance that will process the data.
        /// </param>
        /// <param name="conflated">True if data can be conflated. </param>
        /// <typeparam name="T">
        /// Type of data to be processed.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Action"/>.
        /// </returns>
        public static IDataProcessor<T> BuildProcessor<T>(
            this
            IUnitOfExecution executor,
            Action<T> action,
            bool conflated)
        {
            if (conflated)
            {
                return new DataConflator<T>(executor, action);
            }
            return new DataProcessor<T>(executor, action);
        }

#endregion
    }
}
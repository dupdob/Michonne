#region File header

// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="IExecutorFactory.cs" company="No lock... no deadlock" product="Michonne">
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

namespace Michonne.Interfaces
{
#if !NET20 && !NET30
    using System;
#endif

    /// <summary>
    /// The ExecutorFactory interface.
    /// </summary>
    public interface IExecutorFactory
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Get an execution unit based on the CLR thread pool.
        /// </summary>
        /// <returns>An instance of <see cref="IExecutor" /> that executes <see cref="Action" /> on the CLR thread pool.</returns>
        IDisposableExecutor GetDedicatedThread();

        /// <summary>
        ///     Get an execution unit based on the CLR thread pool.
        /// </summary>
        /// <returns>An instance of <see cref="IExecutor" /> that executes <see cref="Action" /> on the CLR thread pool.</returns>
        IExecutor GetPool();

        /// <summary>
        /// Build a <see cref="ISequencer"/> wrapping an <see cref="IExecutor"/>.
        /// </summary>
        /// <param name="execution">
        /// <see cref="IExecutor"/> that will carry out sequenced task.
        /// </param>
        /// <returns>
        /// A <see cref="ISequencer"/> instance.
        /// </returns>
        ISequencer GetSequence(IExecutor execution);

#endregion
    }
}
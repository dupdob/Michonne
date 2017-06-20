// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="TestHelpers.cs" company="No lock... no deadlock" product="Michonne">
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
namespace Michonne.Implementation
{
    using System.Diagnostics.CodeAnalysis;

    using Interfaces;

    /// <summary>
    /// This static class hosts several methods to assist you in writing tests for classes using IExecutor/Michonne.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public static class TestHelpers
    {
        private static readonly ExecutorFactory Factory = new ExecutorFactory();

        /// <summary>
        /// Gets an instance of IUnitOfExecution wrapping the CLR thread pool.
        /// </summary>
        /// <returns>An instance of IUnitOfExecution wrapping the CLR thread pool.</returns>
        public static IExecutor GetPool()
        {
            return Factory.GetPool();
        }

        /// <summary>
        /// Gets an instance of IUnitOfExecution that executes tasks synchronously.
        /// </summary>
        /// <returns>An instance of IUnitOfExecution wrapping the CLR thread pool.</returns>
        public static IExecutor GetSynchronousExecutor()
        {
            return Factory.GetSynchronousUnitOfExecution();
        }

        /// <summary>
        /// Gets an instance of <see cref="IDisposableExecutor"/> to execute tasks on a dedicated thread.
        /// </summary>
        /// <returns>An instance of <see cref="IDisposableExecutor"/></returns>
        public static IDisposableExecutor GetThread()
        {
            return Factory.GetDedicatedThread();
        }
    }
}
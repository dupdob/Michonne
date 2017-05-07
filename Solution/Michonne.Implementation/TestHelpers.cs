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
    using Interfaces;

    /// <summary>
    /// This static class hosts several methods to assist you in writing tests for classes using UnitOfExecution/Michonne.
    /// </summary>
    public static class TestHelpers
    {
        private static readonly UnitOfExecutionsFactory Factory = new UnitOfExecutionsFactory();

        /// <summary>
        /// Gets an instance of IUnitOfExecution wrapping the CLR threadpool.
        /// </summary>
        /// <returns>An instance of IUnitOfExecution wrapping the CLR threadpool.</returns>
        public static IUnitOfExecution GetPool()
        {
            return Factory.GetPool();
        }

        /// <summary>
        /// Gets an instance of IUnitOfExecution that executes tasks syncrhonously.
        /// </summary>
        /// <returns>An instance of IUnitOfExecution wrapping the CLR threadpool.</returns>
        public static IUnitOfExecution GetSynchronousUnitOfExecution()
        {
            return Factory.GetSynchronousUnitOfExecution();
        }

        /// <summary>
        /// Gets an instance of <see cref="IDisposableUnitOfExecution"/> to execute tasks on a dedicated thread.
        /// </summary>
        /// <returns>An instance of <see cref="IDisposableUnitOfExecution"/></returns>
        public static IDisposableUnitOfExecution GetThread()
        {
            return Factory.GetDedicatedThread();
        }
    }
}
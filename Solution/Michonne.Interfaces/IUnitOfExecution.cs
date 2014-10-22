// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="IUnitOfExecution.cs" company="No lock... no deadlock" product="Michonne">
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
namespace Michonne.Interfaces
{
    using System;

    /// <summary>
    /// Allow to dispatch actions/tasks for execution. 
    /// A dispatcher may be whether asynchronous (more likely) or synchronous.
    /// </summary>
    public interface IUnitOfExecution
    {
        /// <summary>
        ///     Gets the unit of executions factory.
        /// </summary>
        IUnitOfExecutionsFactory UnitOfExecutionsFactory { get; }
        
        /// <summary>
        /// Dispatch an action to be executed.
        /// </summary>
        /// <remarks>
        ///     Depending on the concrete implementation of the dispatcher, the action will be 
        ///     executed asynchronously (most likely) or synchronously (a few exceptions).</remarks>
        /// <param name="action">The action to be executed</param>
        void Dispatch(Action action);
    }
}
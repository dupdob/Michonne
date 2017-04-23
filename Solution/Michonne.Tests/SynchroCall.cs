// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="SynchroCall.cs" company="No lock... no deadlock" product="Michonne">
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
namespace Michonne.Tests
{
    using System;

    using Interfaces;

    /// <summary>
    /// This implementation of <see cref="IUnitOfExecution"/> implements synchronous calls.
    ///  This means that submitted <see cref="Action"/>s are immediately executed.
    ///  It does not offer scalability, as it relies on the calling thread.
    /// </summary>
    /// <remarks>Choose the <see cref="SynchroCall"/> to favor latency against throughput.</remarks>
    public class SynchroCall : IUnitOfExecution
    {
        public int DoneTasks { get; private set; }

        #region IUnitOfExecution Members

        /// <summary>
        ///     Gets the unit of executions factory.
        /// </summary>
        public IUnitOfExecutionsFactory UnitOfExecutionsFactory { get; private set; }

        public void Dispatch(Action action)
        {
            action();
            this.DoneTasks++;
        }

        #endregion
    }
}
#region File header

// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ActionConflator.cs" company="No lock... no deadlock" product="Michonne">
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

    using Interfaces;

    /// <summary>
    /// This class implements a conflation logic on top of an <see cref="IUnitOfExecution"/>.
    /// </summary>
    public sealed class ActionConflator
    {
        #region Fields

        /// <summary>
        /// The unit of execution.
        /// </summary>
        private readonly IUnitOfExecution unitOfExecution;

        /// <summary>
        /// The nextAction.
        /// </summary>
        private Action action;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionConflator"/> class.
        /// </summary>
        /// <param name="unitOfExecution">
        /// The unit of execution.
        /// </param>
        public ActionConflator(IUnitOfExecution unitOfExecution)
        {
            this.unitOfExecution = unitOfExecution;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Dispatch and conflate an action.
        /// </summary>
        /// <param name="nextAction">
        /// The nextAction.
        /// </param>
        public void Conflate(Action nextAction)
        {
            if (Interlocked.Exchange(ref this.action, nextAction) == null)
            {
                // we need to dispatch
                this.unitOfExecution.Dispatch(this.Execute);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Internal execution logic.
        /// </summary>
        private void Execute()
        {
            // performs an atomtic capture of the task and executes it.
            Interlocked.Exchange(ref this.action, null)();
        }

        #endregion
    }
}
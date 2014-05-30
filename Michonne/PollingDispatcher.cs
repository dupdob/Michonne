// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="PollingDispatcher.cs" company="">
// //   Copyright 2014 Thomas PIERRAIN
// //   Licensed under the Apache License, Version 2.0 (the "License");
// //   you may not use this file except in compliance with the License.
// //   You may obtain a copy of the License at
// //       http://www.apache.org/licenses/LICENSE-2.0
// //   Unless required by applicable law or agreed to in writing, software
// //   distributed under the License is distributed on an "AS IS" BASIS,
// //   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// //   See the License for the specific language governing permissions and
// //   limitations under the License.
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace Michonne
{
    using System;
    using System.Collections.Generic;

    using Michonne.Interfaces;

    /// <summary>
    /// Dispatcher that executes tasks/actions only on demand (i.e. when calling the ExecuteNext method). 
    /// This may be useful with third-party constraints or for unit test purpose (to control the tasks execution).
    /// </summary>
    public sealed class PollingDispatcher : IDispatcher
    {
        private readonly object syncRoot = new object();
        private readonly Queue<Action> dispatchedTasks = new Queue<Action>();
 
        public void Dispatch(Action action)
        {
            lock (this.syncRoot)
            {
                this.dispatchedTasks.Enqueue(action);
            }
        }

        public void ExecuteNextTask()
        {
            Action action = null;
            lock (this.syncRoot)
            {
                try
                {
                    action = this.dispatchedTasks.Dequeue();
                }
                catch (InvalidOperationException)
                {
                }
            }
            
            if (action != null)
            {
                action();
            }
        }
    }
}
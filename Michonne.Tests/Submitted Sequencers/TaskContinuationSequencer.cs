// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskContinuationSequencer.cs" company="No lock... no deadlock">
//   Copyright 2014 Cyrille  DUPUYDAUBY (@Cyrdup), Thomas PIERRAIN (@tpierrain)
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//        http://www.apache.org/licenses/LICENSE-2.0
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
namespace Seq
{
    using System;
    using System.Threading.Tasks;

    using Michonne.Interfaces;
    using Michonne.Tests;

    public class TaskContinuationSequencer : ISequencer
    {
        private readonly object _lock = new object();
        private readonly TaskScheduler _scheduler;
        
        private Task _task = Task.FromResult(0);

        public TaskContinuationSequencer(IUnitOfExecution executor)
        {
            this._scheduler = new TaskSchedulerAdapter(executor);
        }

        public void Dispatch(Action action)
        {
            lock (this._lock)
            {
                this._task = this._task.ContinueWith(_ => action(), this._scheduler);
            }
        }
    }
}
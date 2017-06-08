// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="TaskSchedulerAdapter.cs" company="No lock... no deadlock" product="Michonne">
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
namespace Michonne.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Michonne.Interfaces;

    public class TaskSchedulerAdapter : TaskScheduler
    {
        private readonly IExecutor executor;

        // The list of tasks to be executed  
        private readonly LinkedList<Task> tasks = new LinkedList<Task>(); // protected by lock(_tasks) 
        
        public TaskSchedulerAdapter(IExecutor executor)
        {
            this.executor = executor;
        }

        // Queues a task to the scheduler.  
        protected sealed override void QueueTask(Task task)
        {
            // Add the task to the list of tasks to be processed.  If there aren't enough  
            // delegates currently queued or running to process tasks, schedule another.  
            lock (this.tasks)
            {
                this.tasks.AddLast(task);
            }

            this.executor.Dispatch(() =>
            {
                Task next;
                lock (this.tasks)
                {
                    next = this.tasks.First.Value;
                    this.tasks.RemoveFirst();
                }

                base.TryExecuteTask(task);
            });
        }

        // Attempts to execute the specified task on the current thread.  
        protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // If the task was previously queued, remove it from the queue 
            if (taskWasPreviouslyQueued)
            {
                // Try to run the task.  
                if (this.TryDequeue(task))
                {
                    return base.TryExecuteTask(task);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return base.TryExecuteTask(task);
            }
        }

        // Attempt to remove a previously scheduled task from the scheduler.  
        protected sealed override bool TryDequeue(Task task)
        {
            lock (this.tasks)
            {
                return this.tasks.Remove(task);
            }
        }

        // Gets an enumerable of the tasks currently scheduled on this scheduler.  
        protected sealed override IEnumerable<Task> GetScheduledTasks()
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(this.tasks, ref lockTaken);
                if (lockTaken)
                {
                    return this.tasks;
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(this.tasks);
                }
            }
        }
    }
}
// Provides a task scheduler that ensures a maximum concurrency level while  
// running on top of the thread pool. 

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Michonne.Interfaces;

namespace Michonne.Tests
{
    public class TaskSchedulerAdapter : TaskScheduler
    {
        private readonly IUnitOfExecution _executor;
        // Indicates whether the current thread is processing work items.
        [ThreadStatic]
        private static bool _currentThreadIsProcessingItems;

        // The list of tasks to be executed  
        private readonly LinkedList<Task> _tasks = new LinkedList<Task>(); // protected by lock(_tasks) 

        public TaskSchedulerAdapter(IUnitOfExecution executor)
        {
            this._executor = executor;
        }

        // Queues a task to the scheduler.  
        protected sealed override void QueueTask(Task task)
        {
            // Add the task to the list of tasks to be processed.  If there aren't enough  
            // delegates currently queued or running to process tasks, schedule another.  
            lock (this._tasks)
            {
                this._tasks.AddLast(task);
            }
            _executor.Dispatch(() =>
            {
                Task next;
                lock (this._tasks)
                {
                    next = this._tasks.First.Value;
                    this._tasks.RemoveFirst();
                }
                base.TryExecuteTask(task);
            });
        }

        // Attempts to execute the specified task on the current thread.  
        protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // If this thread isn't already processing a task, we don't support inlining 
            if (!_currentThreadIsProcessingItems) return false;

            // If the task was previously queued, remove it from the queue 
            if (taskWasPreviouslyQueued)
                // Try to run the task.  
                if (this.TryDequeue(task))
                    return base.TryExecuteTask(task);
                else
                    return false;
            else
                return base.TryExecuteTask(task);
        }

        // Attempt to remove a previously scheduled task from the scheduler.  
        protected sealed override bool TryDequeue(Task task)
        {
            lock (this._tasks) return this._tasks.Remove(task);
        }

        // Gets an enumerable of the tasks currently scheduled on this scheduler.  
        protected sealed override IEnumerable<Task> GetScheduledTasks()
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(this._tasks, ref lockTaken);
                if (lockTaken) return this._tasks;
                else throw new NotSupportedException();
            }
            finally
            {
                if (lockTaken) Monitor.Exit(this._tasks);
            }
        }
    }
}
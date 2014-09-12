using System;
using System.Threading.Tasks;
using Michonne.Interfaces;
using Michonne.Tests;

namespace Seq
{
    public class TaskContinuationSequencer: ISequencer
    {
        private readonly object _lock = new object();
        private Task _task = Task.FromResult(0);
        private readonly TaskScheduler _scheduler;


        public TaskContinuationSequencer(IUnitOfExecution executor)
        {
            _scheduler = new TaskSchedulerAdapter(executor);
        }
        public void Dispatch(Action action)
        {
            lock (_lock)
            {
                _task = _task.ContinueWith(_ => action(), _scheduler);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using Michonne.Interfaces;

namespace Michonne.Tests
{
    internal class ThreadUnitOfExecution : IUnitOfExecution, IDisposable
    {
        private readonly Thread _myThread;
        private readonly Queue<Action> _tasks = new Queue<Action>(); 
        private readonly object lck = new object();

        public ThreadUnitOfExecution()
        {
            this._myThread = new Thread(this.Process);
            this._myThread.Start();
        }

        private void Process()
        {
            while (true)
            {
                Action next;
                lock (this.lck)
                {
                    if (this._tasks.Count == 0)
                        Monitor.Wait(this.lck);

                    next = this._tasks.Dequeue();
                }
                if (next == null)
                {
                    break;
                }
                next();
            }
        }

        public void Dispatch(Action action)
        {
            lock (this.lck)
            {
                if (this._tasks.Count == 0)
                {
                    Monitor.Pulse(this.lck);   
                }
                this._tasks.Enqueue(action);               
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            this.Dispatch(null);
            if (disposing)
            {
                this._myThread.Join(500);
            }
        }
    }
}
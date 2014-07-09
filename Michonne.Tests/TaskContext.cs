using System.Threading;

namespace Michonne.Tests
{
    internal class TaskContext
    {
        private int _ranTasks;
        private int _targetTaskCount;
        private readonly object _lck = new object();
        private bool _raceCondition;

        public bool 
            RaceCondition
        {
            get { return this._raceCondition; }
        }

        public void Delay(int timer)
        {
            if (Monitor.TryEnter(this._lck))
            {
                try
                {
                    Thread.Sleep(timer);
                }
                finally
                {
                    Monitor.Exit(this._lck);
                }
            }
            else
            {
                this._raceCondition = true;
            }
            Interlocked.Increment(ref this._ranTasks);
            if (Monitor.TryEnter(this._lck))
            {
                try
                {
                    if (this._targetTaskCount == this._ranTasks)
                    {
                        Monitor.PulseAll(this._lck);
                    }
                }
                finally
                {
                    Monitor.Exit(this._lck);
                }
            }
        }

        public bool WaitForTasks(int nbTasks)
        {
            lock (this._lck)
            {
                this._targetTaskCount = nbTasks;
                while (this._ranTasks < nbTasks)
                {
                    Monitor.Wait(this._lck, 100);
                }
                return this._raceCondition;
            }
        }
    }
}
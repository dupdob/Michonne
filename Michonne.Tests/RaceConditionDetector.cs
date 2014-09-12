
namespace Michonne.Tests
{
    using System.Threading;

    internal class RaceConditionDetector
    {
        private int _ranTasks;
        private int _targetTaskCount;
        private readonly object _taskExecutionLock = new object();
        private readonly object _taskCountLock = new object();
        private bool raceConditionDetected;

        public bool RaceConditionDetected
        {
            get { return this.raceConditionDetected; }
        }

        public void Delay(int timer)
        {
            if (Monitor.TryEnter(this._taskExecutionLock))
            {
                try
                {
                    Thread.Sleep(timer);
                }
                finally
                {
                    Monitor.Exit(this._taskExecutionLock);
                }
            }
            else
            {
                this.raceConditionDetected = true;
            }
            Interlocked.Increment(ref this._ranTasks);
            if (Monitor.TryEnter(this._taskCountLock))
            {
                try
                {
                    if (this._targetTaskCount == this._ranTasks)
                    {
                        Monitor.PulseAll(this._taskCountLock);
                    }
                }
                finally
                {
                    Monitor.Exit(this._taskCountLock);
                }
            }
        }

        public bool WaitForTasks(int nbTasks)
        {
            lock (this._taskCountLock)
            {
                this._targetTaskCount = nbTasks;
                while (this._ranTasks < nbTasks)
                {
                    Monitor.Wait(this._taskCountLock, 100);
                }
                return this.raceConditionDetected;
            }
        }
    }
}
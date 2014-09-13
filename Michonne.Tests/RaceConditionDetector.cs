// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RaceConditionDetector.cs" company="No lock... no deadlock">
//    Copyright 2014 Cyrille  DUPUYDAUBY (@Cyrdup), Thomas PIERRAIN (@tpierrain)
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

namespace Michonne.Tests
{
    using System.Threading;

    internal class RaceConditionDetector
    {
        private readonly object _taskExecutionLock = new object();
        private readonly object _taskCountLock = new object();
        private int _ranTasks;
        private int _targetTaskCount;
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

        public bool WaitForTasks(int tasksNumber)
        {
            lock (this._taskCountLock)
            {
                this._targetTaskCount = tasksNumber;
                while (this._ranTasks < tasksNumber)
                {
                    Monitor.Wait(this._taskCountLock, 100);
                }

                return this.raceConditionDetected;
            }
        }
    }
}
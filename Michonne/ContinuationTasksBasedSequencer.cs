// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="ContinuationTasksBasedSequencer.cs" company="">
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
    using System.Threading;
    using System.Threading.Tasks;

    using Michonne.Interfaces;

    /// <summary>
    /// Sequencer implementation provided by Olivier COANET on Cyrille's blog.
    /// (has to test more and to review the TPL continuation task implementation to check whether 
    /// or not this implementation fulfill all the Sequencer requirements:
    ///     http://dupdob.wordpress.com/2014/05/09/the-sequencer-part-2/
    ///     and 
    ///     http://dupdob.wordpress.com/2014/05/14/sequencer-part-2-1/
    /// </summary>
    public class ContinuationTasksBasedSequencer : ISequencer
    {
        private readonly object syncRoot = new object();
        private readonly TaskScheduler taskScheduler;
        private Task task = Task.FromResult(0);
        private int pendingTaskCount;

        public ContinuationTasksBasedSequencer() : this(TaskScheduler.Current)
        {
        }

        public ContinuationTasksBasedSequencer(TaskScheduler taskScheduler)
        {
            // we could need to specify a custom scheduler, in order to limit concurrency, or for testing purpose
            this.taskScheduler = taskScheduler;
        }

        public event Action<Exception> Error;

        // so the pending task count can be controlled or monitored from the outside
        public int PendingTaskCount
        {
            get { return this.pendingTaskCount; }
        }

        public void Dispatch(Action action)
        {
            // it might be a good idea ensure pendingTaskCount is above a max value
            // when it is beyond the max we could block, discard updates, throw or do anything that seems appropriate
            var continuationAction = this.BuildContinuationAction(action);

            lock (this.syncRoot)
            {
                this.task = this.task.ContinueWith(continuationAction, this.taskScheduler);
            }
        }

        private Action<Task> BuildContinuationAction(Action action)
        {
            Interlocked.Increment(ref this.pendingTaskCount);

            return previousTask =>
            {
                // tasks are not supposed to be disposed (http://blogs.msdn.com/b/pfxteam/archive/2012/03/25/10287435.aspx)
                // but I feel more confortable doing it, because tasks are finalizable
                previousTask.Dispose();

                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    var error = this.Error;
                    if (error != null)
                    {
                        error(ex);
                    }
                }

                Interlocked.Decrement(ref this.pendingTaskCount);
            };
        }
    }
}
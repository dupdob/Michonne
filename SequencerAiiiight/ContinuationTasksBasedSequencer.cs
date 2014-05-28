// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="ContinuationTasksBasedSequencer.cs" company="">
// //   Copyright 2014 Olivier COANET
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
namespace SequencerAiiiight
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Sequencer implementation provided by Olivier COANET on Cyrille's blog.
    /// (has to test more and to review the TPL continuation task implementation to check whether 
    /// or not this implementation fulfill all the Sequencer requirements:
    ///     http://dupdob.wordpress.com/2014/05/09/the-sequencer-part-2/
    ///     and 
    ///     http://dupdob.wordpress.com/2014/05/14/sequencer-part-2-1/
    /// </summary>
    public class ContinuationTasksBasedSequencer
    {
        private readonly object syncRoot = new object();
        private Task task = Task.FromResult(0);

        /// <summary>
        /// Gives a task/action to the sequencer in order to execute it in an asynchronous manner, but respecting the
        /// order of the dispatch, and without concurrency among the sequencer's tasks.
        /// </summary>
        /// <param name="action"></param>
        public void Dispatch(Action action)
        {
            lock (this.syncRoot)
            {
                this.task = this.task.ContinueWith(_ => action());
            }
        }
    }
}
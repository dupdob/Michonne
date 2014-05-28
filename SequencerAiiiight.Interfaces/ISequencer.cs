// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="ISequencer.cs" company="">
// //   Copyright 2014 Cyrille DUPUYDAUBY, Thomas PIERRAIN
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
namespace SequencerAiiiight.Interfaces
{
    /// <summary>
    /// Allows to execute tasks asynchronously, but one by one and in the same order as they have been dispatched.
    /// That means that two tasks from the same dispatcher can be executed by two different threads, but NEVER in parallel. 
    /// Sequencer requirements are presented here: 
    ///     http://dupdob.wordpress.com/2014/05/09/the-sequencer-part-2/
    ///     and
    ///     http://dupdob.wordpress.com/2014/05/14/sequencer-part-2-1/
    /// </summary>
    public interface ISequencer : IDispatcher
    {
    }
}
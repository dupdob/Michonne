// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="DirectDispatcher.cs" company="">
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
    using Michonne.Interfaces;

    public class DirectDispatcher : IDispatcher
    {
        /// <summary>
        /// Directly execute every dispatched action in a synchronous manner (in the thread of 
        /// the caller of the Dispatch method).
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <remarks>
        /// The action will be executed synchronously, by the thread calling the Dispatch method.
        /// </remarks>
        public void Dispatch(Action action)
        {
            action();
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="IStapleMarketData.cs" company="No lock... no deadlock" product="Michonne">
//     Copyright 2014 Cyrille DUPUYDAUBY (@Cyrdup), Thomas PIERRAIN (@tpierrain)
//     Licensed under the Apache License, Version 2.0 (the "License");
//     you may not use this file except in compliance with the License.
//     You may obtain a copy of the License at
//         http://www.apache.org/licenses/LICENSE-2.0
//     Unless required by applicable law or agreed to in writing, software
//     distributed under the License is distributed on an "AS IS" BASIS,
//     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//     See the License for the specific language governing permissions and
//     limitations under the License.
//   </copyright>
//   --------------------------------------------------------------------------------------------------------------------
namespace PastaPricer
{
    using System;

    /// <summary>
    /// Provides market data as events for a given staple.
    /// </summary>
    public interface IStapleMarketData
    {
        /// <summary>
        /// Occurs when the price of this staple changed.
        /// </summary>
        event EventHandler<StaplePriceChangedEventArgs> StaplePriceChanged;

        /// <summary>
        /// Gets the name of the Staple corresponding to this <see cref="StapleMarketData"/> instance.
        /// </summary>
        /// <value>
        /// The name of the Staple corresponding to this <see cref="StapleMarketData"/> instance.
        /// </value>
        string StapleName { get; }

        /// <summary>
        /// Starts to receive market data (and thus to raise events) for this staple.
        /// </summary>
        void Start();
    }
}
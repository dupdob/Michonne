// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="StaplePriceChangedEventArgs.cs" company="No lock... no deadlock" product="Michonne">
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
    /// Event arguments when the price of a staple changed.
    /// </summary>
    public class StaplePriceChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaplePriceChangedEventArgs"/> class.
        /// </summary>
        /// <param name="stapleName">StapleName of the market data.</param>
        /// <param name="price">The price.</param>
        public StaplePriceChangedEventArgs(string stapleName, decimal price)
        {
            this.StapleName = stapleName;
            this.Price = price;
        }

        /// <summary>
        /// Gets the name of the considered staple.
        /// </summary>
        /// <value>
        /// The name of the considered staple.
        /// </value>
        public string StapleName { get; private set; }

        /// <summary>
        /// Gets the new price for the considered staple.
        /// </summary>
        /// <value>
        /// The new price for the considered staple.
        /// </value>
        public decimal Price { get; private set; }
    }
}
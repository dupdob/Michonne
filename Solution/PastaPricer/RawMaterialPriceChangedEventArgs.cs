// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="RawMaterialPriceChangedEventArgs.cs" company="No lock... no deadlock" product="Michonne">
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
    /// Event arguments when the price of a raw material changed.
    /// </summary>
    public class RawMaterialPriceChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RawMaterialPriceChangedEventArgs"/> class.
        /// </summary>
        /// <param name="rawMaterialName">The name of the raw material that has changed its price.</param>
        /// <param name="price">The new price of this raw material.</param>
        public RawMaterialPriceChangedEventArgs(string rawMaterialName, decimal price)
        {
            this.RawMaterialName = rawMaterialName;
            this.Price = price;
        }

        /// <summary>
        /// Gets the name of the considered raw material.
        /// </summary>
        /// <value>
        /// The name of the considered raw material.
        /// </value>
        public string RawMaterialName { get; private set; }

        /// <summary>
        /// Gets the new price for the considered raw material.
        /// </summary>
        /// <value>
        /// The new price for the considered raw material.
        /// </value>
        public decimal Price { get; private set; }
    }
}
// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="PastaPriceChangedEventArgs.cs" company="No lock... no deadlock" product="Michonne">
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
    /// Event arguments when the price of a pasta changed.
    /// </summary>
    public class PastaPriceChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PastaPriceChangedEventArgs"/> class.
        /// </summary>
        /// <param name="pastaName">Name of the pasta.</param>
        /// <param name="price">The price.</param>
        public PastaPriceChangedEventArgs(string pastaName, decimal price)
        {
            this.PastaName = pastaName;
            this.Price = price;
        }

        /// <summary>
        /// Gets the name of the considered pasta.
        /// </summary>
        /// <value>
        /// The name of the considered pasta.
        /// </value>
        public string PastaName { get; private set; }

        /// <summary>
        /// Gets the new price for the considered pasta.
        /// </summary>
        /// <value>
        /// The new price for the considered pasta.
        /// </value>
        public decimal Price { get; private set; }
    }
}
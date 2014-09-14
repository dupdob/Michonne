// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="PriceChangedEventArgs.cs" company="No lock... no deadlock" product="Michonne">
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

    public class PriceChangedEventArgs : EventArgs
    {
        public string MarketDataName { get; private set; }
        public decimal Price { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriceChangedEventArgs"/> class.
        /// </summary>
        /// <param name="marketDataName">Name of the market data.</param>
        /// <param name="price">The price.</param>
        public PriceChangedEventArgs(string marketDataName, decimal price)
        {
            this.MarketDataName = marketDataName;
            this.Price = price;
        }
    }
}
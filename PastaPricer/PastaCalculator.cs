// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="PastaCalculator.cs" company="No lock... no deadlock" product="Michonne">
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
    /// Calculator for pasta prices.
    /// </summary>
    public class PastaCalculator
    {
        // TODO: make it static with functions only
        public decimal Compute(decimal flourPrice, decimal eggsPrice, decimal flavorPrice)
        {
            return Math.Round(0.5m + flourPrice + ((1 / 4m) * eggsPrice) + ((1 / 10m) * flavorPrice), 2);
        }
    }
}
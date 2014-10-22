// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="IMarketDataProvider.cs" company="No lock... no deadlock" product="Michonne">
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
    /// <summary>
    /// Provides <see cref="RawMaterialMarketData"/> instances for registered raw material names.
    /// </summary>
    public interface IMarketDataProvider
    {
        /// <summary>
        /// Starts all the registered <see cref="RawMaterialMarketData"/> instances.
        /// </summary>
        void Start();

        /// <summary>
        /// Gets the <see cref="RawMaterialMarketData"/> instance corresponding to this raw material name.
        /// </summary>
        /// <param name="rawMaterialName">RawMaterialName of the raw material.</param>
        /// <returns>The <see cref="RawMaterialMarketData"/> instance corresponding to this raw material name.</returns>
        IRawMaterialMarketData GetRawMaterial(string rawMaterialName);

        /// <summary>
        /// Registers the specified raw material, so that it can be started and retrieved afterwards.
        /// </summary>
        /// <param name="rawMaterialNameToRegister">The raw material name to register.</param>
        void RegisterRawMaterial(string rawMaterialNameToRegister);

        /// <summary>
        /// Stops all the registered <see cref="RawMaterialMarketData"/> instances.
        /// </summary>
        void Stop();
    }
}
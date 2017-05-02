// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="RawMaterialRole.cs" company="No lock... no deadlock" product="Michonne">
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
    /// Raw materials role in the recipe.
    /// </summary>
    public enum RawMaterialRole
    {
        /// <summary>
        /// Pasta shape.
        /// </summary>
        Shape,

        /// <summary>
        /// Main ingredient.
        /// </summary>
        Flour,

        /// <summary>
        /// Some eggs (optional)
        /// </summary>
        Egg,

        /// <summary>
        /// Used to give specific taste/color
        /// </summary>
        Flavor,

        /// <summary>
        /// Packaging type
        /// </summary>
        Packaging,

        /// <summary>
        /// Size of the packaging
        /// </summary>
        Size
    }
}
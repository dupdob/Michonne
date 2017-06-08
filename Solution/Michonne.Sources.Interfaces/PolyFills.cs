// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PolyFills.cs" company="No lock... no deadlock" product="Michonne">
//   Copyright 2014 Cyrille DUPUYDAUBY (@Cyrdup)
//   //     Licensed under the Apache License, Version 2.0 (the "License");
//   //     you may not use this file except in compliance with the License.
//   //     You may obtain a copy of the License at
//   //         http://www.apache.org/licenses/LICENSE-2.0
//   //     Unless required by applicable law or agreed to in writing, software
//   //     distributed under the License is distributed on an "AS IS" BASIS,
//   //     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   //     See the License for the specific language governing permissions and
//   //     limitations under the License.
// </copyright>
// <summary>
//   Replacement required to tackle compatibility with various net versions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#if NET20 || NET30
namespace Michonne.Interfaces
{
    /// <summary>
    ///     Encapsulates a method that has no parameters and does not return a value.
    /// </summary>
    public delegate void Action();
}

namespace System.Runtime.CompilerServices
{
    /// <summary>
    ///     Indicates that a method is an extension method, or that a class or assembly contains extension methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute
    {
    }
}
#endif

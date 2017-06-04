// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PolyFills.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Action type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace System.Runtime.CompilerServices
{
#if NET20 || NET30
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class |
        AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute { }
#endif
}

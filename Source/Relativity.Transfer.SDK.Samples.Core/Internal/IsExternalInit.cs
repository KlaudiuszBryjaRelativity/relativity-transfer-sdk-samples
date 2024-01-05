// ReSharper disable CheckNamespace

using System.ComponentModel;

namespace System.Runtime.CompilerServices;
// ReSharper restore CheckNamespace

/// <summary>
///     C# 9 added support for the init and record keywords. When using C# 9 with target frameworks older than .NET 5.0,
///     using these new features is not possible because the compiler is missing the IsExternalInit class,
///     hence making the features unavailable for any target framework prior to .NET 5.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
internal static class IsExternalInit
{
}
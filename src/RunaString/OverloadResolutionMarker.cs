using System.Diagnostics;

namespace RunaString;

/// <summary>
/// A marker type to resolve generic type constraint overload.
/// Any instance of this type may never created.
/// </summary>
public abstract class OverloadResolutionMarker
{
    [DebuggerHidden]
    private OverloadResolutionMarker() { }

    /// <summary>
    /// A marker to indicate that the generic type parameter is a class type.
    /// </summary>
    public sealed class Class : OverloadResolutionMarker
    {
        [DebuggerHidden]
        private Class() { }
    }

    /// <summary>
    /// A marker to indicate that the generic type parameter is a struct type.
    /// </summary>
    public sealed class Struct : OverloadResolutionMarker
    {
        [DebuggerHidden]
        private Struct() { }
    };

    /// <summary>
    /// A marker to indicate that the generic type parameter is assignable from the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class AssignableFrom<T> : OverloadResolutionMarker
    {
        [DebuggerHidden]
        private AssignableFrom() { }
    }

    /// <summary>
    /// A marker to indicate that the generic type parameter is assignable to the specified type.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public sealed class Intersect<T1, T2> : OverloadResolutionMarker
        where T1 : OverloadResolutionMarker
        where T2 : OverloadResolutionMarker
    {
        [DebuggerHidden]
        private Intersect() { }
    }
}

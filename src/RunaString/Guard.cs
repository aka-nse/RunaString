using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace RunaString;

internal static class Guard
{
    [DebuggerHidden, DoesNotReturn]
    public static void ThrowArgumentNull(string? paramName = null) => throw new ArgumentNullException(paramName);

    [DebuggerHidden, DoesNotReturn]
    public static T ThrowArgumentNull<T>(string? paramName = null) => throw new ArgumentNullException(paramName);

    [DebuggerHidden, DoesNotReturn]
    public static void ThrowArgumentOutOfRange(string? paramName = null, string? message = null) => throw new ArgumentOutOfRangeException(paramName, message);

    [DebuggerHidden, DoesNotReturn]
    public static T ThrowArgumentOutOfRange<T>(string? paramName = null, string? message = null) => throw new ArgumentOutOfRangeException(paramName, message);

    [DebuggerHidden, DoesNotReturn]
    public static void ThrowArgument(string? paramName = null, string? message = null) => throw new ArgumentException(message, paramName);

    [DebuggerHidden, DoesNotReturn]
    public static T ThrowArgument<T>(string? paramName = null, string? message = null) => throw new ArgumentException(message, paramName);

    [DebuggerHidden, DoesNotReturn]
    public static void ThrowInvalidOperation(string? message = null) => throw new InvalidOperationException(message);

    [DebuggerHidden, DoesNotReturn]
    public static T ThrowInvalidOperation<T>(string? message = null) => throw new InvalidOperationException(message);


    [DebuggerHidden]
    public static T NotNull<T>(T? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : notnull
    {
        if (value is null)
        {
            ThrowArgumentNull(paramName);
        }
        return value;
    }

    [DebuggerHidden]
    public static T WithIn<T>(T value, T min, T max, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparisonOperators<T, T, bool>
    {
        if (value < min || value > max)
        {
            ThrowArgumentOutOfRange(paramName, $"Value must be between {min} and {max}.");
        }
        return value;
    }

    [DebuggerHidden]
    public static int WithIn(int index, int length, [CallerArgumentExpression(nameof(index))] string? paramName = null)
    {
        if ((uint)index >= (uint)length)
        {
            ThrowArgumentOutOfRange(paramName, $"Index must be between 0 and {length - 1}.");
        }
        return index;
    }

    [DebuggerHidden]
    public static void WithIn([DoesNotReturnIf(false)] bool shouldBe, string? paramName = null, [CallerArgumentExpression(nameof(shouldBe))] string? shouldBeExpression = null)
    {
        if (!shouldBe)
        {
            ThrowArgumentOutOfRange(null, $"{(paramName ?? "Value")} must be {shouldBeExpression}");
        }
    }

    [DebuggerHidden]
    public static void ValidArgument([DoesNotReturnIf(false)] bool shouldBe, string? message = null)
    {
        if (!shouldBe)
        {
            ThrowArgument(null, message);
        }
    }

    [DebuggerHidden]
    public static void ValidState([DoesNotReturnIf(false)] bool shouldBe, [CallerArgumentExpression(nameof(shouldBe))] string? message = null)
    {
        if (!shouldBe)
        {
            ThrowInvalidOperation(message);
        }
    }
}

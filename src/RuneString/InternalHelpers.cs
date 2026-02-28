using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;

namespace RuneString;

internal static class InternalHelpers
{
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0060 // Remove unused parameter
    [Conditional("DEBUG")]
    public static void NoUse<T>(T value) where T : allows ref struct { }
#pragma warning restore IDE0060
#pragma warning restore IDE0079

    public static bool AreSame<T>(ReadOnlySpan<T> x, ReadOnlySpan<T> y)
    {
        if(!Unsafe.AreSame(ref Unsafe.AsRef(in x[0]), ref Unsafe.AsRef(in y[0])))
        {
            return false;
        }
        return x.Length == y.Length;
    }

    public static bool AreSame<T>(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y) =>
        AreSame(x.Span, y.Span);

    public static void ValidateUtf8(ReadOnlySpan<byte> utf8Buffer)
    {
        if (!Utf8.IsValid(utf8Buffer))
        {
            throw new ArgumentException(
                "The provided buffer is not a valid UTF-8 sequence.",
                nameof(utf8Buffer));
        }
    }

    public static int GetRuneCount<TEnumerator>(TEnumerator enumerator)
        where TEnumerator : IRuneEnumerator<TEnumerator>, allows ref struct
    {
        int count = 0;
        while (enumerator.MoveNext())
        {
            count++;
        }
        return count;
    }

    public static string ToString(ReadOnlySpan<byte> utf8Buffer) =>
        Encoding.UTF8.GetString(utf8Buffer);

    public static int GetHashCode(ReadOnlySpan<byte> utf8Buffer)
    {
        var hash = new HashCode();
        foreach (var x in MemoryMarshal.Cast<byte, int>(utf8Buffer))
        {
            hash.Add(x);
        }
        return hash.ToHashCode();
    }

    public static int Compare(Utf8Span x, Utf8Span y)
    {
        var enumeratorX = x.GetEnumerator();
        var enumeratorY = y.GetEnumerator();
        while (true)
        {
            var hasX = enumeratorX.MoveNext();
            var hasY = enumeratorY.MoveNext();
            if (!hasX && !hasY) return 0;
            if (!hasX) return -1;
            if (!hasY) return 1;
            var runeX = enumeratorX.Current;
            var runeY = enumeratorY.Current;
            var comparison = runeX.CompareTo(runeY);
            if (comparison != 0)
            {
                return comparison;
            }
        }
    }

    public static bool Equals(Utf8Span x, Utf8Span y)
    {
        if(x.BufferLength != y.BufferLength)
        {
            return false;
        }
        var vx = MemoryMarshal.Cast<byte, Vector<byte>>(x.Buffer);
        var vy = MemoryMarshal.Cast<byte, Vector<byte>>(y.Buffer);
        for (int i = 0; i < vx.Length; i++)
        {
            if (vx[i] != vy[i])
            {
                return false;
            }
        }
        var ux = MemoryMarshal.Cast<byte, uint>(x.Buffer.Slice(vx.Length * Vector<byte>.Count));
        var uy = MemoryMarshal.Cast<byte, uint>(y.Buffer.Slice(vy.Length * Vector<byte>.Count));
        for (int i = 0; i < ux.Length; i++)
        {
            if (ux[i] != uy[i])
            {
                return false;
            }
        }
        var xx = x.Buffer.Slice((vx.Length * Vector<byte>.Count) + (ux.Length * sizeof(uint)));
        var yy = y.Buffer.Slice((vy.Length * Vector<byte>.Count) + (uy.Length * sizeof(uint)));
        for (int i = 0; i < xx.Length; i++)
        {
            if (xx[i] != yy[i])
            {
                return false;
            }
        }
        return true;
    }
}
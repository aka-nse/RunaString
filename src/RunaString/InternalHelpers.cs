using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace RunaString;

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

    public static int Compare<TString, TEnumerator>(TString x, TString y)
        where TString : IRunaEnumerable<TString, TEnumerator>, allows ref struct
        where TEnumerator : IRunaEnumerator<TEnumerator>, allows ref struct
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

    public static bool Equals<T>(ReadOnlySpan<T> x, ReadOnlySpan<T> y)
        where T : unmanaged
    {
        if (x.Length != y.Length)
        {
            return false;
        }

        var vx = MemoryMarshal.Cast<T, Vector<uint>>(x);
        var vy = MemoryMarshal.Cast<T, Vector<uint>>(y);
        for (int i = 0; i < vx.Length; i++)
        {
            if (vx[i] != vy[i])
            {
                return false;
            }
        }
        var ux = MemoryMarshal.Cast<T, uint>(x.Slice(vx.Length * Vector<uint>.Count));
        var uy = MemoryMarshal.Cast<T, uint>(y.Slice(vy.Length * Vector<uint>.Count));
        for (int i = 0; i < ux.Length; i++)
        {
            if (ux[i] != uy[i])
            {
                return false;
            }
        }
        var xx = MemoryMarshal.Cast<T, byte>(x.Slice((vx.Length * Vector<uint>.Count) + ux.Length * sizeof(uint) / Unsafe.SizeOf<T>()));
        var yy = MemoryMarshal.Cast<T, byte>(y.Slice((vy.Length * Vector<uint>.Count) + uy.Length * sizeof(uint) / Unsafe.SizeOf<T>()));
        for (int i = 0; i < xx.Length; i++)
        {
            if (xx[i] != yy[i])
            {
                return false;
            }
        }
        return true;
    }

    public static int GetRuneCount<TEnumerator>(TEnumerator enumerator)
        where TEnumerator : IRunaEnumerator<TEnumerator>, allows ref struct
    {
        int count = 0;
        while (enumerator.MoveNext())
        {
            count++;
        }
        return count;
    }


    public static (TIndex start, TIndex end) GetSliceIndex<TString, TEnumerator, TIndex>(TString str, int runeStart, int runeLength)
        where TString : IRunaString<TString, TEnumerator, TIndex>, allows ref struct
        where TEnumerator : IRunaEnumerator<TEnumerator, TIndex>, allows ref struct
        where TIndex : struct, IRunaIndex
    {
        var enumerator = str.GetEnumerator();
        for (var i = 0; i < runeStart; i++)
        {
            if (!enumerator.MoveNext())
            {
                throw new ArgumentOutOfRangeException(nameof(runeStart));
            }
        }
        var startIndex = enumerator.Index;
        for (var i = 0; i < runeLength; i++)
        {
            if (!enumerator.MoveNext())
            {
                throw new ArgumentOutOfRangeException(nameof(runeLength));
            }
        }
        var endIndex = enumerator.Index;
        return (startIndex, endIndex);
    }
}
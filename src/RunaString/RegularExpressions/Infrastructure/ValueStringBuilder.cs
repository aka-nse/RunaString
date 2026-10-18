using System.Buffers;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

#pragma warning disable IDE0130
namespace RunaString.RegularExpressions;
#pragma warning restore IDE0130
internal ref struct ValueStringBuilder
{
    private char[]? _arrayToReturnToPool;
    private Span<char> _chars;
    private int _pos;

    public int Length
    {
        readonly get => _pos;
        set
        {
            Debug.Assert(value >= 0);
            Debug.Assert(value <= _chars.Length);
            _pos = value;
        }
    }

    public ref char this[int index] => ref _chars[index];

    public Span<char> RawChars => _chars;

    public ValueStringBuilder(Span<char> initialBuffer)
    {
        _arrayToReturnToPool = null;
        _chars = initialBuffer;
        _pos = 0;
    }

    public ValueStringBuilder(int initialCapacity)
    {
        _arrayToReturnToPool = ArrayPool<char>.Shared.Rent(initialCapacity);
        _chars = _arrayToReturnToPool;
        _pos = 0;
    }

    public override string ToString()
    {
        string s = _chars[.._pos].ToString();
        Dispose();
        return s;
    }

    public void Dispose()
    {
        var array = _arrayToReturnToPool;
        this = default;
        if (array is not null)
        {
            ArrayPool<char>.Shared.Return(array);
        }
    }

    public void EnsureCapacity(int capacity)
    {
        if (capacity <= _chars.Length)
        {
            return;
        }
        capacity = (int)BitOperations.RoundUpToPowerOf2((uint)capacity);
        var array = ArrayPool<char>.Shared.Rent(capacity);
        _chars[0.._pos].CopyTo(array);
        (_arrayToReturnToPool, array) = (array, _arrayToReturnToPool);
        _chars = array;
        if (array is not null)
        {
            ArrayPool<char>.Shared.Return(array);
        }
    }

    public void NullTerminate()
    {
        EnsureCapacity(_pos + 1);
        _chars[_pos] = '\0';
    }

    public ref char GetPinnableReference() =>
        ref MemoryMarshal.GetReference(_chars);

    public ReadOnlySpan<char> AsSpan() => _chars[.._pos];

    public ReadOnlySpan<char> AsSpan(int start) => _chars[start.._pos];

    public ReadOnlySpan<char> AsSpan(int start, int length) => _chars.Slice(start, length);

    public void Insert(int index, char value, int count)
    {
        Debug.Assert(index >= 0);
        Debug.Assert(count >= 0);
        Debug.Assert(index + count <= _pos);

        EnsureCapacity(_pos + count);
        var dst = _chars[(index + count)..];
        var src = _chars[index.._pos];
        src.CopyTo(dst);
        _chars.Slice(index, count).Fill(value);
        _pos += count;
    }

    public void Insert(int index, string? value)
    {
        Debug.Assert(index >= 0);
        Debug.Assert(index <= _pos);
        if(value is null)
        {
            return;
        }

        int count = value.Length;
        EnsureCapacity(_pos + count);
        var dst = _chars[(index + count)..];
        var src = _chars[index.._pos];
        src.CopyTo(dst);
        value.AsSpan().CopyTo(_chars.Slice(index, count));
        _pos += count;
    }

    public void Append(char c)
    {
        int pos = _pos;
        EnsureCapacity(pos + 1);
        _chars[pos] = c;
        _pos = pos + 1;
    }

    public void Append(char c, int count)
    {
        Debug.Assert(count >= 0);
        int pos = _pos;
        EnsureCapacity(pos + count);
        _chars.Slice(pos, count).Fill(c);
        _pos = pos + count;
    }

    public void Append(string? s)
    {
        if (s is null)
        {
            return;
        }
        Append(s.AsSpan());
    }

    public void Append(scoped ReadOnlySpan<char> value)
    {
        var pos = _pos;
        EnsureCapacity(pos + value.Length);
        value.CopyTo(_chars.Slice(pos, value.Length));
        _pos = pos + value.Length;
    }

    public Span<char> AppendSpan(int length)
    {
        var origPos = _pos;
        EnsureCapacity(origPos + length);
        _pos += length;
        return _chars.Slice(origPos, length);
    }
}

using System.Buffers;
using System.Diagnostics;
using System.Numerics;

#pragma warning disable IDE0130
namespace RunaString.RegularExpressions;
#pragma warning restore IDE0130

internal ref struct ValueListBuilder<T>
{
    private T[]? _arrayToReturnToPool;
    private Span<T> _span;
    private int _pos;

    public int Length
    {
        readonly get => _pos;
        set
        {
            Debug.Assert(value >= 0);
            Debug.Assert(value <= _span.Length);
            _pos = value;
        }
    }

    public ref T this[int index] => ref _span[index];

    public Span<T> RawSpan => _span;

    public ValueListBuilder(Span<T> initialBuffer)
    {
        _arrayToReturnToPool = null;
        _span = initialBuffer;
        _pos = 0;
    }

    public ValueListBuilder(int initialCapacity)
    {
        _arrayToReturnToPool = ArrayPool<T>.Shared.Rent(initialCapacity);
        _span = _arrayToReturnToPool;
        _pos = 0;
    }

    public override string ToString()
    {
        string s = _span[.._pos].ToString();
        Dispose();
        return s;
    }

    public void Dispose()
    {
        var array = _arrayToReturnToPool;
        this = default;
        if (array is not null)
        {
            ArrayPool<T>.Shared.Return(array);
        }
    }

    public void EnsureCapacity(int capacity)
    {
        if (capacity <= _span.Length)
        {
            return;
        }
        capacity = (int)BitOperations.RoundUpToPowerOf2((uint)capacity);
        var array = ArrayPool<T>.Shared.Rent(capacity);
        _span[0.._pos].CopyTo(array);
        (_arrayToReturnToPool, array) = (array, _arrayToReturnToPool);
        _span = array;
        if (array is not null)
        {
            ArrayPool<T>.Shared.Return(array);
        }
    }

    public ReadOnlySpan<T> AsSpan() => _span[.._pos];

    public ReadOnlySpan<T> AsSpan(int start) => _span[start.._pos];

    public ReadOnlySpan<T> AsSpan(int start, int length) => _span.Slice(start, length);

    public void Insert(int index, T value, int count)
    {
        Debug.Assert(index >= 0);
        Debug.Assert(index + count <= _pos);

        EnsureCapacity(_pos + count);
        var dst = _span[(index + count)..];
        var src = _span[index.._pos];
        src.CopyTo(dst);
        _span.Slice(index, count).Fill(value);
        _pos += count;
    }

    public void Insert(int index, scoped ReadOnlySpan<T> value)
    {
        Debug.Assert(index >= 0);
        Debug.Assert(index <= _pos);
        if (value.IsEmpty)
        {
            return;
        }

        int count = value.Length;
        EnsureCapacity(_pos + count);
        var dst = _span[(index + count)..];
        var src = _span[index.._pos];
        src.CopyTo(dst);
        value.CopyTo(_span.Slice(index, count));
        _pos += count;
    }

    public void Append(T c)
    {
        int pos = _pos;
        EnsureCapacity(pos + 1);
        _span[pos] = c;
        _pos = pos + 1;
    }

    public void Append(T c, int count)
    {
        Debug.Assert(count >= 0);
        int pos = _pos;
        EnsureCapacity(pos + count);
        _span.Slice(pos, count).Fill(c);
        _pos = pos + count;
    }

    public void Append(scoped ReadOnlySpan<T> value)
    {
        var pos = _pos;
        EnsureCapacity(pos + value.Length);
        value.CopyTo(_span.Slice(pos, value.Length));
        _pos = pos + value.Length;
    }

    public Span<T> AppendSpan(int length)
    {
        var origPos = _pos;
        EnsureCapacity(origPos + length);
        _pos += length;
        return _span.Slice(origPos, length);
    }

    public bool TryCopyTo(Span<T> destination, out int itemsWritten)
    {
        if (_span.Slice(0, _pos).TryCopyTo(destination))
        {
            itemsWritten = _pos;
            return true;
        }

        itemsWritten = 0;
        return false;
    }

    public T Pop()
    {
        _pos--;
        return _span[_pos];
    }
}

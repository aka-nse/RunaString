using System.Text;

namespace RunaString;

/// <summary>
/// Defines a type that represents a seekable index into a source buffer for an
/// <see cref="IRuneEnumerable{TSelf, TEnumerator, TIndex}"/> and <see cref="IRuneEnumerator{TSelf, TIndex, TBuffer}"/>.
/// </summary>
public interface ISeekIndex
{
    /// <summary>
    /// Gets the zero-based index of the current rune in the source buffer.
    /// </summary>
    public int RuneIndex { get; }
}


/// <summary>
/// Defines a contract for enumerating Unicode runes with support for creating slices over a specified range of the source data.
/// </summary>
/// <typeparam name="TSelf">
/// The type that implements this interface, enabling fluent method chaining and type safety for operations that return a new enumerator.
/// </typeparam>
/// <typeparam name="TEnumerator">
/// The type of the enumerator that iterates over the runes in the source buffer, which must implement the IRuneEnumerable interface to support slicing and range operations.
/// </typeparam>
/// <typeparam name="TIndex">
/// The type used to index into the source data, which must implement the ISeekIndex interface to support seeking and range operations.
/// </typeparam>
public interface IRuneEnumerable<TSelf, TEnumerator, TIndex>
    where TSelf : IRuneEnumerable<TSelf, TEnumerator, TIndex>, allows ref struct
    where TEnumerator : IRuneEnumerator<TEnumerator>, allows ref struct
    where TIndex : ISeekIndex
{
    /// <summary>
    /// Gets the rune located at the specified index in the collection.
    /// </summary>
    /// <returns>The rune at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException" />
    public Rune this[TIndex index] { get; }

    /// <summary>
    /// Creates a new enumerator that iterates over the runes in the specified range of the source.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">
    /// <para>Cannot slice between enumerators with different source buffers;</para>
    /// <para>- or -</para>
    /// <para>Start enumerator must be at an earlier position than end enumerator.</para>
    /// </exception>
    public TSelf Slice(TIndex start, TIndex end);

    /// <summary>
    /// Returns an enumerator that iterates through the runes in the source buffer.
    /// </summary>
    /// <returns></returns>
    public TEnumerator GetEnumerator();

    /// <inheritdoc />
    public string ToString();
}


/// <summary>
/// Defines a value type that iterates over the runes in a source buffer and exposes the current rune and its position.
/// </summary>
/// <typeparam name="TSelf"></typeparam>
/// <remarks>
/// This type is designed to be implemented by structs.
/// </remarks>
public interface IRuneEnumerator<TSelf>
    where TSelf : IRuneEnumerator<TSelf>, allows ref struct
{
    /// <summary>
    /// Gets an empty enumerator that has no source buffer and is positioned at the end of the buffer.
    /// </summary>
    public static abstract TSelf Empty { get; }

    /// <summary>
    /// Gets the current rune in the source buffer.
    /// </summary>
    public Rune Current { get; }

    /// <summary>
    /// Advances the enumerator to the next rune in the source buffer.
    /// </summary>
    /// <returns></returns>
    public bool MoveNext();
}


/// <summary>
/// Defines a value type that iterates over the runes in a source buffer and exposes the current rune and its position.
/// </summary>
/// <typeparam name="TSelf"></typeparam>
/// <typeparam name="TIndex"></typeparam>
/// <typeparam name="TBuffer"></typeparam>
/// <remarks>
/// This type is designed to be implemented by structs.
/// </remarks>
public interface IRuneEnumerator<TSelf, TIndex, TBuffer> : IRuneEnumerator<TSelf>
    where TSelf : IRuneEnumerator<TSelf, TIndex, TBuffer>, allows ref struct
    where TIndex : ISeekIndex
    where TBuffer : allows ref struct
{
    /// <summary>
    /// Gets the seek index that represents the current position of the enumerator in the source buffer.
    /// </summary>
    public TIndex SeekIndex { get; }

    /// <summary>
    /// Returns a new enumerator that is positioned at the specified seek index in the source buffer.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public TSelf Seek(TIndex index);

    /// <summary>
    /// Gets the source buffer that the enumerator is iterating over.
    /// </summary>
    public TBuffer SourceBuffer { get; }

    /// <summary>
    /// Gets the portion of the source buffer that has already been consumed by the enumerator.
    /// </summary>
    public TBuffer ConsumedBuffer { get; }

    /// <summary>
    /// Gets the portion of the source buffer that has not yet been consumed by the enumerator.
    /// </summary>
    public TBuffer RemainingBuffer { get; }
}

internal static class RuneEnumerator
{
    public static class ErrorMessages
    {
        public const string UnmatchSourceBuffer
            = "Cannot slice between enumerators with different source buffers.";
        public const string StartAfterEnd
            = "Start enumerator must be at an earlier position than end enumerator.";
    }

}
using System.Text;

namespace RunaString;

/// <inheritdoc cref="IRunaEnumerator{TSelf, TableIndex, TBuffer}" />
public interface IRunaEnumerator<TSelf>
    where TSelf : IRunaEnumerator<TSelf>, allows ref struct
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


/// <inheritdoc cref="IRunaEnumerator{TSelf, TableIndex, TBuffer}" />
public interface IRuneEnumerator<TSelf, TIndex> : IRunaEnumerator<TSelf>
    where TSelf : IRuneEnumerator<TSelf, TIndex>, allows ref struct
    where TIndex : ISeekIndex
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
public interface IRunaEnumerator<TSelf, TIndex, TBuffer> : IRuneEnumerator<TSelf, TIndex>
    where TSelf : IRunaEnumerator<TSelf, TIndex, TBuffer>, allows ref struct
    where TIndex : ISeekIndex
    where TBuffer : allows ref struct
{
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


internal static class RunaEnumerator
{
    public static class ErrorMessages
    {
        public const string UnmatchSourceBuffer
            = "Cannot slice between enumerators with different source buffers.";
        public const string StartAfterEnd
            = "Start enumerator must be at an earlier position than end enumerator.";
    }
}
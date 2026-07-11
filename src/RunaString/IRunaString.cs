using System.Text;

namespace RunaString;

/// <summary>
/// Defines a contract for enumerating Unicode runes.
/// </summary>
/// <typeparam name="TSelf">
/// The type that implements this interface, enabling fluent method chaining and type safety for operations that return a new enumerator.
/// </typeparam>
/// <typeparam name="TEnumerator">
/// The type of the enumerator that iterates over the runes in the source buffer, which must implement the IRuneEnumerable interface to support slicing and range operations.
/// </typeparam>
public interface IRunaEnumerable<TSelf, TEnumerator>
    where TSelf : allows ref struct
    where TEnumerator : IRunaEnumerator<TEnumerator>, allows ref struct
{
    /// <summary>
    /// Returns an enumerator that iterates through the runes in the source buffer.
    /// </summary>
    /// <returns></returns>
    public TEnumerator GetEnumerator();
}


/// <inheritdoc cref="IRunaString{TSelf, TEnumerator, TIndex}" />
public interface IRunaString<TSelf, TIndex>
    where TSelf : IRunaString<TSelf, TIndex>, allows ref struct
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
    /// Attempts to get the rune located at the specified index in the collection.
    /// </summary>
    /// <param name="index">The index of the rune to retrieve.</param>
    /// <param name="rune">When this method returns, contains the rune at the specified index, if the index is valid; otherwise, the default value.</param>
    /// <returns>True if the rune was successfully retrieved; otherwise, false.</returns>
    /// <remarks>
    /// This method only validates code unit indices, not rune indices.
    /// A valid code unit index must be within the bounds of the source buffer and must not point to the middle of a multi-code-unit sequence that forms a single rune.
    /// </remarks>
    public bool TryGetRune(TIndex index, out Rune rune);

    /// <summary>
    /// Attempts to get the rune located at the specified index in the collection.
    /// </summary>
    /// <param name="index">The index of the rune to retrieve. This must be an index instance created from this string instance.</param>
    /// <param name="rune">When this method returns, contains the rune at the specified index, if the index is valid; otherwise, the default value.</param>
    /// <param name="codeUnitConsumed">When this method returns, contains the number of code units consumed to decode the rune, if the index is valid; otherwise, zero.</param>
    /// <returns>True if the rune was successfully retrieved; otherwise, false.</returns>
    /// <remarks>
    /// This method only validates code unit indices, not rune indices.
    /// This method behave undefined if <c>rune</c> is not related from this string instance.
    /// </remarks>
    public bool TryGetRune(TIndex index, out Rune rune, out int codeUnitConsumed);

    /// <summary>
    /// Attempts to increment the specified index to the next position in the source buffer.
    /// </summary>
    /// <param name="index">The index to increment. This must be an index instance created from this string instance.</param>
    /// <returns>True if the index was successfully incremented; otherwise, false.</returns>
    /// <remarks>
    /// This method only validates code unit indices, not rune indices.
    /// This method behave undefined if <c>rune</c> is not related from this string instance.
    /// Refer to <seealso cref="TryGetRune(TIndex, out Rune)"/>.
    /// </remarks>
    public bool TryIncrement(ref TIndex index);

    /// <summary>
    /// Attempts to decrement the specified index to the previous position in the source buffer.
    /// </summary>
    /// <param name="index">The index to decrement. This must be an index instance created from this string instance.</param>
    /// <returns>True if the index was successfully decremented; otherwise, false.</returns>
    /// <remarks>
    /// This method only validates code unit indices, not rune indices.
    /// This method behave undefined if <c>rune</c> is not related from this string instance.
    /// Refer to <seealso cref="TryGetRune(TIndex, out Rune)"/>.
    /// </remarks>
    public bool TryDecrement(ref TIndex index);

    /// <inheritdoc />
    public string ToString();
}


/// <summary>
/// Defines a contract for Unicode runes string with support for creating slices over a specified range of the source data.
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
public interface IRunaString<TSelf, TEnumerator, TIndex> : IRunaString<TSelf, TIndex>, IRunaEnumerable<TSelf, TEnumerator>
    where TSelf : IRunaString<TSelf, TEnumerator, TIndex>, allows ref struct
    where TEnumerator : IRunaEnumerator<TEnumerator>, allows ref struct
    where TIndex : ISeekIndex
{
}

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


/// <inheritdoc cref="IRunaString{TSelf, TView, TEnumerator, TIndex}" />
public interface IRunaStringBase<TSelf, TIndex>
    : IEquatable<TSelf>, IComparable<TSelf>
    where TSelf : IRunaStringBase<TSelf, TIndex>, allows ref struct
    where TIndex : unmanaged, IRunaIndex<TIndex>
{
    /// <summary>
    /// Creates an instance of the implementing type from a sequence of Unicode scalar values.
    /// </summary>
    /// <param name="runes">The span of <see cref="Rune"/> values to convert.</param>
    /// <returns>A new instance representing the specified runes.</returns>
    public static abstract TSelf FromRunes(scoped ReadOnlySpan<Rune> runes);

    /// <summary>
    /// Creates an instance of the current type from a character span.
    /// </summary>
    /// <param name="str">A character span that contains the string representation of the value.</param>
    /// <returns>An instance of the current type created from the specified character span.</returns>
    public static abstract TSelf FromString(scoped ReadOnlySpan<char> str);

    /// <summary>
    /// Gets the rune located at the specified index in the collection.
    /// </summary>
    /// <returns>The rune at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException" />
    public Rune this[TIndex index] { get; }

    /// <inheritdoc />
    public string ToString();

    /// <summary>
    /// Returns a new <typeparamref name="TSelf" /> that is a slice of the current string, starting at the specified rune index and with the specified rune length.
    /// </summary>
    /// <param name="runeStart"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public TSelf Slice(int runeStart);

    /// <summary>
    /// Returns a new <typeparamref name="TSelf" /> that is a slice of the current string, starting at the specified rune index and with the specified rune length.
    /// </summary>
    /// <param name="runeStart"></param>
    /// <param name="runeLength"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public TSelf Slice(int runeStart, int runeLength);

    /// <summary>
    /// Creates a new enumerator that iterates over the runes in the specified range of the source.
    /// </summary>
    /// <param name="start"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">
    /// <para>Cannot slice between enumerators with different source buffers;</para>
    /// <para>- or -</para>
    /// <para>Start enumerator must be at an earlier position than end enumerator.</para>
    /// </exception>
    public TSelf Slice(TIndex start);

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
    /// 
    /// <para>
    ///     This method only validates code unit indices, not rune indices.
    ///     A valid code unit index must be within the bounds of the source buffer
    ///     and must not point to the middle of a multi-code-unit sequence that forms a single rune.
    /// </para>
    /// <para>
    ///     This method returns true if the read position advances, even if an invalid sequence is encountered.
    /// </para>
    /// </summary>
    /// <param name="index">
    ///     The index of the rune to retrieve.
    /// </param>
    /// <param name="rune">
    ///     When this method returns, contains the rune at the specified index,
    ///     if the index is valid; otherwise, the default value.
    /// </param>
    /// <returns>
    /// True if the rune was successfully retrieved;
    /// otherwise false if the index reaches the end.
    /// The result is undefined if invalid UTF-8 sequence or invalid index against scalar value boundary.
    /// </returns>
    public bool TryGetRune(TIndex index, out Rune rune);

    /// <summary>
    /// Attempts to get the rune located at the specified index in the collection.
    /// 
    /// <para>
    ///     This method only validates code unit indices, not rune indices.
    ///     This method behave undefined if <c>rune</c> is not related from this string instance.
    /// </para>
    /// <para>
    ///     This method returns true if the read position advances, even if an invalid sequence is encountered.
    /// </para>
    /// </summary>
    /// <param name="index">The index of the rune to retrieve. This must be an index instance created from this string instance.</param>
    /// <param name="rune">When this method returns, contains the rune at the specified index, if the index is valid; otherwise, the default value.</param>
    /// <param name="codeUnitConsumed">When this method returns, contains the number of code units consumed to decode the rune, if the index is valid; otherwise, zero.</param>
    /// <returns>
    /// True if the rune was successfully retrieved;
    /// otherwise false if the index reaches the end.
    /// The result is undefined if invalid UTF-8 sequence or invalid index against scalar value boundary.
    /// </returns>
    public bool TryGetRune(TIndex index, out Rune rune, out int codeUnitConsumed);

    /// <summary>
    /// Attempts to increment the specified index to the next position in the source buffer.
    /// </summary>
    /// <param name="index">The index to increment. This must be an index instance created from this string instance.</param>
    /// <returns>The incremented index.</returns>
    /// <remarks>
    /// This method only validates code unit indices, not rune indices.
    /// This method behave undefined if <c>rune</c> is not related from this string instance.
    /// Refer to <seealso cref="TryGetRune(TIndex, out Rune)"/>.
    /// </remarks>
    public TIndex Increment(ref TIndex index);

    /// <summary>
    /// <para>Gets a <typeparamref name="TSelf"/> instance that represents the initial value of incremental iteration.</para>
    /// <para>(like <c>0</c> for <c>System.String</c>)</para>
    /// <para>The result is same as <see cref="IRunaIndex{TIndex}.IncrementBegin"/>. </para>
    /// </summary>
    public TIndex IncrementBegin { get; }

    /// <summary>
    /// <para>Gets a <typeparamref name="TSelf"/> instance that represents the stop value of incremental iteration.</para>
    /// <para>(like <c>System.String.Length</c>)</para>
    /// </summary>
    /// <returns></returns>
    public TIndex GetIncrementEnd();

    /// <summary>
    /// Attempts to decrement the specified index to the previous position in the source buffer.
    /// </summary>
    /// <param name="index">The index to decrement. This must be an index instance created from this string instance.</param>
    /// <returns>The decremented index.</returns>
    /// <remarks>
    /// This method only validates code unit indices, not rune indices.
    /// This method behave undefined if <c>rune</c> is not related from this string instance.
    /// Refer to <seealso cref="TryGetRune(TIndex, out Rune)"/>.
    /// </remarks>
    public TIndex Decrement(ref TIndex index);

    /// <summary>
    /// <para>Gets a <typeparamref name="TSelf"/> instance that represents the stop value of decremental iteration.</para>
    /// <para>(like <c>-1</c> for <c>System.String</c>)</para>
    /// <para>The result is same as <see cref="IRunaIndex{TIndex}.DecrementEnd"/>. </para>
    /// </summary>
    public TIndex DecrementEnd { get; }

    /// <summary>
    /// <para>Gets a <typeparamref name="TSelf"/> instance that represents the initial value of decremental iteration.</para>
    /// <para>(like <c>System.String.Length - 1</c>)</para>
    /// </summary>
    /// <returns></returns>
    public TIndex GetDecrementBegin();

    /// <summary>
    /// Gets the index value that corresponds to the specified rune index.
    /// </summary>
    /// <param name="runeIndex">The zero-based index of the rune to locate.</param>
    /// <returns>A value of type <c>TIndex</c> representing the index at the specified rune position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the specified rune index is out of range.</exception>
    public TIndex GetIndexByRuneIndex(int runeIndex);

    /// <summary>
    /// Determines whether the specified index is within the valid range of the source buffer.
    /// </summary>
    /// <param name="index">The index to check.</param>
    /// <returns>True if the index is within the valid range of this instance; otherwise, false.</returns>
    public bool IsInRange(TIndex index);

    /// <summary>
    /// Returns the number of Unicode code points (runes) in the UTF-8 encoded string.
    /// </summary>
    /// <returns></returns>
    public int GetRuneCount();

    /// <summary>
    /// Determines whether two <see cref="Utf8SpanString" />  instances are equal by comparing their UTF-8 encoded byte sequences for equality.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static abstract bool Equals(scoped TSelf x, scoped TSelf y);

    /// <summary>
    /// Compares two <typeparamref name="TSelf" />  instances by comparing in lexicographical order.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static abstract int Compare(scoped TSelf x, scoped TSelf y);

    /// <summary>
    /// Determines whether two <typeparamref name="TSelf" />  instances are equal in lexicographical order.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static abstract bool operator ==(scoped TSelf x, scoped TSelf y);

    /// <summary>
    /// Determines whether two <typeparamref name="TSelf" />  instances are not equal in lexicographical order.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static abstract bool operator !=(scoped TSelf x, scoped TSelf y);
}


/// <summary>
/// Represents a RunaString value that can be exposed as a view type and queried for ordinal prefix and suffix matches.
/// </summary>
/// <remarks>Uses a self-referential generic pattern to preserve strong typing across operations while exposing a
/// non-owning view representation.</remarks>
/// <typeparam name="TSelf">The concrete self type that implements the interface.</typeparam>
/// <typeparam name="TView">The view type used to represent and compare the current value.</typeparam>
/// <typeparam name="TIndex">The index type used by the string representation.</typeparam>
public interface IRunaStringViewable<TSelf, TView, TIndex>
    : IRunaStringBase<TSelf, TIndex>
    where TSelf : IRunaStringViewable<TSelf, TView, TIndex>, allows ref struct
    where TView : IRunaStringBase<TView, TIndex>, allows ref struct
    where TIndex : unmanaged, IRunaIndex<TIndex>
{
    /// <summary>Gets the current value as <typeparamref name="TView"/>.</summary>
    /// <returns>The current value represented as <typeparamref name="TView"/>.</returns>
    public TView AsView();

    /// <summary>
    /// Finds the first occurrence of a pattern in the current view using code-unit indexing.
    /// </summary>
    /// <remarks>Indexing is based on code units, not Unicode scalar values or text elements.</remarks>
    /// <param name="pattern">The sequence of code units to locate.</param>
    /// <returns>The zero-based code-unit index of the first match, or -1 when no match is found.</returns>
    public int CodeUnitIndexOf(scoped TView pattern);

    /// <summary>
    /// Determines whether the current value starts with the specified pattern using ordinal comparison.
    /// </summary>
    /// <param name="pattern">The pattern to compare with the beginning of the current value.</param>
    /// <returns><see langword="true"/> if the current value starts with <paramref name="pattern"/>; otherwise, <see
    /// langword="false"/>.</returns>
    public bool StartsWith(scoped TView pattern);

    /// <summary>
    /// Determines whether the current value ends with the specified pattern using ordinal comparison.
    /// </summary>
    /// <param name="pattern">The pattern to compare with the end of the current value.</param>
    /// <returns><see langword="true"/> if the current value ends with <paramref name="pattern"/>; otherwise, <see
    /// langword="false"/>.</returns>
    public bool EndsWith(scoped TView pattern);

    /// <inheritdoc cref="IRunaStringBase{TSelf, TIndex}.Equals(TSelf, TSelf)" />
    public static abstract bool Equals(scoped TSelf x, scoped TView y);

    /// <inheritdoc cref="IRunaStringBase{TSelf, TIndex}.Equals(TSelf, TSelf)" />
    public static virtual bool Equals(scoped TView x, scoped TSelf y) => TSelf.Equals(y, x);

    /// <inheritdoc cref="IRunaStringBase{TSelf, TIndex}.Compare(TSelf, TSelf)" />
    public static abstract int Compare(scoped TSelf x, scoped TView y);

    /// <inheritdoc cref="IRunaStringBase{TSelf, TIndex}.Compare(TSelf, TSelf)" />
    public static virtual int Compare(scoped TView x, scoped TSelf y) => -TSelf.Compare(y, x);

    /// <inheritdoc cref="IRunaStringBase{TSelf, TIndex}.operator==(TSelf, TSelf)" />
    public static abstract bool operator ==(scoped TSelf x, scoped TView y);

    /// <inheritdoc cref="IRunaStringBase{TSelf, TIndex}.operator==(TSelf, TSelf)" />
    public static abstract bool operator ==(scoped TView x, scoped TSelf y);

    /// <inheritdoc cref="IRunaStringBase{TSelf, TIndex}.operator!=(TSelf, TSelf)" />
    public static abstract bool operator !=(scoped TSelf x, scoped TView y);

    /// <inheritdoc cref="IRunaStringBase{TSelf, TIndex}.operator!=(TSelf, TSelf)" />
    public static abstract bool operator !=(scoped TView x, scoped TSelf y);
}


/// <summary>
/// Full featured interface for RunaString types that supports enumeration, slicing, and index mapping.
/// </summary>
/// <typeparam name="TSelf">
/// The type that implements this interface, enabling fluent method chaining and type safety for operations that return a new enumerator.
/// </typeparam>
/// <typeparam name="TView">
/// The RunaString type exposed as the view representation.
/// </typeparam>
/// <typeparam name="TEnumerator">
/// The type of the enumerator that iterates over the runes in the source buffer, which must implement the IRuneEnumerable interface to support slicing and range operations.
/// </typeparam>
/// <typeparam name="TIndex">
/// The type used to index into the source data, which must implement the ISeekIndex interface to support seeking and range operations.
/// </typeparam>
public interface IRunaString<TSelf, TView, TEnumerator, TIndex>
    : IRunaStringBase<TSelf, TIndex>
    , IRunaStringViewable<TSelf, TView, TIndex>
    , IRunaEnumerable<TSelf, TEnumerator>
    where TSelf : IRunaStringViewable<TSelf, TView, TIndex>, allows ref struct
    where TView : IRunaStringBase<TView, TIndex>, allows ref struct
    where TEnumerator : IRunaEnumerator<TEnumerator>, allows ref struct
    where TIndex : unmanaged, IRunaIndex<TIndex>
{
}


/// <summary>
/// Extensions for <see cref="ReadOnlySpan{Char}"/>, <see cref="ReadOnlyMemory{Char}"/>,
/// <see cref="ReadOnlySpan{Rune}"/>, and <see cref="ReadOnlyMemory{Rune}"/>.
/// </summary>
public static partial class RunaString
{
}

namespace RunaString;

/// <inheritdoc cref="ISeekIndex{TSelf}" />
public interface ISeekIndex
{
    /// <summary>
    /// Gets the zero-based index of the current rune in the source buffer.
    /// </summary>
    public int RuneIndex { get; }
}


/// <summary>
/// Defines a type that represents a seekable index into a source buffer for an
/// <see cref="IRuneEnumerable{TSelf, TEnumerator, TIndex}"/> and <see cref="IRuneEnumerator{TSelf, TIndex, TBuffer}"/>.
/// </summary>
/// <typeparam name="TSelf">The type that implements this interface.</typeparam>
public interface ISeekIndex<TSelf> : ISeekIndex, IEquatable<TSelf>, IComparable<TSelf>
    where TSelf : ISeekIndex<TSelf>
{
    /// <summary>
    /// Compares two <typeparamref name="TSelf"/> instances for equality by comparing their byte indices and rune positions.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static abstract bool Equals(TSelf x, TSelf y);

    /// <summary>
    /// Compares two <typeparamref name="TSelf"/> instances by their byte indices and rune positions.
    /// The instances must be originated from the same source string.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">
    /// There is an inconsistency between the byte indices and the rune indices.
    /// These instances may have been originated from different strings.
    /// </exception>
    public static abstract int Compare(TSelf x, TSelf y);

    /// <inheritdoc cref="Equals(TSelf, TSelf)"/>
    public static abstract bool operator ==(TSelf x, TSelf y);

    /// <inheritdoc cref="Equals(TSelf, TSelf)"/>
    public static abstract bool operator !=(TSelf x, TSelf y);

    /// <inheritdoc cref="Compare(TSelf, TSelf)"/>
    public static abstract bool operator <(TSelf x, TSelf y);

    /// <inheritdoc cref="Compare(TSelf, TSelf)"/>
    public static abstract bool operator >(TSelf x, TSelf y);

    /// <inheritdoc cref="Compare(TSelf, TSelf)"/>
    public static abstract bool operator <=(TSelf x, TSelf y);

    /// <inheritdoc cref="Compare(TSelf, TSelf)"/>
    public static abstract bool operator >=(TSelf x, TSelf y);
}


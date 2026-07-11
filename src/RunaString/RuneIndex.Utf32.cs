using System.Diagnostics.CodeAnalysis;

namespace RunaString;

/// <summary>
/// A struct that represents a seekable index into a UTF-32 encoded buffer for an <see cref="Utf32SpanEnumerator"/> or <see cref="Utf32MemoryEnumerator"/>.
/// </summary>
/// <remarks>
/// This instance is only for the source sequence which the instance was created from.
/// It is not valid for other sequences, even if they contain the same data.
/// The behavior is undefined if used with a different source sequence.
/// </remarks>
public readonly struct Utf32RuneIndex : ISeekIndex<Utf32RuneIndex>
{
    /// <summary>
    /// A hash code that represents the source string from which this index was created.
    /// </summary>
    public long SourceHashCode { get; }

    /// <summary></summary>
    public int RuneIndex { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Utf32RuneIndex"/> struct with the specified byte index and rune position.
    /// </summary>
    /// <param name="runePosition"></param>
    internal Utf32RuneIndex(int runePosition)
    {
        RuneIndex = runePosition;
    }

    /// <inheritdoc />
    public override string ToString() =>
        $"Utf32RuneIndex {{ RuneIndex = {RuneIndex} }}";

    /// <inheritdoc />
    public override int GetHashCode() =>
        RuneIndex.GetHashCode();

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is Utf32RuneIndex other && Equals(this, other);

    /// <inheritdoc />
    public bool Equals(Utf32RuneIndex other) => Equals(this, other);

    /// <inheritdoc />
    /// <exception cref="ArgumentException">
    /// There is an inconsistency between the byte indices and the rune indices.
    /// These instances may have been originated from different strings.
    /// </exception>
    public int CompareTo(Utf32RuneIndex other) => Compare(this, other);

    /// <inheritdoc />
    public static bool Equals(Utf32RuneIndex x, Utf32RuneIndex y) =>
        x.RuneIndex == y.RuneIndex;

    /// <inheritdoc />
    public static int Compare(Utf32RuneIndex x, Utf32RuneIndex y)
    {
        if (Equals(x, y))
        {
            return 0;
        }
        return x.RuneIndex < y.RuneIndex ? -1 : 1;
    }

    /// <inheritdoc />
    public static bool operator ==(Utf32RuneIndex x, Utf32RuneIndex y) => Equals(x, y);

    /// <inheritdoc />
    public static bool operator !=(Utf32RuneIndex x, Utf32RuneIndex y) => !Equals(x, y);

    /// <inheritdoc />
    public static bool operator <(Utf32RuneIndex x, Utf32RuneIndex y) => Compare(x, y) < 0;

    /// <inheritdoc />
    public static bool operator >(Utf32RuneIndex x, Utf32RuneIndex y) => Compare(x, y) > 0;

    /// <inheritdoc />
    public static bool operator <=(Utf32RuneIndex x, Utf32RuneIndex y) => Compare(x, y) <= 0;

    /// <inheritdoc />
    public static bool operator >=(Utf32RuneIndex x, Utf32RuneIndex y) => Compare(x, y) >= 0;
}

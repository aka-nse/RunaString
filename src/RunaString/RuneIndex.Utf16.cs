using System.Diagnostics.CodeAnalysis;

namespace RunaString;

/// <summary>
/// A struct that represents a seekable index into a UTF-16 encoded buffer for an <see cref="CharsSpanEnumerator"/> or <see cref="CharsMemoryEnumerator"/>.
/// </summary>
/// <remarks>
/// This instance is only for the source sequence which the instance was created from.
/// It is not valid for other sequences, even if they contain the same data.
/// The behavior is undefined if used with a different source sequence.
/// </remarks>
public readonly struct CharsRuneIndex : ISeekIndex<CharsRuneIndex>
{
    /// <summary></summary>
    public int CharIndex { get; }

    /// <summary></summary>
    public int RuneIndex { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CharsRuneIndex"/> struct with the specified byte index and rune position.
    /// </summary>
    /// <param name="charIndex"></param>
    /// <param name="runePosition"></param>
    internal CharsRuneIndex(int charIndex, int runePosition)
    {
        CharIndex = charIndex;
        RuneIndex = runePosition;
    }

    /// <inheritdoc />
    public override string ToString() =>
        $"CharsRuneIndex {{ CharIndex = {CharIndex}, RuneIndex = {RuneIndex} }}";

    /// <inheritdoc />
    public override int GetHashCode() =>
        CharIndex.GetHashCode() ^ RuneIndex.GetHashCode();

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is CharsRuneIndex other && Equals(this, other);

    /// <inheritdoc />
    public bool Equals(CharsRuneIndex other) => Equals(this, other);

    /// <inheritdoc />
    /// <exception cref="ArgumentException">
    /// There is an inconsistency between the byte indices and the rune indices.
    /// These instances may have been originated from different strings.
    /// </exception>
    public int CompareTo(CharsRuneIndex other) => Compare(this, other);

    /// <inheritdoc />
    public static bool Equals(CharsRuneIndex x, CharsRuneIndex y) =>
        x.CharIndex == y.CharIndex && x.RuneIndex == y.RuneIndex;

    /// <inheritdoc />
    public static int Compare(CharsRuneIndex x, CharsRuneIndex y)
    {
        if (Equals(x, y))
        {
            return 0;
        }
        return (x.CharIndex < y.CharIndex, x.RuneIndex < y.RuneIndex) switch
        {
            (true, true) => -1,
            (false, false) => +1,
            _ => throw new ArgumentException($"Inconsistent source hash codes: x and y may be from different source strings."),
        };
    }

    /// <inheritdoc />
    public static bool operator ==(CharsRuneIndex x, CharsRuneIndex y) => Equals(x, y);

    /// <inheritdoc />
    public static bool operator !=(CharsRuneIndex x, CharsRuneIndex y) => !Equals(x, y);

    /// <inheritdoc />
    public static bool operator <(CharsRuneIndex x, CharsRuneIndex y) => Compare(x, y) < 0;

    /// <inheritdoc />
    public static bool operator >(CharsRuneIndex x, CharsRuneIndex y) => Compare(x, y) > 0;

    /// <inheritdoc />
    public static bool operator <=(CharsRuneIndex x, CharsRuneIndex y) => Compare(x, y) <= 0;

    /// <inheritdoc />
    public static bool operator >=(CharsRuneIndex x, CharsRuneIndex y) => Compare(x, y) >= 0;
}

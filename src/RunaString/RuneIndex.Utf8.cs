using System.Diagnostics.CodeAnalysis;

namespace RunaString;

/// <summary>
/// A struct that represents a seekable index into a UTF-8 encoded buffer for an <see cref="Utf8SpanEnumerator"/> or <see cref="Utf8MemoryEnumerator"/>.
/// </summary>
/// <remarks>
/// This instance is only for the source sequence which the instance was created from.
/// It is not valid for other sequences, even if they contain the same data.
/// The behavior is undefined if used with a different source sequence.
/// </remarks>
public readonly struct Utf8RuneIndex : ISeekIndex<Utf8RuneIndex>
{
    /// <summary></summary>
    public int ByteIndex { get; }

    /// <summary></summary>
    public int RuneIndex { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Utf8RuneIndex"/> struct with the specified byte index and rune position.
    /// </summary>
    /// <param name="byteIndex"></param>
    /// <param name="runePosition"></param>
    internal Utf8RuneIndex(int byteIndex, int runePosition)
    {
        ByteIndex = byteIndex;
        RuneIndex = runePosition;
    }

    /// <inheritdoc />
    public override string ToString() =>
        $"Utf8RuneIndex {{ ByteIndex = {ByteIndex}, RuneIndex = {RuneIndex} }}";

    /// <inheritdoc />
    public override int GetHashCode() =>
        ByteIndex.GetHashCode() ^ RuneIndex.GetHashCode();

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is Utf8RuneIndex other && Equals(this, other);

    /// <inheritdoc />
    public bool Equals(Utf8RuneIndex other) => Equals(this, other);

    /// <inheritdoc />
    /// <exception cref="ArgumentException">
    /// There is an inconsistency between the byte indices and the rune indices.
    /// These instances may have been originated from different strings.
    /// </exception>
    public int CompareTo(Utf8RuneIndex other) => Compare(this, other);

    /// <inheritdoc />
    public static bool Equals(Utf8RuneIndex x, Utf8RuneIndex y) =>
        x.ByteIndex == y.ByteIndex && x.RuneIndex == y.RuneIndex;

    /// <inheritdoc />
    public static int Compare(Utf8RuneIndex x, Utf8RuneIndex y)
    {
        if (Equals(x, y))
        {
            return 0;
        }
        return (x.ByteIndex < y.ByteIndex, x.RuneIndex < y.RuneIndex) switch
        {
            (true, true) => -1,
            (false, false) => +1,
            _ => throw new ArgumentException($"Inconsistent source hash codes: x and y may be from different source strings."),
        };
    }

    /// <inheritdoc />
    public static bool operator ==(Utf8RuneIndex x, Utf8RuneIndex y) => Equals(x, y);

    /// <inheritdoc />
    public static bool operator !=(Utf8RuneIndex x, Utf8RuneIndex y) => !Equals(x, y);

    /// <inheritdoc />
    public static bool operator<(Utf8RuneIndex x, Utf8RuneIndex y) => Compare(x, y) < 0;

    /// <inheritdoc />
    public static bool operator >(Utf8RuneIndex x, Utf8RuneIndex y) => Compare(x, y) > 0;

    /// <inheritdoc />
    public static bool operator <=(Utf8RuneIndex x, Utf8RuneIndex y) => Compare(x, y) <= 0;

    /// <inheritdoc />
    public static bool operator >=(Utf8RuneIndex x, Utf8RuneIndex y) => Compare(x, y) >= 0;
}

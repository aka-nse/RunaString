using System.Diagnostics.CodeAnalysis;

namespace RunaString;

/// <summary>
/// A struct that represents a seekable index into a <see cref="System.Text.Rune"/> buffer for an <see cref="RunesSpanEnumerator"/> or <see cref="RunesMemoryEnumerator"/>.
/// </summary>
/// <remarks>
/// This instance is only for the source sequence which the instance was created from.
/// It is not valid for other sequences, even if they contain the same data.
/// The behavior is undefined if used with a different source sequence.
/// </remarks>
public readonly struct RunesIndex : ISeekIndex<RunesIndex>
{
    /// <summary>
    /// A hash code that represents the source string from which this index was created.
    /// </summary>
    public long SourceHashCode { get; }

    /// <summary></summary>
    public int RuneIndex { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RunesIndex"/> struct with the specified byte index and rune position.
    /// </summary>
    /// <param name="runePosition"></param>
    internal RunesIndex(int runePosition)
    {
        RuneIndex = runePosition;
    }

    /// <inheritdoc />
    public override string ToString() =>
        $"RunesIndex {{ RuneIndex = {RuneIndex} }}";

    /// <inheritdoc />
    public override int GetHashCode() =>
        RuneIndex.GetHashCode();

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is RunesIndex other && Equals(this, other);

    /// <inheritdoc />
    public bool Equals(RunesIndex other) => Equals(this, other);

    /// <inheritdoc />
    /// <exception cref="ArgumentException">
    /// There is an inconsistency between the byte indices and the rune indices.
    /// These instances may have been originated from different strings.
    /// </exception>
    public int CompareTo(RunesIndex other) => Compare(this, other);

    /// <inheritdoc />
    public static bool Equals(RunesIndex x, RunesIndex y) =>
        x.RuneIndex == y.RuneIndex;

    /// <inheritdoc />
    public static int Compare(RunesIndex x, RunesIndex y)
    {
        if (Equals(x, y))
        {
            return 0;
        }
        return x.RuneIndex < y.RuneIndex ? -1 : 1;
    }

    /// <inheritdoc />
    public static bool operator ==(RunesIndex x, RunesIndex y) => Equals(x, y);

    /// <inheritdoc />
    public static bool operator !=(RunesIndex x, RunesIndex y) => !Equals(x, y);

    /// <inheritdoc />
    public static bool operator <(RunesIndex x, RunesIndex y) => Compare(x, y) < 0;

    /// <inheritdoc />
    public static bool operator >(RunesIndex x, RunesIndex y) => Compare(x, y) > 0;

    /// <inheritdoc />
    public static bool operator <=(RunesIndex x, RunesIndex y) => Compare(x, y) <= 0;

    /// <inheritdoc />
    public static bool operator >=(RunesIndex x, RunesIndex y) => Compare(x, y) >= 0;
}

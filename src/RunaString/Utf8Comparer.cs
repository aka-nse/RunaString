using System.Diagnostics.CodeAnalysis;

namespace RunaString;

/// <summary>
/// Provides methods for comparing UTF-8 encoded strings and spans, as well as computing their hash codes.
/// </summary>
public abstract partial class Utf8Comparer
    : IComparer<Utf8SpanString>
    , IComparer<Utf8String>
    , IEqualityComparer<Utf8SpanString>
    , IEqualityComparer<Utf8String>
{
    /// <summary>
    /// Gets a default instance of the <see cref="Utf8Comparer"/> class that performs ordinal (binary) comparisons of UTF-8 encoded strings and spans.
    /// </summary>
    public static Utf8Comparer Default { get; } = new Default_();

    private sealed class Default_ : Utf8Comparer
    {
        public override int Compare(Utf8SpanString x, Utf8SpanString y) =>
            InternalHelpers.Compare(x, y);

        public override bool Equals(Utf8SpanString x, Utf8SpanString y) =>
            InternalHelpers.Equals(x, y);

        public override int GetHashCode([NotNull] Utf8SpanString obj) =>
            InternalHelpers.GetHashCode(obj.Buffer);
    }

    private Utf8Comparer()
    {
    }

    /// <inheritdoc />
    public abstract int Compare(Utf8SpanString x, Utf8SpanString y);

    /// <inheritdoc />
    public int Compare(Utf8String x, Utf8String y) =>
        Compare(x, y);

    /// <inheritdoc />
    public abstract bool Equals(Utf8SpanString x, Utf8SpanString y);

    /// <inheritdoc />
    public bool Equals(Utf8String x, Utf8String y) =>
        Equals((Utf8SpanString)x, (Utf8SpanString)y);

    /// <inheritdoc />
    public abstract int GetHashCode([DisallowNull] Utf8SpanString obj);

    /// <inheritdoc />
    public int GetHashCode([DisallowNull] Utf8String obj) =>
        GetHashCode((Utf8SpanString)obj);
}

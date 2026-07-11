using System.Runtime.CompilerServices;

namespace RunaString.Test;

public partial class RuneEnumerableTest
{
    private static int GetUtf8CodeUnitCount(string s, int start, int count) =>
        s.EnumerateRunes()
        .Skip(start)
        .Take(count)
        .Select(static x => x.Utf8SequenceLength)
        .Sum();


    private static int GetCharsCodeUnitCount(string s, int start, int count) =>
        s.EnumerateRunes()
        .Skip(start)
        .Take(count)
        .Select(static x => x.Utf16SequenceLength)
        .Sum();

    private static int GetRunesCodeUnitCount(string s, int start, int count) =>
        s.EnumerateRunes()
        .Skip(start)
        .Take(count)
        .Count();

    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    private static extern Utf8Index CreateUtf8Index(int byteIndex, int runePosition);

    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    private static extern CharsIndex CreateCharsIndex(int charIndex, int runePosition);

    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    private static extern RunesIndex CreateRunesIndex(int runePosition);
}

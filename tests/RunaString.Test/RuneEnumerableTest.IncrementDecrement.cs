using System.Text;

namespace RunaString.Test;

public partial class RuneEnumerableTest
{
    /// <summary>
    /// Provides test cases covering a variety of Unicode inputs (multiple languages, diacritics, combining marks, and emoji sequences)
    /// for increment/decrement rune iteration tests. Uses well-known test strings where appropriate.
    /// </summary>
    public static TheoryData<string, int> IncrementDecrementTestCases()
    {
        var retval = new TheoryData<string, int>();

        void core(string input, int runeIndex)
        {
            retval.Add(input, runeIndex);
        }

        // Well-known pangrams and English test strings
        core("The quick brown fox jumps over the lazy dog", 0);
        core("The quick brown fox jumps over the lazy dog", 10);
        core("The quick brown fox jumps over the lazy dog", 43);

        core("Sphinx of black quartz, judge my vow", 0);
        core("Sphinx of black quartz, judge my vow", 7);
        core("Sphinx of black quartz, judge my vow", 30);

        // Accented and combining characters
        core("Ångström façade Noël", 0);
        core("Ångström façade Noël", 3);
        core("Noe\u0308l — combining diaeresis", 2); // "Noël" using combining diaeresis

        // Cyrillic, Arabic, Devanagari, CJK, Korean, Hebrew
        core("Привет, мир!", 0);
        core("Привет, мир!", 3);

        core("مرحبا بالعالم", 0);
        core("مرحبا بالعالم", 5);

        core("नमस्ते दुनिया", 0);
        core("नमस्ते दुनिया", 4);

        core("汉字テスト", 0);
        core("汉字テスト", 2);

        core("안녕하세요 세계", 0);
        core("안녕하세요 세계", 5);

        core("שָׁלוֹם עוֹלָם", 0);
        core("שָׁלוֹם עוֹלָם", 3);

        // Emoji: single, modifier, ZWJ sequences, flags, keycap sequences
        core("😀", 0);
        core("😀", 1);

        core("👍🏽", 0);
        core("👍🏽", 1);

        core("👩‍⚕️", 0);
        core("👩‍⚕️", 2);

        core("👨‍👩‍👧‍👦", 0);
        core("👨‍👩‍👧‍👦", 1);

        core("🇯🇵", 0);
        core("🇯🇵", 2);

        core("1️⃣2️⃣3️⃣", 0);
        core("1️⃣2️⃣3️⃣", 2);

        core("🧑‍🚀🚀", 0);
        core("🧑‍🚀🚀", 2);

        return retval;
    }

    [Theory]
    [MemberData(nameof(IncrementDecrementTestCases))]
    public void TestIncrementUtf8(string input, int runeIndex)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var charIndex = GetUtf8CodeUnitCount(input, 0, runeIndex);
        var runeLength = input.EnumerateRunes().Count();
        var nextRuneIndex = runeIndex + 1;
        var index = CreateUtf8RuneIndex(charIndex, runeIndex);
        var memory = Utf8String.DangerousFromUtf8([.. bytes], 0, bytes.Length);
        var span = Utf8Span.DangerousFromSpan(bytes);
        if ((uint)nextRuneIndex < (uint)runeLength)
        {
            var nextCharIndex = GetUtf8CodeUnitCount(input, 0, nextRuneIndex);
            var nextIndex = CreateUtf8RuneIndex(nextCharIndex, nextRuneIndex);
            IncrementTestCore_True(
                memory,
                index,
                nextIndex);
            IncrementTestCore_True(
                span,
                index,
                nextIndex);
        }
        else
        {
            IncrementTestCore_False(
                memory,
                index);
            IncrementTestCore_False(
                span,
                index);
        }
    }

    [Theory]
    [MemberData(nameof(IncrementDecrementTestCases))]
    public void TestIncrementUtf16(string input, int runeIndex)
    {
        var charIndex = GetUtf16CodeUnitCount(input, 0, runeIndex);
        var runeLength = input.EnumerateRunes().Count();
        var nextRuneIndex = runeIndex + 1;
        var index = CreateUtf16RuneIndex(charIndex, runeIndex);
        var memory = new Utf16MemoryEnumerable(input.AsMemory());
        var span = new Utf16SpanEnumerable(input.AsSpan());
        if ((uint)nextRuneIndex < (uint)runeLength)
        {
            var nextCharIndex = GetUtf16CodeUnitCount(input, 0, nextRuneIndex);
            var nextIndex = CreateUtf16RuneIndex(nextCharIndex, nextRuneIndex);
            IncrementTestCore_True(memory, index, nextIndex);
            IncrementTestCore_True(span, index, nextIndex);
        }
        else
        {
            IncrementTestCore_False(memory, index);
            IncrementTestCore_False(span, index);
        }
    }

    [Theory]
    [MemberData(nameof(IncrementDecrementTestCases))]
    public void TestIncrementUtf32(string input, int runeIndex)
    {
        var runes = input.EnumerateRunes().ToArray();
        var runeLength = input.EnumerateRunes().Count();
        var nextRuneIndex = runeIndex + 1;
        var index = CreateUtf32RuneIndex(runeIndex);
        var memory = new Utf32MemoryEnumerable(runes);
        var span = new Utf32SpanEnumerable(runes);
        if ((uint)nextRuneIndex < (uint)runeLength)
        {
            var nextIndex = CreateUtf32RuneIndex(nextRuneIndex);
            IncrementTestCore_True(memory, index, nextIndex);
            IncrementTestCore_True(span, index, nextIndex);
        }
        else
        {
            IncrementTestCore_False(memory, index);
            IncrementTestCore_False(span, index);
        }
    }

    private static void IncrementTestCore_True<TStr, TIndex>(TStr input, TIndex index, TIndex expected)
        where TStr : IRuneString<TStr, TIndex>, allows ref struct
        where TIndex : struct, ISeekIndex
    {
        Assert.True(input.TryIncrement(ref index));
        Assert.Equal(expected, index);
    }

    private static void IncrementTestCore_False<TStr, TIndex>(TStr input, TIndex index)
        where TStr : IRuneString<TStr, TIndex>, allows ref struct
        where TIndex : struct, ISeekIndex
    {
        Assert.False(input.TryIncrement(ref index));
    }

    [Theory]
    [MemberData(nameof(IncrementDecrementTestCases))]
    public void TestDecrementUtf8(string input, int runeIndex)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var charIndex = GetUtf8CodeUnitCount(input, 0, runeIndex);
        var runeLength = input.EnumerateRunes().Count();
        var nextRuneIndex = runeIndex - 1;
        var index = CreateUtf8RuneIndex(charIndex, runeIndex);
        var memory = Utf8String.DangerousFromUtf8([.. bytes], 0, bytes.Length);
        var span = Utf8Span.DangerousFromSpan(bytes);
        if ((uint)nextRuneIndex < (uint)runeLength)
        {
            var nextCharIndex = GetUtf8CodeUnitCount(input, 0, nextRuneIndex);
            var nextIndex = CreateUtf8RuneIndex(nextCharIndex, nextRuneIndex);
            DecrementTestCore_True(memory, index, nextIndex);
            DecrementTestCore_True(span, index, nextIndex);
        }
        else
        {
            DecrementTestCore_False(memory, index);
            DecrementTestCore_False(span, index);
        }
    }

    [Theory]
    [MemberData(nameof(IncrementDecrementTestCases))]
    public void TestDecrementUtf16(string input, int runeIndex)
    {
        var charIndex = GetUtf16CodeUnitCount(input, 0, runeIndex);
        var runeLength = input.EnumerateRunes().Count();
        var nextRuneIndex = runeIndex - 1;
        var index = CreateUtf16RuneIndex(charIndex, runeIndex);
        var memory = new Utf16MemoryEnumerable(input.AsMemory());
        var span = new Utf16SpanEnumerable(input.AsSpan());
        if ((uint)nextRuneIndex < (uint)runeLength)
        {
            var nextCharIndex = GetUtf16CodeUnitCount(input, 0, nextRuneIndex);
            var nextIndex = CreateUtf16RuneIndex(nextCharIndex, nextRuneIndex);
            DecrementTestCore_True(memory, index, nextIndex);
            DecrementTestCore_True(span, index, nextIndex);
        }
        else
        {
            DecrementTestCore_False(memory, index);
            DecrementTestCore_False(span, index);
        }
    }

    [Theory]
    [MemberData(nameof(IncrementDecrementTestCases))]
    public void TestDecrementUtf32(string input, int runeIndex)
    {
        var runes = input.EnumerateRunes().ToArray();
        var runeLength = input.EnumerateRunes().Count();
        var nextRuneIndex = runeIndex - 1;
        var index = CreateUtf32RuneIndex(runeIndex);
        var memory = new Utf32MemoryEnumerable(runes);
        var span = new Utf32SpanEnumerable(runes);
        if ((uint)nextRuneIndex < (uint)runeLength)
        {
            var nextIndex = CreateUtf32RuneIndex(nextRuneIndex);
            DecrementTestCore_True(memory, index, nextIndex);
            DecrementTestCore_True(span, index, nextIndex);
        }
        else
        {
            DecrementTestCore_False(memory, index);
            DecrementTestCore_False(span, index);
        }
    }

    private static void DecrementTestCore_True<TStr, TIndex>(TStr input, TIndex index, TIndex expected)
        where TStr : IRuneString<TStr, TIndex>, allows ref struct
        where TIndex : struct, ISeekIndex
    {
        Assert.True(input.TryDecrement(ref index));
        Assert.Equal(expected, index);
    }

    private static void DecrementTestCore_False<TStr, TIndex>(TStr input, TIndex index)
        where TStr : IRuneString<TStr, TIndex>, allows ref struct
        where TIndex : struct, ISeekIndex
    {
        Assert.False(input.TryDecrement(ref index));
    }
}

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using RunaString.RegularExpressions;

namespace RunaString.Test.RegularExpressions;

public class RegexTest
{
    public static TheoryData<string, RegexOptions> ParsePatternData()
    {
        TheoryData<string, RegexOptions> data = [];
        void add([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, RegexOptions options = default)
        {
            data.Add(pattern, options);
        }

        add(@"", RegexOptions.None);
        add(@"", RegexOptions.IgnoreCase);
        add(@"", RegexOptions.Multiline);
        add(@"", RegexOptions.Singleline);
        add(@"", RegexOptions.ExplicitCapture);
        add(@"", RegexOptions.Compiled);
        add(@"", RegexOptions.CultureInvariant);
        add(@"", RegexOptions.RightToLeft);
        add(@"", RegexOptions.ECMAScript);
        add(@"a(b|c)*d");
        add(@"abc");
        add(@"a|b");
        add(@"ab*c");
        add(@"ab+c");
        add(@"ab?c");
        add(@"a{3}");
        add(@"a{2,4}");
        add(@"a{2,}");
        add(@"(abc)");
        add(@"(?:abc)");
        add(@"(?<name>abc)");
        add(@"(a)\1");
        add(@"[abc]");
        add(@"[^abc]");
        add(@"[a-zA-Z0-9_]");
        add(@"\d+\.\d*");
        add(@"^\w+$");
        add(@"\s*=\s*");
        add(@"(foo|bar|baz)");
        add(@"(ab){2,3}");
        add(@"colou?r");
        add(@"\bword\b");
        add(@"(?i)abc");
        add(@"(?m)^\s*#.*$");
        add(@"\p{L}+");
        add(@"[\u3040-\u309F]+");
        add(@"こんにちは");
        add(@"café");
        add(@"straße");
        add(@"Δx=α+β");
        add(@"добро");
        add(@"مرحبا");
        add(@"[\u00C0-\u00FF]+");
        add(@"[ぁ-んァ-ヶ一-龠]+");

        return data;
    }

    [Theory]
    [MemberData(nameof(ParsePatternData))]
    public void ParsePattern(string pattern, RegexOptions options)
    {
        var cultureInfo = CultureInfo.InvariantCulture;
        var actualTree = RegexParser.Parse(pattern, options, cultureInfo);
        var actualCode = RegexWriter.Write(actualTree);

        Helpers.GetExpectedInterpreterCode(
            pattern,
            options,
            cultureInfo,
            out var expectedOptions,
            out var expectedCodes,
            out var expectedStrings,
            out var expectedStringsAsciiLookup,
            out var expectedTrackCount);

        Assert.Equal(expectedOptions, actualCode.Options);
        Assert.Equal(expectedCodes, actualCode.Codes);
        Assert.Equal(expectedStrings, actualCode.Strings);
        Assert.Equal(
                expectedStringsAsciiLookup,
                actualCode.StringsAsciiLookup,
                static (x, y) => (x, y) switch
                {
                    (null, null) => true,
                    (null, _) => false,
                    (_, null) => false,
                    _ => Enumerable.SequenceEqual(x, y),
                }
            );
        Assert.Equal(expectedTrackCount, actualCode.TrackCount);
    }
}


file static class Helpers
{
    private static readonly Func<string, RegexOptions, CultureInfo?, object/*RegexInterpreterCode*/> _compileRegexPattern;
    private static readonly FieldInfo _field_RegexInterpreterCode_Options;
    private static readonly FieldInfo _field_RegexInterpreterCode_Codes;
    private static readonly FieldInfo _field_RegexInterpreterCode_Strings;
    private static readonly FieldInfo _field_RegexInterpreterCode_StringsAsciiLookup;
    private static readonly FieldInfo _field_RegexInterpreterCode_TrackCount;

    static Helpers()
    {
        const BindingFlags staticMemberFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        const BindingFlags instanceMemberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        const string ns = "System.Text.RegularExpressions";
        var assm = typeof(Regex).Assembly;
        var type_RegexTree = assm.GetType($"{ns}.RegexTree", throwOnError: true)!;
        var type_RegexParser = assm.GetType($"{ns}.RegexParser", throwOnError: true)!;
        var type_RegexWriter = assm.GetType($"{ns}.RegexWriter", throwOnError: true)!;
        var type_RegexInterpreterCode = assm.GetType($"{ns}.RegexInterpreterCode", throwOnError: true)!;

        var method_RegexParser_Parse = type_RegexParser
            .GetMethod("Parse", staticMemberFlags, null, [typeof(string), typeof(RegexOptions), typeof(CultureInfo)], null)!;
        var method_RegexWriter_Write = type_RegexWriter
            .GetMethod("Write", staticMemberFlags, null, [type_RegexTree], null)!;

        {
            var arg0Expr = Expression.Parameter(typeof(string), "pattern");
            var arg1Expr = Expression.Parameter(typeof(RegexOptions), "options");
            var arg2Expr = Expression.Parameter(typeof(CultureInfo), "culture");
            var callParseExpr = Expression.Call(method_RegexParser_Parse, [arg0Expr, arg1Expr, arg2Expr]);
            var callWriteExpr = Expression.Call(method_RegexWriter_Write, callParseExpr);
            var lambdaExpr = Expression.Lambda<Func<string, RegexOptions, CultureInfo?, object>>(callWriteExpr, [arg0Expr, arg1Expr, arg2Expr]);
            _compileRegexPattern = lambdaExpr.Compile();
        }

        _field_RegexInterpreterCode_Options = type_RegexInterpreterCode
            .GetField("Options", instanceMemberFlags)!;
        _field_RegexInterpreterCode_Codes = type_RegexInterpreterCode
            .GetField("Codes", instanceMemberFlags)!;
        _field_RegexInterpreterCode_Strings = type_RegexInterpreterCode
            .GetField("Strings", instanceMemberFlags)!;
        _field_RegexInterpreterCode_StringsAsciiLookup = type_RegexInterpreterCode
            .GetField("StringsAsciiLookup", instanceMemberFlags)!;
        _field_RegexInterpreterCode_TrackCount = type_RegexInterpreterCode
            .GetField("TrackCount", instanceMemberFlags)!;
    }

    public static void GetExpectedInterpreterCode(
        string pattern,
        RegexOptions options,
        CultureInfo culture,
        out RegexOptions expectedOptions,
        out int[] expectedCodes,
        out string[] expectedStrings,
        out uint[]?[] expectedStringsAsciiLookup,
        out int expectedTrackCount)
    {
        var code = _compileRegexPattern(pattern, options, culture)!;
        expectedOptions = (RegexOptions)_field_RegexInterpreterCode_Options.GetValue(code)!;
        expectedCodes = (int[])_field_RegexInterpreterCode_Codes.GetValue(code)!;
        expectedStrings = (string[])_field_RegexInterpreterCode_Strings.GetValue(code)!;
        expectedStringsAsciiLookup = (uint[]?[])_field_RegexInterpreterCode_StringsAsciiLookup.GetValue(code)!;
        expectedTrackCount = (int)_field_RegexInterpreterCode_TrackCount.GetValue(code)!;
    }
}
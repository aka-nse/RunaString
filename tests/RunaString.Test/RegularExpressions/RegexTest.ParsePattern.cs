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

        // 1. Literals and escapes
        add(@"abcXYZ012");         // Regular literal
        add(@"\.");                // Escaped metacharacter
        add(@"\\");                // Backslash
        add(@"\a\b\e\f\n\r\t\v");  // Control character escapes
        add(@"\0");                // Null character
        add(@"\x41\x0042");        // Hexadecimal character escapes
        add(@"\u3042\u6F22");      // Unicode escapes
        add(@"\cA");               // Control character
        add(@"[\]\-\\]");          // Escapes inside character class

        // 2. Character classes
        add(@"[abc]");             // Character class
        add(@"[^abc]");            // Negated character class
        add(@"[a-zA-Z0-9]");       // Character ranges
        add(@"[a-fx-z_]");         // Multiple ranges and a literal
        add(@"[\]\-\^]");          // Special characters inside a character class
        add(@"\d");                // Digit
        add(@"\D");                // Non-digit
        add(@"\w");                // Word character
        add(@"\W");                // Non-word character
        add(@"\s");                // Whitespace character
        add(@"\S");                // Non-whitespace character
        add(@"\p{Lu}");            // Unicode category
        add(@"\P{Lu}");            // Negated Unicode category
        add(@"\p{IsBasicLatin}");  // Unicode block
        add(@"\P{IsBasicLatin}");  // Negated Unicode block
        add(@".");                 // Wildcard

        // 3. Quantifiers
        add(@"a*");       // Zero or more
        add(@"a+");       // One or more
        add(@"a?");       // Zero or one
        add(@"a{3}");     // Exact count
        add(@"a{2,}");    // Minimum count
        add(@"a{2,5}");   // Range count
        add(@"a*?");      // Lazy match `*?`
        add(@"a+?");      // Lazy match `+?`
        add(@"a??");      // Lazy match `??`
        add(@"a{3}?");    // Lazy match `{n}?`
        add(@"a{2,}?");   // Lazy match `{n,}?`
        add(@"a{2,5}?");  // Lazy match `{n,m}?`

        // 4. Anchors and position assertions
        add(@"^abc");      // Start of line / start of string
        add(@"abc$");      // End of line / end of string
        add(@"\Aabc");     // Start of string
        add(@"abc\z");     // End of string
        add(@"abc\Z");     // End of string or before final newline
        add(@"\Gabc");     // Position where previous match ended
        add(@"\bword\b");  // Word boundary
        add(@"\Bword\B");  // Non-word boundary

        // 5. Groups and captures
        add(@"(abc)");                    // Capturing group
        add(@"((a)(b(c)))");              // Nested groups
        add(@"(?<name>abc)");             // Named capture `<name>`
        add(@"(?'name'abc)");             // Named capture `'name'`
        add(@"(?:abc)");                  // Non-capturing group
        add(@"(?i)abc");                  // Inline option
        add(@"(?im-sx)abc");              // Enable/disable options
        add(@"(?i:abc)");                 // Scoped option in group
        add(@"(?i-s:abc)");               // Disable options in scoped group
        add(@"(a)?(?(1)b|c)");            // Conditional (capture exists)
        add(@"(?<name>a)?(?(name)b|c)");  // Conditional (named capture exists)

        // 6. Backreferences
        add(@"(a)\1");                   // Numeric backreference
        add(@"(a)(b)\2\1");              // Multiple capture references
        add(@"(?<word>\w+)\s\k<word>");  // Named backreference `<name>`
        add(@"(?'word'\w+)\s\k<word>");  // Named backreference `'name'`
        add(@"(?<1>a)\k<1>");            // Backreference by numeric name
        add(@"(a)?(?(1)\1\\|b)");        // Optional capture reference

        // 7. Alternation
        add(@"cat|dog");         // Simple alternation
        add(@"(?:cat|dog)");     // Alternation in group
        add(@"red|green|blue");  // Multiple alternatives
        add(@"(a|b(c|d))");      // Nested alternation
        add(@"(?:ab|cd)+");      // Alternation with quantifier

        // 8. Lookahead and lookbehind
        add(@"a(?=b)");                   // Positive lookahead
        add(@"a(?!b)");                   // Negative lookahead
        add(@"(?<=a)b");                  // Positive lookbehind
        add(@"(?<!a)b");                  // Negative lookbehind
        add(@"\w+(?=\s+\d+)");            // Compound lookahead
        add(@"(?<=\bID:)\d+");            // Compound lookbehind
        add(@"(?=.*[A-Z])(?!.*[!])\w+");  // Lookahead + negative lookahead

        // 9. Atomic groups
        add(@"(?>a|ab)c");       // Atomic group
        add(@"(?>\w+)\d");       // Atomic group with quantifier
        add(@"(?>a(?>bc|b))d");  // Nested atomic group

        // 10. Balancing groups
        add(@"(?<Open>a)(?<Close-Open>b)");                                     // Balancing group definition
        add(@"(?<Open>\()(?<Close-Open>\))");                                   // Pop from named group stack
        add(@"^(?<Open>\()+(?<Close-Open>\))+(?(Open)(?!))$");                  // Balancing group with conditional
        add(@"^((?'Open'\()[^()]*)+((?'Close-Open'\))[^()]*)+(?(Open)(?!))$");  // Parentheses nesting support

        // 11. Conditionals
        add(@"(a)?(?(1)b|c)");             // Capture group existence
        add(@"(?<name>a)?(?(name)b|c)");   // Named group existence
        add(@"(?(?=a)b|c)");               // Lookahead conditional
        add(@"(?(?!a)b|c)");               // Negative lookahead conditional
        add(@"(?(?<=a)b|c)");              // Lookbehind conditional
        add(@"(?(?<!a)b|c)");              // Negative lookbehind conditional

        // 12. Comment syntax
        add(@"(?# this is a comment)a");             // Inline comment
        add(@"(?<word>\w+)(?# capture a word)\s+");  // Group with comment

        // 13. Practical and advanced composite patterns
        add(@"(?:[^""\\]|\\.)*");                   // Escaped string
        add(@"\A(?:[A-Z][a-z0-9]*)+\z");            // PascalCase identifier
        add(@"\A[A-Za-z_][A-Za-z0-9_]*\z");         // Identifier
        add(@"\b(?<word>\w+)\s+\k<word>\b");        // Duplicate word
        add(@"\A[+-]?(?:\d+(?:\.\d*)?|\.\d+)\z");   // Integer or decimal
        add(@"\A(?:[A-Za-z]:)?(?:\\[^\\<>:""/|?*]+)+\\?\z");            // Path-like string
        add(@"\A(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})\z");       // Date format
        add(@"<(?<tag>[A-Za-z][A-Za-z0-9]*)\b[^>]*>.*?</\k<tag>>");     // HTML-like tag
        add(@"(?<quote>['""])(?:\\.|(?!\k<quote>)[^\\])*\k<quote>");    // Quoted string
        add(@"\A(?>[^()]|(?<Open>\()|(?<-Open>\)))*(?(Open)(?!))\z");   // Balancing parentheses
        add(@"(?:^|,)(?:""(?<value>(?:[^""]|"""")*)""|(?<value>[^,]*))");               // CSV-like field
        add(@"\A(?<Open>\()*(?:[^()]|(?<Open>\()|(?<-Open>\)))*(?(Open)(?!))\z");       // Nested parentheses
        add(@"\A(?<value>[+-]?(?:\d+(?:\.\d*)?|\.\d+))\s*(?<unit>ms|s|kg|m|cm)\z");     // Numeric value with unit
        add(@"\A(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?!.*\s)[A-Za-z\d!@#$%^&*]{8,32}\z");     // Policy-based password
        add(@"\A(?<hour>[01]\d|2[0-3]):(?<minute>[0-5]\d)(?::(?<second>[0-5]\d))?\z");  // Time format
        add(@"\A(?:(?:25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)\.){3}(?:25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)\z");                    // IPv4-like address
        add(@"\A\[(?<timestamp>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})\]\s+(?<level>INFO|WARN|ERROR)\s+(?<message>.*)\z"); // Log line
        add(@"\A(?<scheme>https?)://(?<host>[A-Za-z0-9.-]+)(?::(?<port>\d{1,5}))?(?<path>/[^\s?#]*)?(?:\?(?<query>[^#\s]*))?(?:\#(?<fragment>\S*))?\z");  // URL-like string


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
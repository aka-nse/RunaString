using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
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
            out var expectedTree,
            out var expectedCode);

        Assert.True(Helpers.RegexTreeAccessor.Equals(expectedTree, actualTree));
        Assert.True(Helpers.RegexInterpreterCodeAccessor.Equals(expectedCode, actualCode));
    }
}


file static class Helpers
{
    private const BindingFlags staticMemberFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    private const BindingFlags instanceMemberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private const string ns = "System.Text.RegularExpressions";

    private static readonly Assembly Assembly = typeof(Regex).Assembly;

    private static readonly Func<string, RegexOptions, CultureInfo?, object/*RegexTree*/> _RegexParser_Parse;
    private static readonly Func<object/*RegexTree*/, object/*RegexInterpreterCode*/> _RegexWriter_Write;
    
#pragma warning disable format

    public class RegexTreeAccessor(object instance)
    {
        private static readonly FieldInfo _field_Options;
        private static readonly FieldInfo _field_Root;
        private static readonly FieldInfo _field_CaptureCount;
        private static readonly FieldInfo _field_Culture;
        private static readonly FieldInfo _field_CaptureNames;
        private static readonly FieldInfo _field_CaptureNameToNumberMapping;
        private static readonly FieldInfo _field_CaptureNumberSparseMapping;

        static RegexTreeAccessor()
        {
            var type_RegexTree = Assembly.GetType($"{ns}.RegexTree", throwOnError: true)!;
            _field_Options                    = type_RegexTree.GetField("Options"                   , instanceMemberFlags)!;
            _field_Root                       = type_RegexTree.GetField("Root"                      , instanceMemberFlags)!;
            _field_CaptureCount               = type_RegexTree.GetField("CaptureCount"              , instanceMemberFlags)!;
            _field_Culture                    = type_RegexTree.GetField("Culture"                   , instanceMemberFlags)!;
            _field_CaptureNames               = type_RegexTree.GetField("CaptureNames"              , instanceMemberFlags)!;
            _field_CaptureNameToNumberMapping = type_RegexTree.GetField("CaptureNameToNumberMapping", instanceMemberFlags)!;
            _field_CaptureNumberSparseMapping = type_RegexTree.GetField("CaptureNumberSparseMapping", instanceMemberFlags)!;
        }

        public object Instance => instance;

        public RegexOptions Options => (RegexOptions)_field_Options.GetValue(instance)!;

        public RegexNodeAccessor Root => new (_field_Root.GetValue(instance)!);

        public int CaptureCount => (int)_field_CaptureCount.GetValue(instance)!;

        public CultureInfo? Culture => (CultureInfo?)_field_Culture.GetValue(instance);

        public string[]? CaptureNames => (string[]?)_field_CaptureNames.GetValue(instance);

        public Hashtable? CaptureNameToNumberMapping => (Hashtable?)_field_CaptureNameToNumberMapping.GetValue(instance);

        public Hashtable? CaptureNumberSparseMapping => (Hashtable?)_field_CaptureNumberSparseMapping.GetValue(instance);


        public override string ToString()
        {
            var sb = new StringBuilder();
            ToString(Root, sb, "");
            return sb.ToString();
        }


        private static void ToString(RegexNodeAccessor node, StringBuilder sb, string indent)
        {
            sb.Append(indent);
            sb.Append(node.Kind);
            switch (node.Kind)
            {
            case RegexNodeKind.Set:
                sb.Append(' ');
                sb.Append(node.Str!);
                break;
            case RegexNodeKind.Multi:
                sb.Append(' ');
                sb.Append(node.Str!);
                break;
            case RegexNodeKind.One:
                sb.Append(' ');
                sb.Append(node.Ch);
                break;
            case RegexNodeKind.Notone:
                sb.Append(' ');
                sb.Append(node.Ch);
                break;
            case RegexNodeKind.Capture:
            case RegexNodeKind.Backreference:
                sb.Append(' ');
                sb.Append(node.M);
                break;
            default:
                break;
            }
            indent = indent + "  ";
            foreach (var child in node.Children)
            {
                sb.AppendLine();
                ToString(child, sb, indent);
            }
        }

        public static bool Equals(RegexTreeAccessor? accessor, RegexTree? other)
        {
            switch((accessor, other))
            {
            case (null, null):
                return true;
            case (null, _):
            case (_, null):
                return false;
            default:
                break;
            }
            if (accessor.Options != other.Options) { return false; }
            if (accessor.CaptureCount != other.CaptureCount) { return false; }
            if (accessor.Culture != other.Culture) { return false; }
            if (!Enumerable.SequenceEqual(accessor.CaptureNames ?? [], other.CaptureNames ?? [])) { return false; }
            if(!Equals(accessor.CaptureNameToNumberMapping, other.CaptureNameToNumberMapping)) { return false; }
            if(!Equals(accessor.CaptureNumberSparseMapping, other.CaptureNumberSparseMapping)) { return false; }

            return true;
        }

        private static bool Equals<TKey, TValue>(Hashtable? x, IReadOnlyDictionary<TKey, TValue>? y)
        {
            switch((x, y))
            {
            case (null, null): return true;
            case (null, _): return false;
            case (_, null): return false;
            default:
                break;
            }
            if(x.Count != y.Count) { return false; }
            foreach(var (key, value) in y)
            {
                if (!x.ContainsKey(key!)) { return false; }
                if (!Equals(x[key!], value)) { return false; }
            }
            return true;
        }
    }


    public class RegexNodeAccessor(object instance)
    {
        private static readonly FieldInfo _field_Children;
        private static readonly FieldInfo _field_Options;
        private static readonly PropertyInfo _property_Kind;
        private static readonly PropertyInfo _property_Str;
        private static readonly PropertyInfo _property_Ch;
        private static readonly PropertyInfo _property_M;
        private static readonly PropertyInfo _property_N;

        static RegexNodeAccessor()
        {
            var type_RegexNode = Assembly.GetType($"{ns}.RegexNode", throwOnError: true)!;
            _field_Children = type_RegexNode.GetField("Children", instanceMemberFlags)!;
            _field_Options  = type_RegexNode.GetField("Options" , instanceMemberFlags)!;
            _property_Kind = type_RegexNode.GetProperty("Kind", instanceMemberFlags)!;
            _property_Str  = type_RegexNode.GetProperty("Str" , instanceMemberFlags)!;
            _property_Ch   = type_RegexNode.GetProperty("Ch"  , instanceMemberFlags)!;
            _property_M    = type_RegexNode.GetProperty("M"   , instanceMemberFlags)!;
            _property_N    = type_RegexNode.GetProperty("N"   , instanceMemberFlags)!;
        }

        public object Instance => instance;

        public IEnumerable<RegexNodeAccessor> Children
        {
            get
            {
                var children = _field_Children.GetValue(instance)!;
                return children switch
                {
                    IList list => list.Cast<object>().Select(child => new RegexNodeAccessor(child)),
                    object obj => [new RegexNodeAccessor(obj)],
                    null => [],
                };
            }
        }

        public RegexOptions Options => (RegexOptions)_field_Options.GetValue(instance)!;

        public RegexNodeKind Kind => (RegexNodeKind)Convert.ToInt32(_property_Kind.GetValue(instance)!);

        public string? Str => (string?)_property_Str.GetValue(instance);

        public char Ch => (char)_property_Ch.GetValue(instance)!;

        public int M => (int)_property_M.GetValue(instance)!;

        public int N => (int)_property_N.GetValue(instance)!;

        public static bool Equals(RegexNodeAccessor? accessor, RegexNode? other)
        {
            switch ((accessor, other))
            {
            case (null, null):
                return true;
            case (null, _):
            case (_, null):
                return false;
            default:
                break;
            }
            if (accessor.Kind != other.Kind) { return false; }
            if (accessor.Str != other.Str) { return false; }
            if (accessor.Ch != other.Ch) { return false; }
            if (accessor.M != other.M) { return false; }
            if (accessor.N != other.N) { return false; }
            if (accessor.Options != other.Options) { return false; }
            var children = accessor.Children.ToArray();
            if(children.Length != other.ChildCount()) { return false; }
            for (int i = 0; i < children.Length; i++)
            {
                var accessorChild = children[i];
                var otherChild = other.Child(i);
                if (!Equals(accessorChild, otherChild)) { return false; }
            }
            return true;
        }
    }


    public class RegexInterpreterCodeAccessor(object instance)
    {
        private static readonly FieldInfo _field_Options;
        private static readonly FieldInfo _field_Codes;
        private static readonly FieldInfo _field_Strings;
        private static readonly FieldInfo _field_StringsAsciiLookup;
        private static readonly FieldInfo _field_TrackCount;
        static RegexInterpreterCodeAccessor()
        {
            var type_RegexInterpreterCode = Assembly.GetType($"{ns}.RegexInterpreterCode", throwOnError: true)!;
            _field_Options = type_RegexInterpreterCode.GetField("Options", instanceMemberFlags)!;
            _field_Codes = type_RegexInterpreterCode.GetField("Codes", instanceMemberFlags)!;
            _field_Strings = type_RegexInterpreterCode.GetField("Strings", instanceMemberFlags)!;
            _field_StringsAsciiLookup = type_RegexInterpreterCode.GetField("StringsAsciiLookup", instanceMemberFlags)!;
            _field_TrackCount = type_RegexInterpreterCode.GetField("TrackCount", instanceMemberFlags)!;
        }

        public object Instance => instance;

        public RegexOptions Options => (RegexOptions)_field_Options.GetValue(instance)!;

        public int[] Codes => (int[])_field_Codes.GetValue(instance)!;

        public string[] Strings => (string[])_field_Strings.GetValue(instance)!;

        public uint[]?[] StringsAsciiLookup => (uint[]?[])_field_StringsAsciiLookup.GetValue(instance)!;

        public int TrackCount => (int)_field_TrackCount.GetValue(instance)!;

        public static bool Equals(RegexInterpreterCodeAccessor? accessor, RegexInterpreterCode? other)
        {
            switch((accessor, other))
            {
            case (null, null):
                return true;
            case (null, _):
            case (_, null):
                return false;
            default:
                break;
            }
            if(accessor.Options != other.Options) { return false; }
            if (!Enumerable.SequenceEqual(accessor.Codes, other.Codes)) { return false; }
            if (!Enumerable.SequenceEqual(accessor.Strings, other.Strings)) { return false; }
            if(accessor.TrackCount != other.TrackCount) { return false; }
            return Enumerable.Zip(
                accessor.StringsAsciiLookup,
                other.StringsAsciiLookup,
                static (x, y) => (x, y) switch
                {
                    (null, null) => true,
                    (null, _) => false,
                    (_, null) => false,
                    _ => Enumerable.SequenceEqual(x, y),
                })
                .All(static x => x);
        }
    }

#pragma warning restore format
    
    static Helpers()
    {
        var assm = typeof(Regex).Assembly;
        var type_RegexTree = assm.GetType($"{ns}.RegexTree", throwOnError: true)!;
        var type_RegexNode = assm.GetType($"{ns}.RegexNode", throwOnError: true)!;
        var type_RegexParser = assm.GetType($"{ns}.RegexParser", throwOnError: true)!;
        var type_RegexWriter = assm.GetType($"{ns}.RegexWriter", throwOnError: true)!;
        var type_RegexInterpreterCode = assm.GetType($"{ns}.RegexInterpreterCode", throwOnError: true)!;

        var method_RegexParser_Parse = type_RegexParser
            .GetMethod("Parse", staticMemberFlags, null, [typeof(string), typeof(RegexOptions), typeof(CultureInfo)], null)!;
        {
            var arg0Expr = Expression.Parameter(typeof(string), "pattern");
            var arg1Expr = Expression.Parameter(typeof(RegexOptions), "options");
            var arg2Expr = Expression.Parameter(typeof(CultureInfo), "culture");
            var callParseExpr = Expression.Call(method_RegexParser_Parse, [arg0Expr, arg1Expr, arg2Expr]);
            var lambdaExpr = Expression.Lambda<Func<string, RegexOptions, CultureInfo?, object>>(callParseExpr, [arg0Expr, arg1Expr, arg2Expr]);
            _RegexParser_Parse = lambdaExpr.Compile();
        }

        var method_RegexWriter_Write = type_RegexWriter
            .GetMethod("Write", staticMemberFlags, null, [type_RegexTree], null)!;
        {
            var arg0Expr = Expression.Parameter(typeof(object), "typeErasuredTree");
            var typedArg0Expr = Expression.Convert(arg0Expr, type_RegexTree);
            var callWriteExpr = Expression.Call(method_RegexWriter_Write, [typedArg0Expr]);
            var lambdaExpr = Expression.Lambda<Func<object, object>>(callWriteExpr, [arg0Expr]);
            _RegexWriter_Write = lambdaExpr.Compile();
        }
    }

    public static void GetExpectedInterpreterCode(
        string pattern,
        RegexOptions options,
        CultureInfo culture,
        out RegexTreeAccessor tree,
        out RegexInterpreterCodeAccessor code)
    {
        tree = new(_RegexParser_Parse(pattern, options, culture)!);
        code = new(_RegexWriter_Write(tree.Instance)!);
    }
}
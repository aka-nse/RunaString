using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RunaString.RegularExpressions;

[RegexCodeHashAnalyzer(
    "System/Text/RegularExpressions/RegexPrefixAnalyzer.cs",
    "285012451a4187f191d0e746525e93cce3f48c17e4eb0adb541078941fad78ba")]
internal static partial class RegexPrefixAnalyzer
{
    public static string? FindFirstCharClass(RegexNode root) =>
        FindFirstOrLastCharClass(root, findFirst: true);

    public static string? FindLastCharClass(RegexNode root) =>
        FindFirstOrLastCharClass(root, findFirst: false);

    public static string? FindFirstOrLastCharClass(RegexNode root, bool findFirst)
    {
        RegexCharClass? cc = null;
        return tryFindFirstOrLastCharClass(root, findFirst, ref cc) == true
            ? cc!.ToStringClass()
            : null;

        static bool? tryFindFirstOrLastCharClass(RegexNode node, bool findFirst, ref RegexCharClass? cc)
        {
            if (!StackHelper.TryEnsureSufficientExecutionStack())
            {
                return false;
            }

            switch (node.Kind)
            {
            case RegexNodeKind.One:
            case RegexNodeKind.Oneloop:
            case RegexNodeKind.Onelazy:
            case RegexNodeKind.Oneloopatomic:
                if (cc is null || cc.CanMerge)
                {
                    cc ??= new RegexCharClass();
                    cc.AddChar(node.Ch);
                    return node.Kind is RegexNodeKind.One || node.M > 0 ? true : null;
                }
                return false;

            case RegexNodeKind.Notone:
            case RegexNodeKind.Notoneloop:
            case RegexNodeKind.Notoneloopatomic:
            case RegexNodeKind.Notonelazy:
                if (cc is null || cc.CanMerge)
                {
                    cc ??= new RegexCharClass();
                    if (node.Ch > 0)
                    {
                        cc.AddRange((char)0, (char)(node.Ch - 1));
                    }
                    if (node.Ch < char.MaxValue)
                    {
                        cc.AddRange((char)(node.Ch + 1), char.MaxValue);
                    }
                    return node.Kind is RegexNodeKind.Notone || node.M > 0 ? true : null;
                }
                return false;

            case RegexNodeKind.Set:
            case RegexNodeKind.Setloop:
            case RegexNodeKind.Setlazy:
            case RegexNodeKind.Setloopatomic:
                {
                    bool setSuccess = false;
                    if (cc is null)
                    {
                        cc = RegexCharClass.Parse(node.Str!);
                        setSuccess = true;
                    }
                    else if (cc.CanMerge && RegexCharClass.Parse(node.Str!) is { CanMerge: true } setCc)
                    {
                        cc.AddCharClass(setCc);
                        setSuccess = true;
                    }
                    return
                        !setSuccess ? false :
                        node.Kind is RegexNodeKind.Set || node.M > 0 ? true :
                        null;
                }

            case RegexNodeKind.Multi:
                if (cc is null || cc.CanMerge)
                {
                    cc ??= new RegexCharClass();
                    bool firstChar = findFirst == ((node.Options & RegexOptions.RightToLeft) == 0);
                    cc.AddChar(node.Str![firstChar ? 0 : node.Str.Length - 1]);
                    return true;
                }
                return false;

            case RegexNodeKind.Empty:
            case RegexNodeKind.Nothing:
            case RegexNodeKind.Bol:
            case RegexNodeKind.Eol:
            case RegexNodeKind.Boundary:
            case RegexNodeKind.NonBoundary:
            case RegexNodeKind.ECMABoundary:
            case RegexNodeKind.NonECMABoundary:
            case RegexNodeKind.Beginning:
            case RegexNodeKind.Start:
            case RegexNodeKind.EndZ:
            case RegexNodeKind.End:
            case RegexNodeKind.UpdateBumpalong:
            case RegexNodeKind.PositiveLookaround:
            case RegexNodeKind.NegativeLookaround:
                return null;

            case RegexNodeKind.Atomic:
            case RegexNodeKind.Capture:
                return tryFindFirstOrLastCharClass(node.Child(0), findFirst, ref cc);

            case RegexNodeKind.Loop:
            case RegexNodeKind.Lazyloop:
                return tryFindFirstOrLastCharClass(node.Child(0), findFirst, ref cc) switch
                {
                    false => false,
                    null => null,
                    _ => node.M == 0 ? null : true,
                };

            case RegexNodeKind.Concatenate:
                {
                    int childCount = node.ChildCount();
                    if (findFirst)
                    {
                        for (int i = 0; i < childCount; i++)
                        {
                            bool? childResult = tryFindFirstOrLastCharClass(node.Child(i), findFirst, ref cc);
                            if (childResult != null)
                            {
                                return childResult;
                            }
                        }
                    }
                    else
                    {
                        for (int i = childCount - 1; i >= 0; i--)
                        {
                            bool? childResult = tryFindFirstOrLastCharClass(node.Child(i), findFirst, ref cc);
                            if (childResult != null)
                            {
                                return childResult;
                            }
                        }
                    }
                    return null;
                }

            case RegexNodeKind.Alternate:
                {
                    int childCount = node.ChildCount();
                    bool anyChildWasNull = false;
                    for (int i = 0; i < childCount; i++)
                    {
                        bool? childResult = tryFindFirstOrLastCharClass(node.Child(i), findFirst, ref cc);
                        if (childResult is null)
                        {
                            anyChildWasNull = true;
                        }
                        else if (childResult == false)
                        {
                            return false;
                        }
                    }
                    return anyChildWasNull ? null : true;
                }

            case RegexNodeKind.BackreferenceConditional:
            case RegexNodeKind.ExpressionConditional:
                int branchStart = node.Kind is RegexNodeKind.BackreferenceConditional ? 0 : 1;
                return (tryFindFirstOrLastCharClass(node.Child(branchStart), findFirst, ref cc), tryFindFirstOrLastCharClass(node.Child(branchStart + 1), findFirst, ref cc)) switch
                {
                    (false, _) or (_, false) => false,
                    (null, _) or (_, null) => null,
                    _ => true,
                };

            case RegexNodeKind.Backreference:
                return false;
            }

            Debug.Fail($"Unexpected node {node.Kind}");
            return false;
        }
    }
}

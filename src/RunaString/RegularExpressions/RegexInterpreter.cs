using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RunaString.RegularExpressions;

internal class RegexInterpreterFactory(RegexTree tree)
    : IRegexRunnerFactory
{
    private readonly RegexInterpreterCode _code = RegexWriter.Write(tree);
    private readonly CultureInfo? _culture = tree.Culture;

    public RegexRunner<TString, TIndex> CreateInstance<TString, TIndex>()
        where TString : IRunaString<TString, TIndex>, allows ref struct
        where TIndex : unmanaged, IRunaIndex<TIndex> =>
        new RegexInterpreter<TString, TIndex>(_code, _culture);
}


[RegexCodeHashAnalyzer(
    "System/Text/RegularExpressions/RegexInterpreter.cs",
    "" // "c4ffa23812cd3b24b2a86edfb2a936f17d6227420c086b381cfb446a2b0531a3"
    )]
[RegexCodeHashAnalyzer(
    "System/Text/RegularExpressions/RegexRunner.cs",
    "" // "4c847e7482d3dc6f0158ba6671bc0aef9e872261b0785e2277c4520bb004a00d"
    )]
internal partial class RegexInterpreter<TString, TIndex>(RegexInterpreterCode code, CultureInfo? culture)
    : RegexRunner<TString, TIndex>
    where TString : IRunaString<TString, TIndex>, allows ref struct
    where TIndex : unmanaged, IRunaIndex<TIndex>
{
    private RunaRegex? runregex;
    private readonly RegexInterpreterCode _code = code;
    private readonly CultureInfo? _culture = culture;
    private RegexOpcode _operator;
    private int _codepos;
    private bool _rightToLeft;
    private TIndex runtextstart;
    private TIndex runtextend;
    private TIndex runtextbeg;
    private TIndex runtextpos;
    private int[]? runtrack;
    private int[]? runstack;
    private int[]? runcrawl;
    private int runtrackpos;
    private int runstackpos;
    private int runcrawlpos;
    private int runtrackcount;
    private bool _checkTimeout;
    private int _timeout; // [msec]
    private long _timeoutOccursAt;


    public override void Scan(TString text)
    {
        Debug.Assert(runregex is not null);
        Debug.Assert(runtrack is not null);
        Debug.Assert(runstack is not null);
        Debug.Assert(runcrawl is not null);

        if (runregex.RightToLeft)
        {
            while (text.IsInRange(runtextpos))
            {
                CheckTimeout();

                if (TryMatchAtCurrentPosition(text))
                {
                    // The match was successful, so we can exit the loop and return.
                    return;
                }

                // Reset state for another iteration.
                runtrackpos = runtrack.Length;
                runstackpos = runstack.Length;
                runcrawlpos = runcrawl.Length;
                text.Decrement(ref runtextpos);
            }
        }
        else
        {
            while (text.IsInRange(runtextpos))
            {
                CheckTimeout();

                if (TryMatchAtCurrentPosition(text))
                {
                    // The match was successful, so we can exit the loop and return.
                    return;
                }

                // Reset state for another iteration.
                runtrackpos = runtrack.Length;
                runstackpos = runstack.Length;
                runcrawlpos = runcrawl.Length;
                text.Increment(ref runtextpos);
            }
        }
    }


    private bool TryMatchAtCurrentPosition(TString inputSpan)
    {
        throw new NotImplementedException();
        /*
        SetOperator((RegexOpcode)_code.Codes[0]);
        _codepos = 0;
        int advance = -1;

        while (true)
        {
            if (advance >= 0)
            {
                // Single common Advance call to reduce method size; and single method inline point.
                // Details at https://github.com/dotnet/corefx/pull/25096.
                Advance(advance);
                advance = -1;
            }

            switch (_operator)
            {
            case RegexOpcode.Stop:
                return runmatch!.FoundMatch;

            case RegexOpcode.Nothing:
                break;

            case RegexOpcode.Goto:
                Goto(Operand(0));
                continue;

            case RegexOpcode.TestBackreference:
                if (!IsMatched(Operand(0)))
                {
                    break;
                }
                advance = 1;
                continue;

            case RegexOpcode.Lazybranch:
                TrackPush(runtextpos);
                advance = 1;
                continue;

            case RegexOpcode.Lazybranch | RegexOpcode.Backtracking:
                TrackPop();
                runtextpos = TrackPeek();
                Goto(Operand(0));
                continue;

            case RegexOpcode.Setmark:
                StackPush(runtextpos);
                TrackPush();
                advance = 0;
                continue;

            case RegexOpcode.Nullmark:
                StackPush(-1);
                TrackPush();
                advance = 0;
                continue;

            case RegexOpcode.Setmark | RegexOpcode.Backtracking:
            case RegexOpcode.Nullmark | RegexOpcode.Backtracking:
                StackPop();
                break;

            case RegexOpcode.Getmark:
                StackPop();
                TrackPush(StackPeek());
                runtextpos = StackPeek();
                advance = 0;
                continue;

            case RegexOpcode.Getmark | RegexOpcode.Backtracking:
                TrackPop();
                StackPush(TrackPeek());
                break;

            case RegexOpcode.Capturemark:
                if (Operand(1) != -1 && !IsMatched(Operand(1)))
                {
                    break;
                }
                StackPop();
                if (Operand(1) != -1)
                {
                    TransferCapture(Operand(0), Operand(1), StackPeek(), runtextpos);
                }
                else
                {
                    Capture(Operand(0), StackPeek(), runtextpos);
                }
                TrackPush(StackPeek());
                advance = 2;
                continue;

            case RegexOpcode.Capturemark | RegexOpcode.Backtracking:
                TrackPop();
                StackPush(TrackPeek());
                Uncapture();
                if (Operand(0) != -1 && Operand(1) != -1)
                {
                    Uncapture();
                }
                break;

            case RegexOpcode.Branchmark:
                StackPop();
                if (runtextpos != StackPeek())
                {
                    // Nonempty match -> loop now
                    TrackPush(StackPeek(), runtextpos); // Save old mark, textpos
                    StackPush(runtextpos);              // Make new mark
                    Goto(Operand(0));                   // Loop
                }
                else
                {
                    // Empty match -> straight now
                    TrackPush2(StackPeek());            // Save old mark
                    advance = 1;                        // Straight
                }
                continue;

            case RegexOpcode.Branchmark | RegexOpcode.Backtracking:
                TrackPop(2);
                StackPop();
                runtextpos = TrackPeek(1); // Recall position
                TrackPush2(TrackPeek());   // Save old mark
                advance = 1;               // Straight
                continue;

            case RegexOpcode.Branchmark | RegexOpcode.BacktrackingSecond:
                TrackPop();
                StackPush(TrackPeek()); // Recall old mark
                break;                  // Backtrack

            case RegexOpcode.Lazybranchmark:
                // We hit this the first time through a lazy loop and after each
                // successful match of the inner expression.  It simply continues
                // on and doesn't loop.
                StackPop();
                {
                    int oldMarkPos = StackPeek();
                    if (runtextpos != oldMarkPos)
                    {
                        // Nonempty match -> try to loop again by going to 'back' state
                        if (oldMarkPos != -1)
                        {
                            TrackPush(oldMarkPos, runtextpos); // Save old mark, textpos
                        }
                        else
                        {
                            TrackPush(runtextpos, runtextpos);
                        }
                    }
                    else
                    {
                        // The inner expression found an empty match. We'll go directly to BacktrackingSecond
                        // if we backtrack. We do not touch the grouping stack here... instead, we record the
                        // old mark and a "no-stack-pop" flag (0) on the backtracking stack.
                        TrackPush2(oldMarkPos, 0);
                    }
                }
                advance = 1;
                continue;

            case RegexOpcode.Lazybranchmark | RegexOpcode.Backtracking:
                {
                    // After the first time, Lazybranchmark | RegexOpcode.Back occurs
                    // with each iteration of the loop, and therefore with every attempted
                    // match of the inner expression.  We'll try to match the inner expression,
                    // then go back to Lazybranchmark if successful.  If the inner expression
                    // fails, we go to Lazybranchmark | RegexOpcode.Back2
                    TrackPop(2);
                    int pos = TrackPeek(1);

                    // Store the previous mark. The second value (1) flags that a grouping stack frame
                    // must be popped when backtracking, because a new frame will be pushed next.
                    TrackPush2(TrackPeek(), 1);

                    StackPush(pos);          // Make new mark
                    runtextpos = pos;        // Recall position
                    Goto(Operand(0));        // Loop
                }
                continue;

            case RegexOpcode.Lazybranchmark | RegexOpcode.BacktrackingSecond:
                // The lazy loop has failed.  We'll do a true backtrack and
                // start over before the lazy loop.
                int needsPop = runtrack![runtrackpos]; // flag: 0 or 1
                int oldMark = runtrack[runtrackpos + 1]; // saved old mark
                runtrackpos += 2; // consume both payload ints
                if (needsPop != 0)
                {
                    // We pushed on the grouping stack in the Backtracking arm; balance it now.
                    StackPop();
                }

                // Restore the old mark and backtrack
                StackPush(oldMark);
                break;

            case RegexOpcode.Setcount:
                StackPush(runtextpos, Operand(0));
                TrackPush();
                advance = 1;
                continue;

            case RegexOpcode.Nullcount:
                StackPush(-1, Operand(0));
                TrackPush();
                advance = 1;
                continue;

            case RegexOpcode.Setcount | RegexOpcode.Backtracking:
            case RegexOpcode.Nullcount | RegexOpcode.Backtracking:
            case RegexOpcode.Setjump | RegexOpcode.Backtracking:
                StackPop(2);
                break;

            case RegexOpcode.Branchcount:
                // StackPush:
                //  0: Mark
                //  1: Count
                StackPop(2);
                {
                    int mark = StackPeek();
                    int count = StackPeek(1);
                    int matched = runtextpos - mark;
                    if (count >= Operand(1) || (matched == 0 && count >= 0))
                    {
                        // Max loops or empty match -> straight now
                        TrackPush2(mark, count); // Save old mark, count
                        advance = 2;             // Straight
                    }
                    else
                    {
                        // Nonempty match -> count+loop now
                        TrackPush(mark);                  // remember mark
                        StackPush(runtextpos, count + 1); // Make new mark, incr count
                        Goto(Operand(0));                 // Loop
                    }
                }
                continue;

            case RegexOpcode.Branchcount | RegexOpcode.Backtracking:
                // TrackPush:
                //  0: Previous mark
                // StackPush:
                //  0: Mark (= current pos, discarded)
                //  1: Count
                TrackPop();
                StackPop(2);
                if (StackPeek(1) > 0)
                {
                    // Positive -> can go straight
                    runtextpos = StackPeek();                  // Zap to mark
                    TrackPush2(TrackPeek(), StackPeek(1) - 1); // Save old mark, old count
                    advance = 2;                               // Straight
                    continue;
                }
                StackPush(TrackPeek(), StackPeek(1) - 1);      // Recall old mark, old count
                break;

            case RegexOpcode.Branchcount | RegexOpcode.BacktrackingSecond:
                // TrackPush:
                //  0: Previous mark
                //  1: Previous count
                TrackPop(2);
                StackPush(TrackPeek(), TrackPeek(1)); // Recall old mark, old count
                break;                                // Backtrack

            case RegexOpcode.Lazybranchcount:
                // StackPush:
                //  0: Mark
                //  1: Count
                StackPop(2);
                {
                    int mark = StackPeek();
                    int count = StackPeek(1);
                    if (count < 0)
                    {
                        // Negative count -> loop now
                        TrackPush2(mark);                 // Save old mark
                        StackPush(runtextpos, count + 1); // Make new mark, incr count
                        Goto(Operand(0));                 // Loop
                    }
                    else
                    {
                        // Nonneg count -> straight now
                        TrackPush(mark, count, runtextpos); // Save mark, count, position
                        advance = 2;                        // Straight
                    }
                }
                continue;

            case RegexOpcode.Lazybranchcount | RegexOpcode.Backtracking:
                // TrackPush:
                //  0: Mark
                //  1: Count
                //  2: Textpos
                TrackPop(3);
                {
                    int mark = TrackPeek();
                    int textpos = TrackPeek(2);
                    if (TrackPeek(1) < Operand(1) && textpos != mark)
                    {
                        // Under limit and not empty match -> loop
                        runtextpos = textpos;                 // Recall position
                        StackPush(textpos, TrackPeek(1) + 1); // Make new mark, incr count
                        TrackPush2(mark);                     // Save old mark
                        Goto(Operand(0));                     // Loop
                        continue;
                    }
                    else
                    {
                        // Max loops or empty match -> backtrack
                        StackPush(TrackPeek(), TrackPeek(1)); // Recall old mark, count
                        break;                                // backtrack
                    }
                }

            case RegexOpcode.Lazybranchcount | RegexOpcode.BacktrackingSecond:
                // TrackPush:
                //  0: Previous mark
                // StackPush:
                //  0: Mark (== current pos, discarded)
                //  1: Count
                TrackPop();
                StackPop(2);
                StackPush(TrackPeek(), StackPeek(1) - 1); // Recall old mark, count
                break;                                    // Backtrack

            case RegexOpcode.Setjump:
                CheckTimeout(); // to ensure that positive/negative lookarounds have a timeout check
                StackPush(Trackpos(), Crawlpos());
                TrackPush();
                advance = 0;
                continue;

            case RegexOpcode.Backjump:
                // StackPush:
                //  0: Saved trackpos
                //  1: Crawlpos
                StackPop(2);
                Trackto(StackPeek());
                while (Crawlpos() != StackPeek(1))
                {
                    Uncapture();
                }
                break;

            case RegexOpcode.Forejump:
                // StackPush:
                //  0: Saved trackpos
                //  1: Crawlpos
                StackPop(2);
                Trackto(StackPeek());
                TrackPush(StackPeek(1));
                advance = 0;
                continue;

            case RegexOpcode.Forejump | RegexOpcode.Backtracking:
                // TrackPush:
                //  0: Crawlpos
                TrackPop();
                while (Crawlpos() != TrackPeek())
                {
                    Uncapture();
                }
                break;

            case RegexOpcode.Bol:
                {
                    int m1 = runtextpos - 1;
                    if ((uint)m1 < (uint)inputSpan.Length && inputSpan[m1] != '\n')
                    {
                        break;
                    }
                    advance = 0;
                    continue;
                }

            case RegexOpcode.Eol:
                {
                    int runtextpos = this.runtextpos;
                    if ((uint)runtextpos < (uint)inputSpan.Length && inputSpan[runtextpos] != '\n')
                    {
                        break;
                    }
                    advance = 0;
                    continue;
                }

            case RegexOpcode.Boundary:
                if (!IsBoundary(inputSpan, runtextpos))
                {
                    break;
                }
                advance = 0;
                continue;

            case RegexOpcode.NonBoundary:
                if (IsBoundary(inputSpan, runtextpos))
                {
                    break;
                }
                advance = 0;
                continue;

            case RegexOpcode.ECMABoundary:
                if (!IsECMABoundary(inputSpan, runtextpos))
                {
                    break;
                }
                advance = 0;
                continue;

            case RegexOpcode.NonECMABoundary:
                if (IsECMABoundary(inputSpan, runtextpos))
                {
                    break;
                }
                advance = 0;
                continue;

            case RegexOpcode.Beginning:
                if (runtextpos > 0)
                {
                    break;
                }
                advance = 0;
                continue;

            case RegexOpcode.Start:
                if (runtextpos != runtextstart)
                {
                    break;
                }
                advance = 0;
                continue;

            case RegexOpcode.EndZ:
                {
                    int runtextpos = this.runtextpos;
                    if (runtextpos < inputSpan.Length - 1 || ((uint)runtextpos < (uint)inputSpan.Length && inputSpan[runtextpos] != '\n'))
                    {
                        break;
                    }
                    advance = 0;
                    continue;
                }

            case RegexOpcode.End:
                if (runtextpos < inputSpan.Length)
                {
                    break;
                }
                advance = 0;
                continue;

            case RegexOpcode.One:
                if (Forwardchars() < 1 || Forwardcharnext(inputSpan) != (char)Operand(0))
                {
                    break;
                }
                advance = 1;
                continue;

            case RegexOpcode.Notone:
                if (Forwardchars() < 1 || Forwardcharnext(inputSpan) == (char)Operand(0))
                {
                    break;
                }
                advance = 1;
                continue;

            case RegexOpcode.Set:
                if (Forwardchars() < 1)
                {
                    break;
                }
                else
                {
                    int operand = Operand(0);
                    if (!RegexCharClass.CharInClass(Forwardcharnext(inputSpan), _code.Strings[operand], ref _code.StringsAsciiLookup[operand]))
                    {
                        break;
                    }
                }
                advance = 1;
                continue;

            case RegexOpcode.Multi:
                if (!MatchString(_code.Strings[Operand(0)], inputSpan))
                {
                    break;
                }
                advance = 1;
                continue;

            case RegexOpcode.Backreference:
            case RegexOpcode.Backreference | RegexOpcode.CaseInsensitive:
                {
                    int capnum = Operand(0);
                    if (IsMatched(capnum))
                    {
                        if (!MatchRef(MatchIndex(capnum), MatchLength(capnum), inputSpan, (_operator & RegexOpcode.CaseInsensitive) != 0))
                        {
                            break;
                        }
                    }
                    else
                    {
                        if ((runregex!.roptions & RegexOptions.ECMAScript) == 0)
                        {
                            break;
                        }
                    }
                }
                advance = 1;
                continue;

            case RegexOpcode.Onerep:
                {
                    int c = Operand(1);
                    if (Forwardchars() < c)
                    {
                        break;
                    }

                    char ch = (char)Operand(0);
                    while (c-- > 0)
                    {
                        if (Forwardcharnext(inputSpan) != ch)
                        {
                            goto BreakBackward;
                        }
                    }
                }
                advance = 2;
                continue;

            case RegexOpcode.Notonerep:
                {
                    int c = Operand(1);
                    if (Forwardchars() < c)
                    {
                        break;
                    }

                    char ch = (char)Operand(0);
                    while (c-- > 0)
                    {
                        if (Forwardcharnext(inputSpan) == ch)
                        {
                            goto BreakBackward;
                        }
                    }
                }
                advance = 2;
                continue;

            case RegexOpcode.Setrep:
                {
                    int c = Operand(1);
                    if (Forwardchars() < c)
                    {
                        break;
                    }

                    int operand0 = Operand(0);
                    string set = _code.Strings[operand0];
                    ref uint[]? setLookup = ref _code.StringsAsciiLookup[operand0];

                    while (c-- > 0)
                    {
                        if (!RegexCharClass.CharInClass(Forwardcharnext(inputSpan), set, ref setLookup))
                        {
                            goto BreakBackward;
                        }
                    }
                }
                advance = 2;
                continue;

            case RegexOpcode.Oneloop:
            case RegexOpcode.Oneloopatomic:
                {
                    int len = Math.Min(Operand(1), Forwardchars());
                    char ch = (char)Operand(0);
                    int i;

                    for (i = len; i > 0; i--)
                    {
                        if (Forwardcharnext(inputSpan) != ch)
                        {
                            Backwardnext();
                            break;
                        }
                    }

                    if (len > i && _operator == RegexOpcode.Oneloop)
                    {
                        TrackPush(len - i - 1, runtextpos - Bump());
                    }
                }
                advance = 2;
                continue;

            case RegexOpcode.Notoneloop:
            case RegexOpcode.Notoneloopatomic:
                {
                    int len = Math.Min(Operand(1), Forwardchars());
                    char ch = (char)Operand(0);
                    int i;

                    if (!_rightToLeft)
                    {
                        // We're left-to-right, so we can employ the vectorized IndexOf
                        // to search for the character.
                        i = inputSpan.Slice(runtextpos, len).IndexOf(ch);
                        if (i == -1)
                        {
                            runtextpos += len;
                            i = 0;
                        }
                        else
                        {
                            runtextpos += i;
                            i = len - i;
                        }
                    }
                    else
                    {
                        for (i = len; i > 0; i--)
                        {
                            if (Forwardcharnext(inputSpan) == ch)
                            {
                                Backwardnext();
                                break;
                            }
                        }
                    }

                    if (len > i && _operator == RegexOpcode.Notoneloop)
                    {
                        TrackPush(len - i - 1, runtextpos - Bump());
                    }
                }
                advance = 2;
                continue;

            case RegexOpcode.Setloop:
            case RegexOpcode.Setloopatomic:
                {
                    int len = Math.Min(Operand(1), Forwardchars());
                    int operand0 = Operand(0);
                    string set = _code.Strings[operand0];
                    ref uint[]? setLookup = ref _code.StringsAsciiLookup[operand0];
                    int i;

                    for (i = len; i > 0; i--)
                    {
                        if (!RegexCharClass.CharInClass(Forwardcharnext(inputSpan), set, ref setLookup))
                        {
                            Backwardnext();
                            break;
                        }
                    }

                    if (len > i && _operator == RegexOpcode.Setloop)
                    {
                        TrackPush(len - i - 1, runtextpos - Bump());
                    }
                }
                advance = 2;
                continue;

            case RegexOpcode.Oneloop | RegexOpcode.Backtracking:
            case RegexOpcode.Notoneloop | RegexOpcode.Backtracking:
            case RegexOpcode.Setloop | RegexOpcode.Backtracking:
                TrackPop(2);
                {
                    int i = TrackPeek();
                    int pos = TrackPeek(1);
                    runtextpos = pos;
                    if (i > 0)
                    {
                        TrackPush(i - 1, pos - Bump());
                    }
                }
                advance = 2;
                continue;

            case RegexOpcode.Onelazy:
            case RegexOpcode.Notonelazy:
            case RegexOpcode.Setlazy:
                {
                    int c = Math.Min(Operand(1), Forwardchars());
                    if (c > 0)
                    {
                        TrackPush(c - 1, runtextpos);
                    }
                }
                advance = 2;
                continue;

            case RegexOpcode.Onelazy | RegexOpcode.Backtracking:
                TrackPop(2);
                {
                    int pos = TrackPeek(1);
                    runtextpos = pos;

                    if (Forwardcharnext(inputSpan) != (char)Operand(0))
                    {
                        break;
                    }

                    int i = TrackPeek();
                    if (i > 0)
                    {
                        TrackPush(i - 1, pos + Bump());
                    }
                }
                advance = 2;
                continue;

            case RegexOpcode.Notonelazy | RegexOpcode.Backtracking:
                TrackPop(2);
                {
                    int pos = TrackPeek(1);
                    runtextpos = pos;

                    if (Forwardcharnext(inputSpan) == (char)Operand(0))
                    {
                        break;
                    }

                    int i = TrackPeek();
                    if (i > 0)
                    {
                        TrackPush(i - 1, pos + Bump());
                    }
                }
                advance = 2;
                continue;

            case RegexOpcode.Setlazy | RegexOpcode.Backtracking:
                TrackPop(2);
                {
                    int pos = TrackPeek(1);
                    runtextpos = pos;

                    int operand0 = Operand(0);
                    if (!RegexCharClass.CharInClass(Forwardcharnext(inputSpan), _code.Strings[operand0], ref _code.StringsAsciiLookup[operand0]))
                    {
                        break;
                    }

                    int i = TrackPeek();
                    if (i > 0)
                    {
                        TrackPush(i - 1, pos + Bump());
                    }
                }
                advance = 2;
                continue;

            case RegexOpcode.UpdateBumpalong:
                // UpdateBumpalong should only exist in the code stream at such a point where the root
                // of the backtracking stack contains the runtextpos from the start of this Go call. Replace
                // that tracking value with the current runtextpos value if it's greater.
                {
                    Debug.Assert(!_rightToLeft, "UpdateBumpalongs aren't added for RTL");
                    ref int trackingpos = ref runtrack![runtrack.Length - 1];
                    if (trackingpos < runtextpos)
                    {
                        trackingpos = runtextpos;
                    }
                    advance = 0;
                    continue;
                }

            default:
                Debug.Fail($"Unimplemented state: {_operator:X8}");
                break;
            }

        BreakBackward:
            Backtrack();
        }
        */
    }
}

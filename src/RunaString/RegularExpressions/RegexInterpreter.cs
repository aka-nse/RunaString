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
        where TIndex : struct, IRunaIndex<TIndex> =>
        new RegexInterpreter<TString, TIndex>(_code, _culture);
}


internal partial class RegexInterpreter<TString, TIndex>(RegexInterpreterCode code, CultureInfo? culture)
    : RegexRunner<TString, TIndex>
    where TString : IRunaString<TString, TIndex>, allows ref struct
    where TIndex : struct, IRunaIndex<TIndex>
{
    private RunaRegex? runregex;
    private TIndex runtextstart;
    private TIndex runtextpos;
    private int[]? runtrack;
    private int[]? runstack;
    private int[]? runcrawl;
    private int runtrackpos;
    private int runstackpos;
    private int runcrawlpos;

    private bool _checkTimeout;
    private int _timeout; // [msec]
    private long _timeoutOccursAt;


    internal override void Scan(TString text)
    {
        Debug.Assert(runregex is not null);
        Debug.Assert(runtrack is not null);
        Debug.Assert(runstack is not null);
        Debug.Assert(runcrawl is not null);

        if (runregex.RightToLeft)
        {
            while (code.FindOptimizations.TryFindNextStartingPositionRightToLeft(text, ref runtextpos, runtextstart))
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
                if (!text.IsInRange(runtextpos))
                {
                    // Iteration has reached the end of the string, no more positions to check.
                    return;
                }
            }
        }
        else
        {
            while (code.FindOptimizations.TryFindNextStartingPositionLeftToRight(text, ref runtextpos, runtextstart))
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
                if(!text.IsInRange(runtextpos))
                {
                    // Iteration has reached the end of the string, no more positions to check.
                    return;
                }
            }
        }
    }


    private bool TryMatchAtCurrentPosition(TString inputSpan)
    {
        throw new NotImplementedException();
    }


    private void CheckTimeout()
    {
        if (_checkTimeout && Environment.TickCount64 >= _timeoutOccursAt)
        {
            throwRegexTimeout();
        }

        void throwRegexTimeout() => throw new RegexMatchTimeoutException(
            string.Empty,
            runregex!.Pattern,
            TimeSpan.FromMilliseconds(_timeout));
    }
}

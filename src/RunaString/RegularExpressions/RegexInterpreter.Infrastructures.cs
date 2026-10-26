using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace RunaString.RegularExpressions;

partial class RegexInterpreter<TString, TIndex>
{
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

    private void EnsureStorage()
    {
        int limit = runtrackcount * 4;

        if (runstackpos < limit)
            DoubleStack();

        if (runtrackpos < limit)
            DoubleTrack();
    }

    private void DoubleStack()
    {
        int[] newstack = new int[runstack!.Length * 2];

        Array.Copy(runstack, 0, newstack, runstack.Length, runstack.Length);
        runstackpos += runstack.Length;
        runstack = newstack;
    }

    private void DoubleTrack()
    {
        int[] newtrack = new int[runtrack!.Length * 2];

        Array.Copy(runtrack, 0, newtrack, runtrack.Length, runtrack.Length);
        runtrackpos += runtrack.Length;
        runtrack = newtrack;
    }

    private void InitTrackCount() => runtrackcount = _code.TrackCount;

    private void Advance(int i)
    {
        _codepos += i + 1;
        SetOperator((RegexOpcode)_code.Codes[_codepos]);
    }

    private void Goto(int newpos)
    {
        EnsureStorage();

        _codepos = newpos;
        SetOperator((RegexOpcode)_code.Codes[newpos]);
    }

    private void Trackto(int newpos) => runtrackpos = runtrack!.Length - newpos;

    private int Trackpos() => runtrack!.Length - runtrackpos;

    private void TrackPush() => runtrack![--runtrackpos] = _codepos;

    private void TrackPush(int i1)
    {
        int[] localruntrack = runtrack!;
        int localruntrackpos = runtrackpos;

        localruntrack[--localruntrackpos] = i1;
        localruntrack[--localruntrackpos] = _codepos;

        runtrackpos = localruntrackpos;
    }

    private void TrackPush(int i1, int i2)
    {
        int[] localruntrack = runtrack!;
        int localruntrackpos = runtrackpos;

        localruntrack[--localruntrackpos] = i1;
        localruntrack[--localruntrackpos] = i2;
        localruntrack[--localruntrackpos] = _codepos;

        runtrackpos = localruntrackpos;
    }

    private void TrackPush(int i1, int i2, int i3)
    {
        int[] localruntrack = runtrack!;
        int localruntrackpos = runtrackpos;

        localruntrack[--localruntrackpos] = i1;
        localruntrack[--localruntrackpos] = i2;
        localruntrack[--localruntrackpos] = i3;
        localruntrack[--localruntrackpos] = _codepos;

        runtrackpos = localruntrackpos;
    }

    private void TrackPush2(int i1)
    {
        int[] localruntrack = runtrack!;
        int localruntrackpos = runtrackpos;

        localruntrack[--localruntrackpos] = i1;
        localruntrack[--localruntrackpos] = -_codepos;

        runtrackpos = localruntrackpos;
    }

    private void TrackPush2(int i1, int i2)
    {
        int[] localruntrack = runtrack!;
        int localruntrackpos = runtrackpos;

        localruntrack[--localruntrackpos] = i1;
        localruntrack[--localruntrackpos] = i2;
        localruntrack[--localruntrackpos] = -_codepos;

        runtrackpos = localruntrackpos;
    }

    private void Backtrack()
    {
        CheckTimeout();

        int newpos = runtrack![runtrackpos];
        runtrackpos++;

        int back = (int)RegexOpcode.Backtracking;
        if (newpos < 0)
        {
            newpos = -newpos;
            back = (int)RegexOpcode.BacktrackingSecond;
        }
        SetOperator((RegexOpcode)(_code.Codes[newpos] | back));

        EnsureStorage();

        _codepos = newpos;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetOperator(RegexOpcode op)
    {
        _operator = op & ~RegexOpcode.RightToLeft;
        _rightToLeft = (op & RegexOpcode.RightToLeft) != 0;
    }

    private void TrackPop() => runtrackpos++;

    private void TrackPop(int framesize) => runtrackpos += framesize;

    private int TrackPeek() => runtrack![runtrackpos - 1];

    private int TrackPeek(int i) => runtrack![runtrackpos - i - 1];

    private void StackPush(int i1) => runstack![--runstackpos] = i1;

    private void StackPush(int i1, int i2)
    {
        int[] localrunstack = runstack!;
        int localrunstackpos = runstackpos;

        localrunstack[--localrunstackpos] = i1;
        localrunstack[--localrunstackpos] = i2;

        runstackpos = localrunstackpos;
    }

    private void StackPop() => runstackpos++;

    private void StackPop(int framesize) => runstackpos += framesize;

    private int StackPeek() => runstack![runstackpos - 1];

    private int StackPeek(int i) => runstack![runstackpos - i - 1];

    private int Operand(int i) => _code.Codes[_codepos + i + 1];

    private int Bump() => _rightToLeft ? -1 : 1;

    private TIndex Forwardchars(TString text) =>
     _rightToLeft
      ? runtextpos
      : text.Decrement(runtextend, runtextpos.RuneIndex);

    private Rune Forwardcharnext(TString inputSpan)
    {
        var i = _rightToLeft
            ? inputSpan.Decrement(ref runtextpos)
            : inputSpan.Increment(ref runtextpos);
        return inputSpan[i];
    }

}


file static class Helpers
{
    extension<TString, TIndex>(TString text)
        where TString : IRunaString<TString, TIndex>, allows ref struct
        where TIndex : unmanaged, IRunaIndex<TIndex>
    {
        public TIndex Increment(TIndex index, int count)
        {
            for (int i = 0; i < count; i++)
            {
                text.Increment(ref index);
            }
            return index;
        }


        public TIndex Decrement(TIndex index, int count)
        {
            for (int i = 0; i < count; i++)
            {
                text.Decrement(ref index);
            }
            return index;
        }
    }
}
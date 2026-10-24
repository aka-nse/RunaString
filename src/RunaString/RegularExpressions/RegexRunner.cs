namespace RunaString.RegularExpressions;

internal interface IRegexRunnerFactory
{
    public RegexRunner<TString, TIndex> CreateInstance<TString, TIndex>()
        where TString : IRunaString<TString, TIndex>, allows ref struct
        where TIndex : struct,IRunaIndex<TIndex>;
}

[RegexCodeHashAnalyzer(
    "System/Text/RegularExpressions/RegexRunner.cs",
    "" // "4c847e7482d3dc6f0158ba6671bc0aef9e872261b0785e2277c4520bb004a00d"
    )]
internal abstract class RegexRunner<TString, TIndex>
        where TString : IRunaString<TString, TIndex>, allows ref struct
        where TIndex : struct, IRunaIndex<TIndex>
{
    internal abstract void Scan(TString text);
}

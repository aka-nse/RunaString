using System.Runtime.CompilerServices;

#pragma warning disable IDE0130
namespace RunaString.RegularExpressions;
#pragma warning restore IDE0130

internal static class StackHelper
{
    public static bool TryEnsureSufficientExecutionStack() =>
        RuntimeHelpers.TryEnsureSufficientExecutionStack();
}

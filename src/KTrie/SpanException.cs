using System;
using System.Runtime.CompilerServices;

namespace KTrie;

internal static class SpanException
{
    public static void ThrowIfNullOrEmpty(ReadOnlySpan<char> argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument.IsEmpty)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(argument.ToString(), paramName);
        }
    }
}

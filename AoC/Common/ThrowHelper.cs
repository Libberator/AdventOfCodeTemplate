using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AoC;

internal static class ThrowHelper
{
    public static void ThrowIfNullOrEmpty([NotNull] string? value, string? errorMessage = null,
        [CallerArgumentExpression(nameof(value))]
        string? paramName = null)
    {
        if (!string.IsNullOrEmpty(value)) return;
        errorMessage ??= "Value cannot be null or empty";
        throw new ArgumentNullException(paramName, errorMessage);
    }

    public static void ThrowIfOutOfRange(int value, int min, int max, string? errorMessage = null,
        [CallerArgumentExpression(nameof(value))]
        string? paramName = null)
    {
        if (value >= min && value <= max) return;
        errorMessage ??= $"Value must be between {min} and {max}.";
        throw new ArgumentOutOfRangeException(paramName, value, errorMessage);
    }
}
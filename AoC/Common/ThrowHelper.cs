using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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

    /// <summary>
    ///     Use like so: `ThrowHelper.Try(() => SomeMethod(args));`
    /// </summary>
    public static void Try(Action method)
    {
        try
        {
            method();
        }
        catch (Exception e)
        {
            Logger.Log(e.Message, ConsoleColor.Red);
        }
    }

    /// <summary>
    ///     Use like so: `await ThrowHelper.TryAsync(async () => await SomeAsyncMethod(args));`
    /// </summary>
    public static async Task TryAsync(Func<Task> asyncMethod)
    {
        try
        {
            await asyncMethod();
        }
        catch (Exception e)
        {
            Logger.Log(e.Message, ConsoleColor.Red);
        }
    }

    /// <summary>
    ///     Use like so: `var result = await ThrowHelper.TryAsync(async () => await SomeAsyncMethod(args));`
    /// </summary>
    public static async Task<T> TryAsync<T>(Func<Task<T>> asyncMethod)
    {
        try
        {
            return await asyncMethod();
        }
        catch (Exception e)
        {
            Logger.Log(e.Message, ConsoleColor.Red);
            return default!;
        }
    }
}
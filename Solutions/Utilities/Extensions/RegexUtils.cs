using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC.Utilities.Extensions;

public static partial class Utils
{
    [GeneratedRegex(@"(-?\d+)", RegexOptions.Compiled)]
    public static partial Regex NumberPattern();

    // Note: uses either a comma or a period as the delimiter
    [GeneratedRegex(@"(-?\d+(?:[,.]{1}\d{3})?)", RegexOptions.Compiled)]
    public static partial Regex NumberThousandsPattern();

    // Note: uses either a comma or a period as the delimiter
    [GeneratedRegex(@"(-?\d+(?:[,.]{1}\d+)?)", RegexOptions.Compiled)]
    public static partial Regex FloatPattern();

    [GeneratedRegex(@"(-?\d+[, ]+-?\d+)", RegexOptions.Compiled)]
    public static partial Regex Vec2DPattern();

    [GeneratedRegex(@"(-?\d+(?:[, ]+-?\d+){2})", RegexOptions.Compiled)]
    public static partial Regex Vec3DPattern();

    [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
    public static partial Regex WhiteSpacePattern();

    public static T Parse<T>(this Match match, int group = 1, IFormatProvider? provider = null)
        where T : ISpanParsable<T> => T.Parse(match.Groups[group].ValueSpan, provider);

    public static T ParseOrDefault<T>(this Match match, T @default = default!, int group = 1,
        IFormatProvider? provider = null) where T : ISpanParsable<T> =>
        match.Success ? T.Parse(match.Groups[group].ValueSpan, provider) : @default;

    public static IEnumerable<T> ParseMany<T>(this MatchCollection matches, IFormatProvider? provider = null)
        where T : ISpanParsable<T> =>
        matches.SelectMany(m => m.Captures.Select(capture => T.Parse(capture.ValueSpan, provider)));

    public static IEnumerable<T> Parse<T>(this string s, Regex pattern, IFormatProvider? provider = null)
        where T : ISpanParsable<T> => pattern.Matches(s).ParseMany<T>(provider);

    public static IEnumerable<T> ParseMany<T>(this string[] s, Regex pattern, IFormatProvider? provider = null)
        where T : ISpanParsable<T> => s.SelectMany(l => l.Parse<T>(pattern, provider));
}
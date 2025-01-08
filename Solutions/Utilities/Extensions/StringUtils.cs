using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using AoC.Utilities.Geometry;

namespace AoC.Utilities.Extensions;

public static partial class Utils
{
    public static float[] ParseFloats(this string s) => s.Parse<float>(FloatPattern()).ToArray();
    public static float[] ParseFloats(this string[] s) => s.ParseMany<float>(FloatPattern()).ToArray();
    public static int[] ParseInts(this string s) => s.Parse<int>(NumberPattern()).ToArray();
    public static int[] ParseInts(this string[] s) => s.ParseMany<int>(NumberPattern()).ToArray();
    public static long[] ParseLongs(this string s) => s.Parse<long>(NumberPattern()).ToArray();
    public static long[] ParseLongs(this string[] s) => s.ParseMany<long>(NumberPattern()).ToArray();
    public static Vec2D[] ParseVec2Ds(this string s) => s.Parse<Vec2D>(Vec2DPattern()).ToArray();
    public static Vec2D[] ParseVec2Ds(this string[] s) => s.ParseMany<Vec2D>(Vec2DPattern()).ToArray();

    /// <summary>Converts a char to an integer (0-9). This assumes your char matches [0-9].</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int AsDigit(this char c) => c - '0'; // `c & 0xF;` also works (same performance) but is less readable

    /// <summary>Converts a char to an integer (0-25). This assumes your char matches [A-Za-z].</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int AsIndex(this char c) => (c - 1) & 0x1F; // `char.ToLower(c) - 'a';` also works but is slower

    /// <summary>Chunk the source between null or whitespace strings.</summary>
    public static IList<string[]> ChunkByNonEmpty(this IEnumerable<string> source) =>
        source.ChunkBy(s => !string.IsNullOrWhiteSpace(s));

    /// <summary>
    ///     Get the size of the grid of characters as (rows, cols). This assumes it's rectangular and has at least 1 row.
    /// </summary>
    public static Vec2D GetGridSize(this string[] grid) => new(grid.Length, grid[0].Length);

    /// <summary>Finds first instance of character, returns position as (row, col). If not found, returns (-1,-1).</summary>
    public static Vec2D FindPosOf(this string[] data, char target)
    {
        int y = -1, x = Array.FindIndex(data, line => (y = line.IndexOf(target)) != -1);
        return new Vec2D(x, y);
    }

    /// <summary>Tries to find first instance of character, outputs position as (row, col).</summary>
    public static bool TryFindPosOf(this string[] data, char target, out Vec2D pos)
    {
        pos = data.FindPosOf(target);
        return pos.X != -1 && pos.Y != -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char GetAt(this string[] data, Vec2D pos) => data[pos.X][pos.Y];

    /// <summary>
    ///     Replaces the string with the character at <paramref name="pos" /> changed to <paramref name="value" />.
    /// </summary>
    public static void SetAt(this string[] data, Vec2D pos, char value) =>
        data[pos.X] = data[pos.X].SetAt(pos.Y, value);

    /// <summary>
    ///     Returns a new string with the character at <paramref name="index" /> changed to <paramref name="value" />.
    /// </summary>
    public static string SetAt(this string s, int index, char value)
    {
        var span = s.AsSpan();
        Span<char> buffer = stackalloc char[span.Length];
        span.CopyTo(buffer);
        buffer[index] = value;
        return new string(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string RemoveWhiteSpace(this string str) => WhiteSpacePattern().Replace(str, string.Empty);

    /// <summary>Returns a concatenated string with the source <paramref name="s" /> repeated <paramref name="n" /> times.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Repeat(this string s, int n) => new StringBuilder(n * s.Length).Insert(0, s, n).ToString();

    /// <summary>Returns a concatenated string with the source <paramref name="c" /> repeated <paramref name="n" /> times.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Repeat(this char c, int n) => new(c, n);

    /// <summary>Returns a new string[] with the source data transposed (col swapped with row). Does not modify in-place.</summary>
    public static string[] Transpose(this string[] data)
    {
        var width = data[0].Length;
        if (data.Any(line => line.Length != width))
            throw new ArgumentException($"Invalid data length. Not all lines are {width} characters long");

        var result = new string[width];
        var sb = new StringBuilder();
        for (var i = 0; i < width; i++)
        {
            foreach (var line in data)
                sb.Append(line[i]);
            result[i] = sb.ToString();
            sb.Clear();
        }

        return result;
    }
}
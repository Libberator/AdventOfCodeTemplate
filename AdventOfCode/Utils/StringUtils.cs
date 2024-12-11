using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AoC;

public static partial class Utils
{
    public static int[] ToIntArray(this string[] data) => Array.ConvertAll(data, int.Parse);

    public static long[] ToLongArray(this string[] data) => Array.ConvertAll(data, long.Parse);

    public static ulong[] ToULongArray(this string[] data) => Array.ConvertAll(data, ulong.Parse);

    public static BigInteger[] ToBigIntArray(this string[] data) => Array.ConvertAll(data, BigInteger.Parse);

    public static int BinaryToInt(this string s) => Convert.ToInt32(s, 2);

    public static long BinaryToLong(this string s) => Convert.ToInt64(s, 2);

    public static int HexToInt(this string s) => Convert.ToInt32(s, 16);

    /// <summary>
    ///     Returns a 4-length string of 1's and 0's, given a char from '0' to 'F'. Useful for converting data from a
    ///     string.
    /// </summary>
    public static string HexToBinary(this char hexChar)
    {
        return hexChar switch
        {
            '0' => "0000",
            '1' => "0001",
            '2' => "0010",
            '3' => "0011",
            '4' => "0100",
            '5' => "0101",
            '6' => "0110",
            '7' => "0111",
            '8' => "1000",
            '9' => "1001",
            'A' or 'a' => "1010",
            'B' or 'b' => "1011",
            'C' or 'c' => "1100",
            'D' or 'd' => "1101",
            'E' or 'e' => "1110",
            'F' or 'f' => "1111",
            _ => throw new IndexOutOfRangeException($"Unable to convert hexadecimal char to binary: '{hexChar}'")
        };
    }

    /// <summary>Returns a concatenated string with the <paramref name="source" /> repeated <paramref name="n" /> times.</summary>
    public static string Repeat(this string source, int n) =>
        new StringBuilder(n * source.Length).Insert(0, source, n).ToString();

    /// <summary>Returns a concatenated string with the source <paramref name="c" /> repeated <paramref name="n" /> times.</summary>
    public static string Repeat(this char c, int n) => new(c, n);
    
    /// <summary>
    /// Returns a new string[] with the source data transposed (col swapped with row). Does not modify in-place. 
    /// </summary>
    public static string[] Transpose(this string[] data)
    {
        var width = data[0].Length;
        if (data.Any(line => line.Length != width))
            throw new ArgumentException($"Invalid data length. Not all lines are {width} characters");
        
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
    
    /// <summary>
    /// Convert to an arbitrary base, between 2 and 36
    /// </summary>
    public static string ToBase(this int value, int radix)
    {
        const int bitsInInt = 32;
        const string digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        
        if (radix < 2 || radix > digits.Length)
            throw new ArgumentException($"The radix must be >= 2 and <= {digits.Length}");
        
        var i = bitsInInt - 1;
        var currentNumber = Math.Abs(value);
        var buffer = new char[bitsInInt];
        
        while (currentNumber != 0)
        {
            buffer[i--] = digits[currentNumber % radix];
            currentNumber /= radix;
        }

        var result = new string(buffer, i + 1, bitsInInt - i - 1);// char[32 - i];
        if (value < 0)
            result = "-" + result;

        return result;
    }
    
    /// <summary>
    /// Convert to an arbitrary base, between 2 and 36
    /// </summary>
    public static string ToBase(this long value, int radix)
    {
        const int bitsInLong = 64;
        const string digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        if (radix < 2 || radix > digits.Length)
            throw new ArgumentException($"The radix must be >= 2 and <= {digits.Length}");

        if (value == 0)
            return "0";

        var i = bitsInLong - 1;
        var currentNumber = Math.Abs(value);
        var buffer = new char[bitsInLong];

        while (currentNumber != 0)
        {
            var remainder = (int)(currentNumber % radix);
            buffer[i--] = digits[remainder];
            currentNumber /= radix;
        }

        var result = new string(buffer, i + 1, bitsInLong - i - 1);
        if (value < 0)
            result = "-" + result;

        return result;
    }
}
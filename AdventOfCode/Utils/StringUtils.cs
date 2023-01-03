using System;
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
    /// <summary>Returns a 4-length string of 1's and 0's, given a char from '0' to 'F'. Useful for converting data from a string.</summary>
    public static string HexToBinary(this char hexChar) => hexChar switch
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
        _ => throw new IndexOutOfRangeException($"Unable to convert hexadecimal char to binary: '{hexChar}'"),
    };

    /// <summary>Returns a concatenated string with the <paramref name="source"/> repeated <paramref name="n"/> times.</summary>
    public static string Repeat(this string source, int n) => new StringBuilder(n * source.Length).Insert(0, source, n).ToString();
}
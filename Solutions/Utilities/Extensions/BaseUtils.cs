using System;

namespace AoC.Utilities.Extensions;

public partial class Utils
{
    /// <summary>Given a char from '0' to 'F', return a 4-length string of 1's and 0's.</summary>
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
        _ => throw new ArgumentOutOfRangeException(nameof(hexChar),
            $"Unable to convert hexadecimal char to binary: '{hexChar}'")
    };

    /// <summary>Convert to an arbitrary base, between 2 and 36.</summary>
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

        var result = new string(buffer, i + 1, bitsInInt - i - 1); // char[32 - i];
        if (value < 0)
            result = "-" + result;

        return result;
    }

    /// <summary>Convert to an arbitrary base, between 2 and 36.</summary>
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

    /// <summary>Converts a string representation of a number in a given base (from 2 to 36) to a base-10 integer.</summary>
    public static long ToBase10(this string s, int fromBase)
    {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentException("Input number cannot be null or empty.", nameof(s));

        if (fromBase is < 2 or > 36)
            throw new ArgumentException("Base must be in the range 2 to 36.", nameof(fromBase));

        var isNegative = s.StartsWith('-');
        var digits = isNegative ? s[1..] : s;

        long result = 0;
        long multiplier = 1;

        for (var i = digits.Length - 1; i >= 0; i--)
        {
            var digitChar = digits[i];
            var digitValue = digitChar is >= '0' and <= '9'
                ? digitChar.AsDigit()
                : char.ToUpper(digitChar) - 'A' + 10;

            if (digitValue < 0 || digitValue >= fromBase)
                throw new ArgumentException($"Invalid character '{digitChar}' for base {fromBase}.");

            result += digitValue * multiplier;
            multiplier *= fromBase;
        }

        return isNegative ? -result : result;
    }

    /// <summary>Only valid bases are: 2, 8, 10, or 16. If other base is required, use ToBase10().</summary>
    public static int ToIntFromBase(this string s, int fromBase) => Convert.ToInt32(s, fromBase);

    /// <summary>Only valid bases are: 2, 8, 10, or 16. If other base is required, use ToBase10().</summary>
    public static long ToLongFromBase(this string s, int fromBase) => Convert.ToInt64(s, fromBase);
}
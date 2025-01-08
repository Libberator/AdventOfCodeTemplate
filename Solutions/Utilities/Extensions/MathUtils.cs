using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AoC.Utilities.Extensions;

// Note: To find mathematical formulas for specific sequences, go to https://oeis.org/
public static partial class Utils
{
    ///<summary>Binomial coefficient. a.k.a. "n choose k". ex. "7 choose 2" = 7 * (7 - 1) / 2 = 21.</summary>
    public static int BinomialChoose(this int n, int k)
    {
        if (k > n - k) k = n - k; // take advantage of symmetry
        var c = 1;
        for (var i = 1; i <= k; i++, n--)
            c = c / i * n + c % i * n / i;
        return c;
    }

    /// <summary>
    ///     Returns a sorted list of all the factors of <paramref name="n" />.
    ///     Throws exception if <paramref name="n" /> is negative or 0.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException" />
    public static List<T> Factors<T>(this T n) where T : INumber<T>
    {
        if (n < T.One)
            throw new ArgumentOutOfRangeException(nameof(n), $"Argument must be greater than 0. Value given: {n}");
        List<T> factors = [T.One];

        var i = T.One + T.One;
        while (i * i <= n)
        {
            if (n % i == T.Zero)
            {
                factors.Add(i);
                var pair = n / i;
                if (i != pair) factors.Add(pair);
            }

            i++;
        }

        if (n > T.One) factors.Add(n);
        factors.Sort();
        return factors;
    }

    /// <summary>
    ///     This will return the first prime number that <paramref name="n" /> is divisible by.
    ///     If <paramref name="n" /> is 1 or prime, it will return itself. Negative values throw exception.
    /// </summary>
    public static T FirstPrimeFactor<T>(this T n) where T : INumber<T>
    {
        var two = T.One + T.One;
        if (n < T.Zero)
            throw new ArgumentOutOfRangeException(nameof(n), $"Argument must be a positive integer. Value given: {n}");
        if (n % two == T.Zero) return two;

        for (var d = two + T.One; d * d <= n; d += two)
            if (n % d == T.Zero)
                return d;

        return n;
    }

    /// <summary>Returns the greatest common divisor of the two arguments.</summary>
    public static T GreatestCommonDivisor<T>(T a, T b) where T : INumber<T>
    {
        while (true)
        {
            if (b <= T.Zero) return T.Abs(a);
            var a1 = a;
            a = b;
            b = a1 % b;
        }
    }

    // Note: This "IsPrime" method is a "naive" implementation.
    // For values greater than 2^14, see Miller-Rabin for a quicker approach: https://cade.site/diy-fast-isprime
    /// <summary>Checks if number is prime: greater than 1 with no positive divisors other than 1 and itself.</summary>
    public static bool IsPrime<T>(this T n) where T : INumber<T> => n > T.One && n.FirstPrimeFactor() == n;

    /// <summary>Returns the least common multiple of the two arguments.</summary>
    public static T LeastCommonMultiple<T>(T a, T b) where T : INumber<T> => a * b / GreatestCommonDivisor(a, b);

    /// <summary>
    ///     Computes `n mod m`. This is different from the `%` operator in the case of
    ///     negative numbers, e.g. `-8 % 7 = -1`, but `-8.Mod(7) = 6`.
    /// </summary>
    public static T Mod<T>(this T n, T mod) where T : INumber<T>
    {
        var remainder = n % mod;
        return remainder < T.Zero ? remainder + mod : remainder;
    }

    /// <summary>Computes the modular inverse of a number 'number' modulo 'modulus'.</summary>
    public static T ModInverse<T>(this T number, T modulus) where T : INumber<T>
    {
        T currentCoefficient = T.Zero, nextCoefficient = T.One;
        T currentRemainder = modulus, nextRemainder = number;

        // Extended Euclidean Algorithm to find the inverse
        while (nextRemainder != T.Zero)
        {
            var quotient = currentRemainder / nextRemainder;

            // Update coefficients
            var tempT = currentCoefficient;
            currentCoefficient = nextCoefficient;
            nextCoefficient = tempT - quotient * nextCoefficient;

            // Update remainders
            var tempR = currentRemainder;
            currentRemainder = nextRemainder;
            nextRemainder = tempR - quotient * nextRemainder;
        }

        // Ensure that gcd(number, modulus) is 1
        if (currentRemainder > T.One)
            throw new ArgumentException(
                $"Modular inverse does not exist because {number} and {modulus} are not coprime.");

        // Ensure the result is positive
        if (currentCoefficient < T.Zero)
            currentCoefficient += modulus;

        return currentCoefficient;
    }

    /// <summary>Similar to Sum(), except each element in the <paramref name="source" /> is multiplied by each other.</summary>
    public static T Product<T>(this IEnumerable<T> source) where T : INumber<T> =>
        source.Aggregate(T.MultiplicativeIdentity, (current, value) => current * value);

    /// <summary>Rounds to nearest integer value, and returns as integer. Uses Banker's Rounding (to nearest even)</summary>
    public static int RoundToInt(this double value) => (int)Math.Round(value);

    /// <summary>Rounds to nearest integer value, and returns as integer. Uses Banker's Rounding (to nearest even)</summary>
    public static int RoundToInt(this float value) => (int)Math.Round(value);

    /// <summary>Given n, returns the sum of 1 + 2 + ... + n -1 + n. Same as (n+1).BinomialChoose(2).</summary>
    public static T TriangleSum<T>(T n) where T : INumber<T> => n * (n + T.One) / (T.One + T.One);

    /// <summary>Calculate the expectation of the squared deviation from its mean from a set of numbers.</summary>
    public static double Variance(this IEnumerable<int> source)
    {
        var array = source.ToArray();
        var mean = array.Average();
        return array.Sum(n => (n - mean) * (n - mean)) / array.Length;
    }
}
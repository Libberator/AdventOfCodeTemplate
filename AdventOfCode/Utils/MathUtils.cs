using System;
using System.Collections.Generic;
using System.Numerics;

namespace AoC;

public static partial class Utils
{
    /// <summary>Similar to Sum(), except each element in the <paramref name="source"/> is multiplied by each other.</summary>
    public static T Product<T>(this IEnumerable<T> source) where T : INumber<T>
    {
        T result = T.MultiplicativeIdentity;
        foreach (var value in source) result *= value;
        return result;
    }

    /// <summary>
    /// Returns a sorted list of all the factors of <paramref name="n"/>. 
    /// Throws exception if <paramref name="n"/> is negative or 0.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public static List<int> Factor(int n)
    {
        if (n < 1) throw new ArgumentOutOfRangeException($"Argument must be greater than 0. Value given: {n}");
        List<int> factors = [1];
        var upperLimit = (int)Math.Sqrt(n); // casting to int automatically floors
        for (var i = upperLimit; i >= 2; i--)
        {
            if (n % i == 0)
            {
                factors.Insert(1, i);
                var pair = n / i;
                if (i != pair) factors.Add(pair);
            }
        }
        if (n > 1) factors.Add(n);
        return factors;
    }

    /// <summary>
    /// Returns a sorted list of all the factors of <paramref name="n"/>. 
    /// Throws exception if <paramref name="n"/> is negative or 0.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public static List<long> Factor(this long n)
    {
        if (n < 1) throw new ArgumentOutOfRangeException($"Argument must be greater than 0. Value given: {n}");
        List<long> factors = [1];
        var upperLimit = (long)Math.Sqrt(n); // casting automatically floors
        for (var i = upperLimit; i >= 2; i--)
        {
            if (n % i == 0)
            {
                factors.Insert(1, i);
                var pair = n / i;
                if (i != pair) factors.Add(pair);
            }
        }
        if (n > 1) factors.Add(n);
        return factors;
    }

    // Note: This "IsPrime" method is a "naive" implementation.
    // For values greater than 2^14, see Miller-Rabin for a quicker approach: https://cade.site/diy-fast-isprime
    /// <summary>Checks if <paramref name="n"/> is prime: greater than 1 with no positive divisors other than 1 and itself.</summary>
    public static bool IsPrime(this int n) => IsPrime((long)n);
    /// <summary>Checks if <paramref name="n"/> is prime: greater than 1 with no positive divisors other than 1 and itself.</summary>
    public static bool IsPrime(this long n)
    {
        if (n <= 1) return false;
        return n.FirstPrimeFactor() == n;
    }

    /// <summary>
    /// This will return the first prime number that <paramref name="n"/> is divisible by.
    /// If <paramref name="n"/> is 1 or prime, it will return itself. Negative values throw exception.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public static int FirstPrimeFactor(this int n) => (int)FirstPrimeFactor((long)n);
    /// <summary>
    /// This will return the first prime number that <paramref name="n"/> is divisible by.
    /// If <paramref name="n"/> is 1 or prime, it will return itself. Negative values throw exception.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public static long FirstPrimeFactor(this long n)
    {
        if (n < 0) throw new ArgumentOutOfRangeException($"n must be a positive integer. Value given: {n}");
        if ((n & 1) == 0) return 2;

        for (int d = 3; d * d <= n; d += 2)
            if (n % d == 0) return d;

        return n;
    }

    /// <summary>Returns the greatest common divisor of the two arguments.</summary>
    public static T GreatestCommonDivisor<T>(T a, T b) where T : INumber<T> => b > T.Zero ? GreatestCommonDivisor(b, a % b) : T.Abs(a);

    /// <summary>Returns the least common multiple of the two arguments.</summary>
    public static T LeastCommonMultiple<T>(T a, T b) where T : INumber<T> => a * b / GreatestCommonDivisor(a, b);

    /// <summary>
    /// Computes `n mod m`. This is different than the `%` operator in the case of
    /// negative numbers, e.g. `-8 % 7 = -1`, but `-8.Mod(7) = 6`.
    /// </summary>
    public static int Mod(this int n, int mod)
    {
        var remainder = n % mod;
        return remainder + (remainder < 0 ? mod : 0);
    }

    // Note: To find mathematical formulas for specific sequences, go to https://oeis.org/
}

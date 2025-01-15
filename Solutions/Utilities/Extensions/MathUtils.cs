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

    /// <summary>
    ///     Get the next (higher) power of 10. Similar to `10 * Math.Pow(10, Math.Floor(Math.Log10(number))` but more
    ///     efficient.
    /// </summary>
    /// <example>
    ///     Useful for number concatenation. Instead of using strings to do "123" + "45", it would be
    ///     <code>123 * 45.NextPowerOfTen() + 45  // 12,345</code>
    /// </example>
    /// <returns>[1-9] -> 10. [10-99] -> 100. [100-999] -> 1000. etc.</returns>
    public static T NextPowerOf10<T>(this T value) where T : INumber<T>
    {
        if (T.IsNegative(value))
            throw new ArgumentException($"Argument must be a positive integer. Value given: {value}", nameof(value));
        var ten = (T.One + T.One + T.One) * (T.One + T.One + T.One) + T.One;
        var power = ten;
        while (value >= ten)
        {
            value /= ten;
            power *= ten;
        }

        return power;
    }

    /// <summary>Similar to Sum(), except each element in the <paramref name="source" /> is multiplied by each other.</summary>
    public static T Product<T>(this IEnumerable<T> source) where T : INumber<T> =>
        source.Aggregate(T.MultiplicativeIdentity, (current, value) => current * value);

    /// <summary>
    ///     Generic solver for a set of linear equations. Expects an NxN matrix of coefficient and N-length array of
    ///     constants where N is the number of unknown variables that it will solve for. 
    /// </summary>
    /// <param name="coefficients">Multidimensional array of coefficients; the multipliers of the unknown variables</param>
    /// <param name="constants">The known values unassociated with any variable on the right-hand side of the equation</param>
    /// <returns>The values for the unknown variables which satisfies all the equations. Handle floating point precision</returns>
    /// <example>
    ///     Assume you have the set of equations in the following form:<br />
    ///     [2x + 3y = 8] and [3x + y = 5]<br />
    ///     Here's what solving that would look like:
    ///     <code>
    /// var coefficients = new int[,] { { 2, 3 }, { 3, 1 } };
    /// var constants = new int[] { 8, 5 };
    /// var result = Utils.SolveLinearEquations(coefficients, constants); // [ 1, 2 ]
    /// </code>
    /// </example>
    public static T[] SolveLinearEquations<T>(T[,] coefficients, T[] constants) where T : INumber<T>
    {
        var n = constants.Length;

        if (coefficients.GetLength(0) != n || coefficients.GetLength(1) != n)
            throw new ArgumentException("Matrix dimensions must match the number of constants.");

        // Augmented matrix
        var augmented = new T[n, n + 1];
        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < n; j++)
                augmented[i, j] = coefficients[i, j];
            augmented[i, n] = constants[i];
        }

        // Gaussian elimination
        for (var i = 0; i < n; i++)
        {
            // Pivot selection (partial pivoting)
            var maxRow = i;
            for (var k = i + 1; k < n; k++)
                if (T.Abs(augmented[k, i]) > T.Abs(augmented[maxRow, i]))
                    maxRow = k;

            // Swap rows
            for (var k = i; k < n + 1; k++)
                (augmented[i, k], augmented[maxRow, k]) = (augmented[maxRow, k], augmented[i, k]);

            // Make the pivot element 1
            var pivot = augmented[i, i];
            if (pivot == T.Zero)
                throw new InvalidOperationException("Matrix is singular or system has no unique solution.");

            for (var k = i; k < n + 1; k++)
                augmented[i, k] /= pivot;

            // Eliminate column below pivot
            for (var k = i + 1; k < n; k++)
            {
                var factor = augmented[k, i];
                for (var j = i; j < n + 1; j++)
                    augmented[k, j] -= factor * augmented[i, j];
            }
        }

        // Back substitution
        var result = new T[n];
        for (var i = n - 1; i >= 0; i--)
        {
            result[i] = augmented[i, n];
            for (var j = i + 1; j < n; j++)
                result[i] -= augmented[i, j] * result[j];
        }

        return result;
    }

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
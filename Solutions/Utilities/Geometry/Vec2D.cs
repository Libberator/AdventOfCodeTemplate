using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace AoC.Utilities.Geometry;

// ReSharper disable InconsistentNaming
/// <summary>
///     A value type containing two integers. All directions are interpreted as (row, col)
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public readonly record struct Vec2D(int X, int Y) : ISpanParsable<Vec2D>, ISpanFormattable
{
    #region Static Fields

    ///<summary>Gets the vector (1,1).</summary>
    public static readonly Vec2D One = new(1, 1);

    ///<summary>Gets the vector (0,0).</summary>
    public static readonly Vec2D Zero = new(0, 0);

    /// <summary>Gets the vector (-1,0).</summary>
    public static readonly Vec2D N = new(-1, 0);

    /// <summary>Gets the vector (0,1).</summary>
    public static readonly Vec2D E = new(0, 1);

    /// <summary>Gets the vector (1,0).</summary>
    public static readonly Vec2D S = new(1, 0);

    /// <summary>Gets the vector (0,-1).</summary>
    public static readonly Vec2D W = new(0, -1);

    /// <summary>Gets the vector (-1,1).</summary>
    public static readonly Vec2D NE = new(-1, 1);

    /// <summary>Gets the vector (1,1).</summary>
    public static readonly Vec2D SE = new(1, 1);

    /// <summary>Gets the vector (1,-1).</summary>
    public static readonly Vec2D SW = new(1, -1);

    /// <summary>Gets the vector (-1,-1).</summary>
    public static readonly Vec2D NW = new(-1, -1);

    /// <summary>Returns the four cardinal directions: N, E, S, W.</summary>
    public static readonly Vec2D[] CardinalDirs = [N, E, S, W];

    /// <summary>Also called "inter-cardinal directions." Returns the four ordinal directions: NE, SE, SW, NW.</summary>
    public static readonly Vec2D[] OrdinalDirs = [NE, SE, SW, NW];

    /// <summary>Returns all 8 points of the compass clockwise starting from N.</summary>
    public static readonly Vec2D[] AllDirs = [N, NE, E, SE, S, SW, W, NW];

    #endregion

    #region Member Methods

    /// <summary>Returns a vector whose elements are the absolute values of each of the vector's elements.</summary>
    public Vec2D Abs() => Abs(this);

    /// <summary>Restricts a vector between a minimum and a maximum value, inclusive.</summary>
    public Vec2D Clamp(Vec2D min, Vec2D max) => Clamp(min.X, max.X, min.Y, max.Y);

    /// <summary>Restricts a vector between a minimum and a maximum value, inclusive.</summary>
    public Vec2D Clamp(int minX, int maxX, int minY, int maxY) =>
        new(Math.Clamp(X, minX, maxX), Math.Clamp(Y, minY, maxY));

    /// <summary>Returns the determinant of this and an <paramref name="other" /> vector. Essentially the 2D Cross Product</summary>
    public int Determinant(Vec2D other) => Determinant(this, other);

    /// <summary>Computes the Euclidean distance between this and an <paramref name="other" /> vector.</summary>
    public double DistanceEuclidean(Vec2D other) => Math.Sqrt(DistanceSquared(other));

    /// <summary>
    ///     Computes the Chebyshev distance, also known as chessboard distance - the amount of moves a king would take to
    ///     get from a to b.
    /// </summary>
    public int DistanceChebyshev(Vec2D other) => Math.Max(Math.Abs(other.X - X), Math.Abs(other.Y - Y));

    /// <summary>
    ///     Computes the Manhattan distance (a.k.a. Taxicab distance) between this and an <paramref name="other" /> vector. No
    ///     diagonal moves.
    /// </summary>
    public int DistanceManhattan(Vec2D other) => Math.Abs(other.X - X) + Math.Abs(other.Y - Y);

    /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
    public int DistanceSquared(Vec2D other) => (other.X - X) * (other.X - X) + (other.Y - Y) * (other.Y - Y);

    /// <summary>Returns the dot product of two vectors.</summary>
    public int Dot(Vec2D other) => Dot(this, other);

    /// <summary>
    ///     Returns all points in a rectangle, or line, between <see cref="Zero" /> and this vector, exclusive.
    /// </summary>
    public IEnumerable<Vec2D> GeneratePoints(int padding = 0) => GeneratePoints(X, Y, padding);

    /// <summary>
    ///     Returns all points in a rectangle, or line, between this vector (inclusive) and <paramref name="max" />
    ///     (exclusive).
    /// </summary>
    public IEnumerable<Vec2D> GeneratePoints(Vec2D max, int padding = 0) => GeneratePoints(this, max, padding);

    /// <summary>
    ///     Returns all points in a rectangle, or line, between <see cref="Zero" /> and this vector as the max (both
    ///     inclusive).
    /// </summary>
    public IEnumerable<Vec2D> GeneratePointsInclusive(int padding = 0) => GeneratePointsInclusive(X, Y, padding);

    /// <summary>
    ///     Returns all points in a rectangle, or line, between this vector and an <paramref name="other" /> opposite
    ///     corner point.
    /// </summary>
    public IEnumerable<Vec2D> GeneratePointsInclusive(Vec2D other, int padding = 0) =>
        GeneratePointsInclusive(this, other, padding);

    /// <summary>
    ///     Returns all positions in a diamond pattern that is between <paramref name="minDistance" /> (default 0) and
    ///     <paramref name="maxDistance" /> (inclusive) units away from this vector via Manhattan Distance.
    /// </summary>
    /// <param name="maxDistance">Should be greater than or equal to minDistance</param>
    /// <param name="minDistance">Should be greater than or equal to zero (default)</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public IEnumerable<Vec2D> GetDiamondPattern(int maxDistance, int minDistance = 0) =>
        GetDiamondPattern(this, maxDistance, minDistance);

    /// <summary>Determines if the two vectors are right next to each other laterally.</summary>
    public bool IsAdjacentTo(Vec2D other) => DistanceManhattan(other) == 1;

    /// <summary>
    ///     When treating X and Y as values that are proportional to each other (e.g. imagine a fraction X/Y), this
    ///     returns true if the two values cannot be reduced any further. For example, if this were a direction vector,
    ///     then this is true when the values are in their smallest magnitude while maintaining the same direction.
    /// </summary>
    public bool IsCanonical() => MinCollinear(this) == this;

    /// <summary>Determines if the two vectors are along the same diagonal (same X and Y distance apart).</summary>
    public bool IsDiagonalTo(Vec2D other) => Math.Abs(other.X - X) == Math.Abs(other.Y - Y);

    /// <summary>Horizontally or Vertically aligned at any distance, but not the same position</summary>
    public bool IsLateralTo(Vec2D other) => (other.X == X) ^ (other.Y == Y);

    /// <summary>
    ///     When treated as directions, determines if the two are parallel and neither are <see cref="Zero" />
    /// </summary>
    public bool IsParallelTo(Vec2D other) => this != Zero && other != Zero && MinCollinear(this) == MinCollinear(other);

    /// <summary>
    ///     When treated as directions, determines if the two are perpendicular and neither are <see cref="Zero" />
    /// </summary>
    public bool IsPerpendicularTo(Vec2D other) => this != Zero && other != Zero && Dot(this, other) == 0;

    /// <summary>Returns true if this vector is between <see cref="Zero" /> (inclusive) and max (exclusive).</summary>
    public bool IsWithinBounds(Vec2D max) => WithinBounds(this, Zero, max);

    /// <summary>Returns true if this vector is between min (inclusive) and max (exclusive).</summary>
    public bool IsWithinBounds(Vec2D min, Vec2D max) => WithinBounds(this, min, max);

    /// <summary>Returns true if this vector is between 0 (inclusive) and max (exclusive).</summary>
    public bool IsWithinBounds(int maxX, int maxY) => WithinBounds(this, 0, maxX, 0, maxY);

    /// <summary>Returns true if this vector is between min (inclusive) and max (exclusive).</summary>
    public bool IsWithinBounds(int minX, int maxX, int minY, int maxY) => WithinBounds(this, minX, maxX, minY, maxY);

    /// <summary>Returns true if this vector is between <see cref="Zero" /> and max (inclusive).</summary>
    public bool IsWithinBoundsInclusive(Vec2D max) => WithinBoundsInclusive(this, Zero, max);

    /// <summary>Returns true if this vector is between min and max (inclusive).</summary>
    public bool IsWithinBoundsInclusive(Vec2D min, Vec2D max) => WithinBoundsInclusive(this, min, max);

    /// <summary>Returns true if this vector is between 0 and max (inclusive).</summary>
    public bool IsWithinBoundsInclusive(int maxX, int maxY) => WithinBoundsInclusive(this, 0, maxX, 0, maxY);

    /// <summary>Returns true if this vector is between min and max (inclusive).</summary>
    public bool IsWithinBoundsInclusive(int minX, int maxX, int minY, int maxY) =>
        WithinBoundsInclusive(this, minX, maxX, minY, maxY);

    /// <summary>Returns the length, or magnitude, of the vector (a.k.a. Euclidean Distance from origin).</summary>
    public double Length() => Length(this);

    /// <summary>Returns the squared length, or magnitude, of the vector.</summary>
    public int LengthSquared() => LengthSquared(this);

    /// <summary>Returns a vector whose elements are the maximum of each pair-wise.</summary>
    public Vec2D Max(Vec2D other) => Max(this, other);

    /// <summary>Returns a vector whose elements are the minimum of each pair-wise.</summary>
    public Vec2D Min(Vec2D other) => Min(this, other);

    /// <summary>Gets a new vector reduced to its smallest integer multiple, maintaining direction.</summary>
    public Vec2D MinCollinear() => MinCollinear(this);

    /// <summary>Returns a normalized vector, where each part is between -1 and 1.</summary>
    public Vec2D Normalized() => Normalize(this);

    /// <summary>Returns a vector that is the rotated clockwise 90° version of this vector.</summary>
    public Vec2D RotatedRight() => RotateRight(this);

    /// <summary>Returns a vector that is the rotated counter-clockwise 90° version of this vector.</summary>
    public Vec2D RotatedLeft() => RotateLeft(this);

    #endregion

    #region Static Methods

    /// <summary>Returns a vector whose elements are the absolute values of each of the <paramref name="value" />'s elements.</summary>
    public static Vec2D Abs(Vec2D value) => new(Math.Abs(value.X), Math.Abs(value.Y));

    /// <summary>Returns the determinant of two vectors. Essentially the 2D Cross Product.</summary>
    public static int Determinant(Vec2D a, Vec2D b) => a.X * b.Y - a.Y * b.X;

    /// <summary>Returns the dot product of two vectors.</summary>
    public static int Dot(Vec2D a, Vec2D b) => a.X * b.X + a.Y * b.Y;

    /// <summary>Returns all points in a rectangle, or line, between min (inclusive) and max (exclusive).</summary>
    public static IEnumerable<Vec2D> GeneratePoints(Vec2D min, Vec2D max, int padding = 0) =>
        GeneratePoints(min.X, max.X, min.Y, max.Y, padding);

    /// <summary>Returns all points in a rectangle, or line, between 0 (inclusive) and max (exclusive).</summary>
    public static IEnumerable<Vec2D> GeneratePoints(int maxX, int maxY, int padding = 0) =>
        GeneratePoints(0, maxX, 0, maxY, padding);

    /// <summary>Returns all points in a rectangle, or line, between min (inclusive) and max (exclusive).</summary>
    public static IEnumerable<Vec2D> GeneratePoints(int minX, int maxX, int minY, int maxY, int padding = 0)
    {
        for (var x = minX + padding; x < maxX - padding; x++)
            for (var y = minY + padding; y < maxY - padding; y++)
                yield return new Vec2D(x, y);
    }

    /// <summary>Returns all points in a rectangle, or line, between two vectors (inclusive).</summary>
    public static IEnumerable<Vec2D> GeneratePointsInclusive(Vec2D a, Vec2D b, int padding = 0)
    {
        int minX = Math.Min(a.X, b.X), maxX = Math.Max(a.X, b.X);
        int minY = Math.Min(a.Y, b.Y), maxY = Math.Max(a.Y, b.Y);
        return GeneratePointsInclusive(minX, maxX, minY, maxY, padding);
    }

    /// <summary>Returns all points in a rectangle, or line, between 0 and max (both inclusive).</summary>
    public static IEnumerable<Vec2D> GeneratePointsInclusive(int maxX, int maxY, int padding = 0) =>
        GeneratePointsInclusive(0, maxX, 0, maxY, padding);

    /// <summary>Returns all points in a rectangle, or line, between min and max (both inclusive).</summary>
    public static IEnumerable<Vec2D> GeneratePointsInclusive(int minX, int maxX, int minY, int maxY, int padding = 0)
    {
        for (var x = minX + padding; x <= maxX - padding; x++)
            for (var y = minY + padding; y <= maxY - padding; y++)
                yield return new Vec2D(x, y);
    }

    /// <summary>
    ///     Returns all positions in a diamond pattern that is between <paramref name="minDistance" /> (default 0) and
    ///     <paramref name="maxDistance" /> (inclusive) units away from <paramref name="center" /> via Manhattan Distance.
    /// </summary>
    /// <param name="center">Center of the diamond pattern</param>
    /// <param name="maxDistance">Should be greater than or equal to minDistance</param>
    /// <param name="minDistance">Should be greater than or equal to zero (default)</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IEnumerable<Vec2D> GetDiamondPattern(Vec2D center, int maxDistance, int minDistance = 0)
    {
        if (minDistance < 0)
            throw new ArgumentOutOfRangeException(nameof(minDistance), "minDistance must be >= zero");
        if (maxDistance < minDistance)
            throw new ArgumentOutOfRangeException(nameof(maxDistance), "maxDistance must be >= minDistance");
        for (var x = -maxDistance; x <= maxDistance; x++)
        {
            var maxY = maxDistance - Math.Abs(x);
            var minY = Math.Max(minDistance - Math.Abs(x), 0);

            for (var y = -maxY; y < -minY; y++)
                yield return new Vec2D(center.X + x, center.Y + y);

            for (var y = minY; y <= maxY; y++)
                yield return new Vec2D(center.X + x, center.Y + y);
        }
    }

    /// <summary>
    ///     Returns a value as the percentage that <paramref name="value" /> is between <paramref name="a" /> and
    ///     <paramref name="b" />. Not clamped: may return values outside the 0 to 1 range.
    /// </summary>
    public static double InverseLerp(Vec2D value, Vec2D a, Vec2D b)
    {
        var ab = b - a;
        var av = value - a;
        double abLengthSquared = ab.LengthSquared();
        return abLengthSquared == 0 ? 0 : Dot(av, ab) / abLengthSquared;
    }

    /// <summary>
    ///     Returns a value as the percentage that <paramref name="value" /> is between <paramref name="a" /> and
    ///     <paramref name="b" />. Clamped: will return values between 0 and 1.
    /// </summary>
    public static double InverseLerpClamped(Vec2D value, Vec2D a, Vec2D b) =>
        Math.Clamp(InverseLerp(value, a, b), 0, 1);

    /// <summary>Returns the length, or magnitude, of the vector <paramref name="value" />.</summary>
    public static double Length(Vec2D value) => Math.Sqrt(LengthSquared(value));

    /// <summary>Returns the squared length, or magnitude, of the vector <paramref name="value" />.</summary>
    public static int LengthSquared(Vec2D value) => value.X * value.X + value.Y * value.Y;

    /// <summary>Performs a linear interpolation between two vectors based on the given weighting (0 to 1).</summary>
    public static Vec2D Lerp(Vec2D a, Vec2D b, double weight)
    {
        var x = (int)Math.Round(a.X + (b.X - a.X) * weight);
        var y = (int)Math.Round(a.Y + (b.Y - a.Y) * weight);
        return new Vec2D(x, y);
    }

    /// <summary>
    ///     Given two lines (each as a directions and a point in space it travels through), determine if they intersect
    ///     and output the <paramref name="intersection" /> point (or closest approximation given integer limitations).
    /// </summary>
    public static bool LineIntersect(Vec2D pt1, Vec2D dir1, Vec2D pt2, Vec2D dir2, out Vec2D intersection)
    {
        intersection = Zero;
        var det = Determinant(dir2, dir1);
        if (det == 0) return false;

        float quotient1 = dir2.X * (pt2.Y - pt1.Y) - dir2.Y * (pt2.X - pt1.X);
        var t = quotient1 / det;
        intersection = new Vec2D((int)Math.Round(pt1.X + t * dir1.X), (int)Math.Round(pt1.Y + t * dir1.Y));

        float quotient2 = dir1.X * (pt1.Y - pt2.Y) - dir1.Y * (pt1.X - pt2.X);
        var s = -quotient2 / det;
        var intersection2 = new Vec2D((int)Math.Round(pt2.X + s * dir2.X), (int)Math.Round(pt2.Y + s * dir2.Y));

        return intersection == intersection2;
    }

    /// <summary>Re-maps a vector from one range to another.</summary>
    public static Vec2D Map(Vec2D value, Vec2D fromMin, Vec2D fromMax, Vec2D toMin, Vec2D toMax) =>
        Lerp(toMin, toMax, InverseLerp(value, fromMin, fromMax));

    /// <summary>Returns a vector whose elements are the maximum of each pair-wise.</summary>
    public static Vec2D Max(Vec2D a, Vec2D b) => new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

    /// <summary>Returns a vector whose elements are the minimum of each pair-wise.</summary>
    public static Vec2D Min(Vec2D a, Vec2D b) => new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));

    /// <summary>Gets a new vector reduced to its smallest integer multiple, maintaining direction.</summary>
    public static Vec2D MinCollinear(Vec2D value)
    {
        var maxDivisor = Math.Max(value.X, value.Y);
        for (var k = maxDivisor; k > 1; k--)
        {
            var candidate = value / k;
            if (k * candidate == value)
                return candidate;
        }

        return value;
    }

    /// <summary>Returns a normalized vector, where each part is between -1 and 1.</summary>
    public static Vec2D Normalize(Vec2D value) => new(Math.Sign(value.X), Math.Sign(value.Y));

    /// <summary>Returns a vector that is the rotated clockwise 90° version of <paramref name="value" />.</summary>
    public static Vec2D RotateRight(Vec2D value) => new(value.Y, -value.X);

    /// <summary>Returns a vector that is the rotated counter-clockwise 90° version of <paramref name="value" />.</summary>
    public static Vec2D RotateLeft(Vec2D value) => new(-value.Y, value.X);

    /// <summary>Returns true if <paramref name="value" /> is between <see cref="Zero" /> (inclusive) and max (exclusive).</summary>
    public static bool WithinBounds(Vec2D value, Vec2D max) => WithinBounds(value, 0, max.X, 0, max.Y);

    /// <summary>Returns true if <paramref name="value" /> is between min (inclusive) and max (exclusive).</summary>
    public static bool WithinBounds(Vec2D value, Vec2D min, Vec2D max) =>
        WithinBounds(value, min.X, max.X, min.X, max.Y);

    /// <summary>Returns true if <paramref name="value" /> is between 0 (inclusive) and max (exclusive).</summary>
    public static bool WithinBounds(Vec2D value, int maxX, int maxY) => WithinBounds(value, 0, maxX, 0, maxY);

    /// <summary>Returns true if <paramref name="value" /> is between min (inclusive) and max (exclusive).</summary>
    public static bool WithinBounds(Vec2D value, int minX, int maxX, int minY, int maxY) =>
        value.X >= minX && value.X < maxX && value.Y >= minY && value.Y < maxY;

    /// <summary>Returns true if <paramref name="value" /> is between <see cref="Zero" /> and max (inclusive).</summary>
    public static bool WithinBoundsInclusive(Vec2D value, Vec2D max) => WithinBoundsInclusive(value, Zero, max);

    /// <summary>Returns true if <paramref name="value" /> is between min and max (inclusive).</summary>
    public static bool WithinBoundsInclusive(Vec2D value, Vec2D min, Vec2D max) =>
        WithinBoundsInclusive(value, min.X, max.X, min.Y, max.Y);

    /// <summary>Returns true if <paramref name="value" /> between 0 and max (inclusive).</summary>
    public static bool WithinBoundsInclusive(Vec2D value, int maxX, int maxY) =>
        WithinBoundsInclusive(value, 0, maxX, 0, maxY);

    /// <summary>Returns true if <paramref name="value" /> between min and max (inclusive).</summary>
    public static bool WithinBoundsInclusive(Vec2D value, int minX, int maxX, int minY, int maxY) =>
        value.X >= minX && value.X <= maxX && value.Y >= minY && value.Y <= maxY;

    #endregion

    #region Parsing

    public static Vec2D Parse(string s, IFormatProvider? provider = null) => Parse(s, NumberStyles.Integer, provider);

    public static Vec2D Parse(string s, NumberStyles style, IFormatProvider? provider = null) =>
        string.IsNullOrEmpty(s) ? throw new ArgumentNullException(s) : Parse(s.AsSpan(), style, provider);

    public static Vec2D Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null) =>
        Parse(s, NumberStyles.Integer, provider);

    public static Vec2D Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider = null)
    {
        var trimChars = new ReadOnlySpan<char>(['(', ')', '[', ']', '{', '}']);
        var delimiters = new ReadOnlySpan<char>([',', ';', ' ']);
        s = s.Trim(trimChars);

        Span<Range> ranges = stackalloc Range[2];
        var splitCount = s.SplitAny(ranges, delimiters,
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (splitCount != 2)
            throw new FormatException($"Invalid input: '{s}'.");

        var xPart = s[ranges[0]];
        if (!int.TryParse(xPart, out var x))
            throw new FormatException($"Invalid number format for X: '{xPart.ToString()}'.");

        var yPart = s[ranges[1]];
        if (!int.TryParse(yPart, out var y))
            throw new FormatException($"Invalid number format for Y: '{yPart.ToString()}'.");

        return new Vec2D(x, y);
    }

    public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out Vec2D result) =>
        TryParse(s, out result);

    public static bool TryParse(string? s, IFormatProvider? provider, out Vec2D result) => TryParse(s, out result);

    public static bool TryParse(string? s, out Vec2D result) => string.IsNullOrEmpty(s)
        ? throw new ArgumentNullException(nameof(s))
        : TryParse(s.AsSpan(), out result);

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider,
        out Vec2D result) => TryParse(s, out result);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Vec2D result) =>
        TryParse(s, out result);

    public static bool TryParse(ReadOnlySpan<char> s, out Vec2D result)
    {
        result = default;
        try
        {
            result = Parse(s);
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Formatting

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var s = ToString();
        if (destination.Length < s.Length)
        {
            charsWritten = 0;
            return false;
        }

        s.AsSpan().CopyTo(destination);
        charsWritten = s.Length;
        return true;
    }

    public string ToString([StringSyntax("NumericFormat")] string? format, IFormatProvider? formatProvider) =>
        $"{X.ToString(format, formatProvider)},{Y.ToString(format, formatProvider)}";

    /// <summary>Returns the string representation of the current instance using default formatting: "X,Y".</summary>
    public override string ToString() => $"{X},{Y}";

    #endregion

    #region Operators

    public static Vec2D operator ++(Vec2D value) => new(value.X + 1, value.Y + 1);
    public static Vec2D operator --(Vec2D value) => new(value.X - 1, value.Y - 1);
    public static Vec2D operator +(Vec2D value) => value;
    public static Vec2D operator +(Vec2D left, Vec2D right) => new(left.X + right.X, left.Y + right.Y);
    public static Vec2D operator -(Vec2D value) => new(-value.X, -value.Y);
    public static Vec2D operator -(Vec2D left, Vec2D right) => new(left.X - right.X, left.Y - right.Y);
    public static Vec2D operator *(Vec2D left, int right) => new(left.X * right, left.Y * right);
    public static Vec2D operator *(int left, Vec2D right) => new(left * right.X, left * right.Y);
    public static Vec2D operator *(Vec2D left, Vec2D right) => new(left.X * right.X, left.Y * right.Y);
    public static Vec2D operator /(Vec2D value, int divisor) => new(value.X / divisor, value.Y / divisor);
    public static Vec2D operator /(Vec2D left, Vec2D right) => new(left.X / right.X, left.Y / right.Y);
    public static Vec2D operator %(Vec2D value, int mod) => new(value.X % mod, value.Y % mod);
    public static Vec2D operator %(Vec2D left, Vec2D right) => new(left.X % right.X, left.Y % right.Y);

    #endregion
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace AoC.Utilities.Geometry;

// ReSharper disable InconsistentNaming
/// <summary>
///     A value type containing two longs. All directions are interpreted as (row, col)
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public readonly record struct Vec2DLong(long X, long Y) : ISpanParsable<Vec2DLong>, ISpanFormattable
{
    #region Static Fields

    ///<summary>Gets the vector (1,1).</summary>
    public static readonly Vec2DLong One = new(1, 1);

    ///<summary>Gets the vector (0,0).</summary>
    public static readonly Vec2DLong Zero = new(0, 0);

    /// <summary>Gets the vector (-1,0).</summary>
    public static readonly Vec2DLong N = new(-1, 0);

    /// <summary>Gets the vector (0,1).</summary>
    public static readonly Vec2DLong E = new(0, 1);

    /// <summary>Gets the vector (1,0).</summary>
    public static readonly Vec2DLong S = new(1, 0);

    /// <summary>Gets the vector (0,-1).</summary>
    public static readonly Vec2DLong W = new(0, -1);

    /// <summary>Gets the vector (-1,1).</summary>
    public static readonly Vec2DLong NE = new(-1, 1);

    /// <summary>Gets the vector (1,1).</summary>
    public static readonly Vec2DLong SE = new(1, 1);

    /// <summary>Gets the vector (1,-1).</summary>
    public static readonly Vec2DLong SW = new(1, -1);

    /// <summary>Gets the vector (-1,-1).</summary>
    public static readonly Vec2DLong NW = new(-1, -1);

    /// <summary>Returns the four cardinal directions N, E, S, W in that order.</summary>
    public static readonly Vec2DLong[] CardinalDirs = [N, E, S, W];

    /// <summary>Also called "inter-cardinal directions." Returns the four ordinal directions NE, SE, SW, NW in that order.</summary>
    public static readonly Vec2DLong[] OrdinalDirs = [NE, SE, SW, NW];

    /// <summary>Returns all 8 points of the compass going clockwise from North around to NW.</summary>
    public static readonly Vec2DLong[] AllDirs = [N, NE, E, SE, S, SW, W, NW];

    #endregion

    #region Member Methods

    /// <summary>Returns a vector whose elements are the absolute values of each of the vector's elements.</summary>
    public Vec2DLong Abs() => Abs(this);

    /// <summary>Restricts a vector between a minimum and a maximum value, inclusive.</summary>
    public Vec2DLong Clamp(Vec2DLong min, Vec2DLong max) => Clamp(min.X, max.X, min.Y, max.Y);

    /// <summary>Restricts a vector between a minimum and a maximum value, inclusive.</summary>
    public Vec2DLong Clamp(long minX, long maxX, long minY, long maxY) =>
        new(Math.Clamp(X, minX, maxX), Math.Clamp(Y, minY, maxY));

    /// <summary>Returns the determinant of this and an <paramref name="other" /> vector. Essentially the 2D Cross Product</summary>
    public long Determinant(Vec2DLong other) => Determinant(this, other);

    /// <summary>Computes the Euclidean distance between this and an <paramref name="other" /> vector.</summary>
    public double DistanceEuclidean(Vec2DLong other) => Math.Sqrt(DistanceSquared(other));

    /// <summary>
    ///     Computes the Chebyshev distance, also known as chessboard distance - the amount of moves a king would take to
    ///     get from a to b.
    /// </summary>
    public long DistanceChebyshev(Vec2DLong other) => Math.Max(Math.Abs(other.X - X), Math.Abs(other.Y - Y));

    /// <summary>
    ///     Computes the Manhattan distance (a.k.a. Taxicab distance) between this and an <paramref name="other" /> vector. No
    ///     diagonal moves.
    /// </summary>
    public long DistanceManhattan(Vec2DLong other) => Math.Abs(other.X - X) + Math.Abs(other.Y - Y);

    /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
    public long DistanceSquared(Vec2DLong other) => (other.X - X) * (other.X - X) + (other.Y - Y) * (other.Y - Y);

    /// <summary>Returns the dot product of two vectors.</summary>
    public long Dot(Vec2DLong other) => Dot(this, other);

    /// <summary>
    ///     Returns all points in a rectangle, or line, between <see cref="Zero" /> and this vector, exclusive.
    /// </summary>
    public IEnumerable<Vec2DLong> GeneratePoints(long padding = 0) => GeneratePoints(X, Y, padding);

    /// <summary>
    ///     Returns all points in a rectangle, or line, between this vector (inclusive) and <paramref name="max" />
    ///     (exclusive).
    /// </summary>
    public IEnumerable<Vec2DLong> GeneratePoints(Vec2DLong max, long padding = 0) =>
        GeneratePoints(this, max, padding);

    /// <summary>
    ///     Returns all points in a rectangle, or line, between <see cref="Zero" /> and this vector as the max (both
    ///     inclusive).
    /// </summary>
    public IEnumerable<Vec2DLong> GeneratePointsInclusive(long padding = 0) => GeneratePointsInclusive(X, Y, padding);

    /// <summary>
    ///     Returns all points in a rectangle, or line, between this vector and an <paramref name="other" /> opposite
    ///     corner point, inclusive.
    /// </summary>
    public IEnumerable<Vec2DLong> GeneratePointsInclusive(Vec2DLong other, long padding = 0) =>
        GeneratePointsInclusive(this, other, padding);

    /// <summary>
    ///     Returns all positions in a diamond pattern that is between <paramref name="minDistance" /> (default 0) and
    ///     <paramref name="maxDistance" /> (inclusive) units away from this vector via Manhattan Distance.
    /// </summary>
    /// <param name="maxDistance">Should be greater than or equal to minDistance</param>
    /// <param name="minDistance">Should be greater than or equal to zero (default)</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public IEnumerable<Vec2DLong> GetDiamondPattern(long maxDistance, long minDistance = 0) =>
        GetDiamondPattern(this, maxDistance, minDistance);

    /// <summary>Determines if the two vectors are right next to each other laterally.</summary>
    public bool IsAdjacentTo(Vec2DLong other) => DistanceManhattan(other) == 1;

    /// <summary>
    ///     When treating X and Y as values that are proportional to each other (e.g. imagine a fraction X/Y), this
    ///     returns true if the two values cannot be reduced any further. For example, if this were a direction vector,
    ///     then this is true when the values are in their smallest magnitude while maintaining the same direction.
    /// </summary>
    public bool IsCanonical() => MinCollinear(this) == this;

    /// <summary>Determines if the two vectors are along the same diagonal (same X and Y distance apart).</summary>
    public bool IsDiagonalTo(Vec2DLong other) => Math.Abs(other.X - X) == Math.Abs(other.Y - Y);

    /// <summary>Horizontally or Vertically aligned at any distance, but not the same position</summary>
    public bool IsLateralTo(Vec2DLong other) => (other.X == X) ^ (other.Y == Y);

    /// <summary>
    ///     When treated as directions, determines if the two are parallel and neither are <see cref="Zero" />
    /// </summary>
    public bool IsParallelTo(Vec2DLong other) =>
        this != Zero && other != Zero && MinCollinear(this) == MinCollinear(other);

    /// <summary>
    ///     When treated as directions, determines if the two are perpendicular and neither are <see cref="Zero" />
    /// </summary>
    public bool IsPerpendicularTo(Vec2DLong other) => this != Zero && other != Zero && Dot(this, other) == 0;

    /// <summary>Returns true if this vector is between <see cref="Zero" /> (inclusive) and max (exclusive).</summary>
    public bool IsWithinBounds(Vec2DLong max) => WithinBounds(this, Zero, max);

    /// <summary>Returns true if this vector is between min (inclusive) and max (exclusive).</summary>
    public bool IsWithinBounds(Vec2DLong min, Vec2DLong max) => WithinBounds(this, min, max);

    /// <summary>Returns true if this vector is between 0 (inclusive) and max (exclusive).</summary>
    public bool IsWithinBounds(long maxX, long maxY) => WithinBounds(this, 0, maxX, 0, maxY);

    /// <summary>Returns true if this vector is between min x and y (inclusive) and max x and y (exclusive).</summary>
    public bool IsWithinBounds(long minX, long maxX, long minY, long maxY) =>
        WithinBounds(this, minX, maxX, minY, maxY);

    /// <summary>Returns true if this vector is between <see cref="Zero" />) and max (inclusive).</summary>
    public bool IsWithinBoundsInclusive(Vec2DLong max) => WithinBoundsInclusive(this, Zero, max);

    /// <summary>Returns true if this vector is between min and max (inclusive).</summary>
    public bool IsWithinBoundsInclusive(Vec2DLong min, Vec2DLong max) => WithinBoundsInclusive(this, min, max);

    /// <summary>Returns true if this vector is between 0 and max (inclusive).</summary>
    public bool IsWithinBoundsInclusive(long maxX, long maxY) => WithinBoundsInclusive(this, 0, maxX, 0, maxY);

    /// <summary>Returns true if this vector is between min and max (inclusive).</summary>
    public bool IsWithinBoundsInclusive(long minX, long maxX, long minY, long maxY) =>
        WithinBoundsInclusive(this, minX, maxX, minY, maxY);

    /// <summary>Returns the length, or magnitude, of the vector (a.k.a. Euclidean Distance from origin).</summary>
    public double Length() => Length(this);

    /// <summary>Returns the squared length, or magnitude, of the vector.</summary>
    public long LengthSquared() => LengthSquared(this);

    /// <summary>Returns a vector whose elements are the maximum of each pair-wise.</summary>
    public Vec2DLong Max(Vec2DLong other) => Max(this, other);

    /// <summary>Returns a vector whose elements are the minimum of each pair-wise.</summary>
    public Vec2DLong Min(Vec2DLong other) => Min(this, other);

    /// <summary>Gets a new vector reduced to its smallest integer multiple, maintaining direction.</summary>
    public Vec2DLong MinCollinear() => MinCollinear(this);

    /// <summary>Returns a normalized vector, where each part is between -1 and 1.</summary>
    public Vec2DLong Normalized() => Normalize(this);

    /// <summary>Returns a vector that is the rotated clockwise 90° version of this vector.</summary>
    public Vec2DLong RotatedRight() => RotateRight(this);

    /// <summary>Returns a vector that is the rotated counter-clockwise 90° version of this vector.</summary>
    public Vec2DLong RotatedLeft() => RotateLeft(this);

    #endregion

    #region Static Methods

    /// <summary>Returns a vector whose elements are the absolute values of each of the <paramref name="value" />'s elements.</summary>
    public static Vec2DLong Abs(Vec2DLong value) => new(Math.Abs(value.X), Math.Abs(value.Y));

    /// <summary>Returns the determinant of two vectors. Essentially the 2D Cross Product.</summary>
    public static long Determinant(Vec2DLong a, Vec2DLong b) => a.X * b.Y - a.Y * b.X;

    /// <summary>Returns the dot product of two vectors.</summary>
    public static long Dot(Vec2DLong a, Vec2DLong b) => a.X * b.X + a.Y * b.Y;

    /// <summary>Returns all points in a rectangle, or line, between min (inclusive) and max (exclusive).</summary>
    public static IEnumerable<Vec2DLong> GeneratePoints(Vec2DLong min, Vec2DLong max, long padding = 0) =>
        GeneratePoints(min.X, max.X, min.Y, max.Y, padding);

    /// <summary>Returns all points in a rectangle, or line, between 0 (inclusive) and max (exclusive).</summary>
    public static IEnumerable<Vec2DLong> GeneratePoints(long maxX, long maxY, long padding = 0) =>
        GeneratePoints(0, maxX, 0, maxY, padding);

    /// <summary>Returns all points in a rectangle, or line, between min (inclusive) and max (exclusive).</summary>
    public static IEnumerable<Vec2DLong> GeneratePoints(long minX, long maxX, long minY, long maxY, long padding = 0)
    {
        for (var x = minX + padding; x < maxX - padding; x++)
            for (var y = minY + padding; y < maxY - padding; y++)
                yield return new Vec2DLong(x, y);
    }

    /// <summary>Returns all points in a rectangle, or line, between two vectors (inclusive).</summary>
    public static IEnumerable<Vec2DLong> GeneratePointsInclusive(Vec2DLong a, Vec2DLong b, long padding = 0)
    {
        long minX = Math.Min(a.X, b.X), maxX = Math.Max(a.X, b.X);
        long minY = Math.Min(a.Y, b.Y), maxY = Math.Max(a.Y, b.Y);
        return GeneratePointsInclusive(minX, maxX, minY, maxY, padding);
    }

    /// <summary>Returns all points in a rectangle, or line, between 0 and max (both inclusive).</summary>
    public static IEnumerable<Vec2DLong> GeneratePointsInclusive(long maxX, long maxY, long padding = 0) =>
        GeneratePointsInclusive(0, maxX, 0, maxY, padding);

    /// <summary>Returns all points in a rectangle, or line, between min and max (both inclusive).</summary>
    public static IEnumerable<Vec2DLong> GeneratePointsInclusive(long minX, long maxX, long minY, long maxY,
        long padding = 0)
    {
        for (var x = minX + padding; x <= maxX - padding; x++)
            for (var y = minY + padding; y <= maxY - padding; y++)
                yield return new Vec2DLong(x, y);
    }

    /// <summary>
    ///     Returns all positions in a diamond pattern that is between <paramref name="minDistance" /> (default 0) and
    ///     <paramref name="maxDistance" /> (inclusive) units away from <paramref name="center" /> via Manhattan Distance.
    /// </summary>
    /// <param name="center">Center of the diamond pattern</param>
    /// <param name="maxDistance">Should be greater than or equal to minDistance</param>
    /// <param name="minDistance">Should be greater than or equal to zero (default)</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IEnumerable<Vec2DLong> GetDiamondPattern(Vec2DLong center, long maxDistance, long minDistance = 0)
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
                yield return new Vec2DLong(center.X + x, center.Y + y);

            for (var y = minY; y <= maxY; y++)
                yield return new Vec2DLong(center.X + x, center.Y + y);
        }
    }

    /// <summary>
    ///     Returns a value as the percentage that <paramref name="value" /> is between <paramref name="a" /> and
    ///     <paramref name="b" />. Not clamped: may return values outside the 0 to 1 range.
    /// </summary>
    public static double InverseLerp(Vec2DLong value, Vec2DLong a, Vec2DLong b)
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
    public static double InverseLerpClamped(Vec2DLong value, Vec2DLong a, Vec2DLong b) =>
        Math.Clamp(InverseLerp(value, a, b), 0, 1);

    /// <summary>Returns the length, or magnitude, of the vector <paramref name="value" />.</summary>
    public static double Length(Vec2DLong value) => Math.Sqrt(LengthSquared(value));

    /// <summary>Returns the squared length, or magnitude, of the vector <paramref name="value" />.</summary>
    public static long LengthSquared(Vec2DLong value) => value.X * value.X + value.Y * value.Y;

    /// <summary>Performs a linear interpolation between two vectors based on the given weighting (0f to 1f).</summary>
    public static Vec2DLong Lerp(Vec2DLong a, Vec2DLong b, double weight)
    {
        var x = (int)Math.Round(a.X + (b.X - a.X) * weight);
        var y = (int)Math.Round(a.Y + (b.Y - a.Y) * weight);
        return new Vec2DLong(x, y);
    }

    /// <summary>
    ///     Given two lines (each as a directions and a point in space it travels through), determine if they intersect
    ///     and output the <paramref name="intersection" /> point (or closest approximation given integer limitations).
    /// </summary>
    public static bool LineIntersect(Vec2DLong pt1, Vec2DLong dir1, Vec2DLong pt2, Vec2DLong dir2,
        out Vec2DLong intersection)
    {
        intersection = Zero;
        var det = Determinant(dir2, dir1);
        if (det == 0) return false;

        double quotient1 = dir2.X * (pt2.Y - pt1.Y) - dir2.Y * (pt2.X - pt1.X);
        var t = quotient1 / det;
        intersection = new Vec2DLong((long)Math.Round(pt1.X + t * dir1.X), (long)Math.Round(pt1.Y + t * dir1.Y));

        double quotient2 = dir1.X * (pt1.Y - pt2.Y) - dir1.Y * (pt1.X - pt2.X);
        var s = -quotient2 / det;
        var intersection2 = new Vec2DLong((long)Math.Round(pt2.X + s * dir2.X), (long)Math.Round(pt2.Y + s * dir2.Y));

        return intersection == intersection2;
    }

    /// <summary>Re-maps a vector from one range to another.</summary>
    public static Vec2DLong Map(Vec2DLong value, Vec2DLong fromMin, Vec2DLong fromMax, Vec2DLong toMin,
        Vec2DLong toMax) => Lerp(toMin, toMax, InverseLerp(value, fromMin, fromMax));

    /// <summary>Returns a vector whose elements are the maximum of each pair-wise.</summary>
    public static Vec2DLong Max(Vec2DLong a, Vec2DLong b) => new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

    /// <summary>Returns a vector whose elements are the minimum of each pair-wise.</summary>
    public static Vec2DLong Min(Vec2DLong a, Vec2DLong b) => new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));

    /// <summary>Gets a new vector reduced to its smallest integer multiple, maintaining direction.</summary>
    public static Vec2DLong MinCollinear(Vec2DLong value)
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
    public static Vec2DLong Normalize(Vec2DLong value) => new(Math.Sign(value.X), Math.Sign(value.Y));

    /// <summary>Returns a vector that is the rotated clockwise 90° version of <paramref name="value" />.</summary>
    public static Vec2DLong RotateRight(Vec2DLong value) => new(value.Y, -value.X);

    /// <summary>Returns a vector that is the rotated counter-clockwise 90° version of <paramref name="value" />.</summary>
    public static Vec2DLong RotateLeft(Vec2DLong value) => new(-value.Y, value.X);

    /// <summary>Returns true if <paramref name="value" /> is between <see cref="Zero" /> (inclusive) and max (exclusive).</summary>
    public static bool WithinBounds(Vec2DLong value, Vec2DLong max) => WithinBounds(value, 0, max.X, 0, max.Y);

    /// <summary>Returns true if <paramref name="value" /> is between min (inclusive) and max (exclusive).</summary>
    public static bool WithinBounds(Vec2DLong value, Vec2DLong min, Vec2DLong max) =>
        WithinBounds(value, min.X, max.X, min.Y, max.Y);

    /// <summary>Returns true if <paramref name="value" /> is between 0 (inclusive) and max (exclusive).</summary>
    public static bool WithinBounds(Vec2DLong value, long maxX, long maxY) =>
        WithinBounds(value, 0, maxX, 0, maxY);

    /// <summary>Returns true if <paramref name="value" /> is between min (inclusive) and max (exclusive).</summary>
    public static bool WithinBounds(Vec2DLong value, long minX, long maxX, long minY, long maxY) =>
        value.X >= minX && value.X < maxX && value.Y >= minY && value.Y < maxY;

    /// <summary>Returns true if <paramref name="value" /> is between <see cref="Zero" /> and max (inclusive).</summary>
    public static bool WithinBoundsInclusive(Vec2DLong value, Vec2DLong max) => WithinBoundsInclusive(value, Zero, max);

    /// <summary>Returns true if <paramref name="value" /> is between min and max (inclusive).</summary>
    public static bool WithinBoundsInclusive(Vec2DLong value, Vec2DLong min, Vec2DLong max) =>
        WithinBoundsInclusive(value, min.X, max.X, min.Y, max.Y);

    /// <summary>Returns true if <paramref name="value" /> between 0 and max (inclusive).</summary>
    public static bool WithinBoundsInclusive(Vec2DLong value, long maxX, long maxY) =>
        WithinBoundsInclusive(value, 0, maxX, 0, maxY);

    /// <summary>Returns true if <paramref name="value" /> between min and max (inclusive).</summary>
    public static bool WithinBoundsInclusive(Vec2DLong value, long minX, long maxX, long minY, long maxY) =>
        value.X >= minX && value.X <= maxX && value.Y >= minY && value.Y <= maxY;

    #endregion

    #region Parsing

    public static Vec2DLong Parse(string s, IFormatProvider? provider = null) =>
        Parse(s, NumberStyles.Integer, provider);

    public static Vec2DLong Parse(string s, NumberStyles style, IFormatProvider? provider = null) =>
        string.IsNullOrEmpty(s) ? throw new ArgumentNullException(s) : Parse(s.AsSpan(), style, provider);

    public static Vec2DLong Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null) =>
        Parse(s, NumberStyles.Integer, provider);

    public static Vec2DLong Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider = null)
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

        return new Vec2DLong(x, y);
    }

    public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out Vec2DLong result) =>
        TryParse(s, out result);

    public static bool TryParse(string? s, IFormatProvider? provider, out Vec2DLong result) => TryParse(s, out result);

    public static bool TryParse(string? s, out Vec2DLong result) => string.IsNullOrEmpty(s)
        ? throw new ArgumentNullException(nameof(s))
        : TryParse(s.AsSpan(), out result);

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider,
        out Vec2DLong result) => TryParse(s, out result);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Vec2DLong result) =>
        TryParse(s, out result);

    public static bool TryParse(ReadOnlySpan<char> s, out Vec2DLong result)
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

    public static Vec2DLong operator ++(Vec2DLong value) => new(value.X + 1, value.Y + 1);
    public static Vec2DLong operator --(Vec2DLong value) => new(value.X - 1, value.Y - 1);
    public static Vec2DLong operator +(Vec2DLong value) => value;
    public static Vec2DLong operator +(Vec2DLong left, Vec2DLong right) => new(left.X + right.X, left.Y + right.Y);
    public static Vec2DLong operator -(Vec2DLong value) => new(-value.X, -value.Y);
    public static Vec2DLong operator -(Vec2DLong left, Vec2DLong right) => new(left.X - right.X, left.Y - right.Y);
    public static Vec2DLong operator *(long left, Vec2DLong right) => new(left * right.X, left * right.Y);
    public static Vec2DLong operator *(Vec2DLong left, long right) => new(left.X * right, left.Y * right);
    public static Vec2DLong operator *(Vec2DLong left, Vec2DLong right) => new(left.X * right.X, left.Y * right.Y);
    public static Vec2DLong operator /(Vec2DLong value, long divisor) => new(value.X / divisor, value.Y / divisor);
    public static Vec2DLong operator /(Vec2DLong left, Vec2DLong right) => new(left.X / right.X, left.Y / right.Y);
    public static Vec2DLong operator %(Vec2DLong value, long mod) => new(value.X % mod, value.Y % mod);
    public static Vec2DLong operator %(Vec2DLong left, Vec2DLong right) => new(left.X % right.X, left.Y % right.Y);

    #endregion
}
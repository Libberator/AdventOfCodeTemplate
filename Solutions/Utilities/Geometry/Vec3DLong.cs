using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace AoC.Utilities.Geometry;

/// <summary>
///     A value type containing three longs. Directions are interpreted as a left-handed coordinate system with Y-up.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public readonly record struct Vec3DLong(long X, long Y, long Z) : ISpanParsable<Vec3DLong>, ISpanFormattable
{
    #region Static Fields

    ///<summary>Gets the vector (1,1,1).</summary>
    public static readonly Vec3DLong One = new(1, 1, 1);

    ///<summary>Gets the vector (0,0,0).</summary>
    public static readonly Vec3DLong Zero = new(0, 0, 0);

    /// <summary>Gets the vector (1,0,0).</summary>
    public static readonly Vec3DLong Right = new(1, 0, 0);

    /// <summary>Gets the vector (0,1,0).</summary>
    public static readonly Vec3DLong Up = new(0, 1, 0);

    /// <summary>Gets the vector (0,0,1).</summary>
    public static readonly Vec3DLong Forward = new(0, 0, 1);

    /// <summary>Gets the vector (-1,0,0).</summary>
    public static readonly Vec3DLong Left = new(-1, 0, 0);

    /// <summary>Gets the vector (0,-1,0).</summary>
    public static readonly Vec3DLong Down = new(0, -1, 0);

    /// <summary>Gets the vector (0,0,-1).</summary>
    public static readonly Vec3DLong Backward = new(0, 0, -1);

    /// <summary>Returns the 6 base directions which affect 1 axis (i.e. the "centers" of a Rubik's cube).</summary>
    public static readonly Vec3DLong[] PrimaryDirs = [Right, Up, Forward, Left, Down, Backward];

    /// <summary>Returns the 12 base directions which affect 2 axes (i.e. the "edges" of a Rubik's cube).</summary>
    public static readonly Vec3DLong[] SecondaryDirs =
    [
        new(1, 1, 0), new(0, 1, 1), new(-1, 1, 0), new(0, 1, -1), // top row
        new(1, 0, 1), new(-1, 0, 1), new(-1, 0, -1), new(1, 0, -1), // middle row
        new(1, -1, 0), new(0, -1, 1), new(-1, -1, 0), new(0, -1, -1) // bottom row
    ];

    /// <summary>Returns the 8 base directions which affect 3 axes (i.e. the "corners" of a Rubik's cube).</summary>
    public static readonly Vec3DLong[] TertiaryDirs =
    [
        new(1, 1, 1), new(-1, 1, 1), new(-1, 1, -1), new(1, 1, -1), // top row
        new(1, -1, 1), new(-1, -1, 1), new(-1, -1, -1), new(1, -1, -1) // bottom row
    ];

    /// <summary>Returns all 26 base directions (i.e. direction to all cubes on a Rubik's cube from the origin).</summary>
    public static readonly Vec3DLong[] AllDirs = [..PrimaryDirs, ..SecondaryDirs, ..TertiaryDirs];

    #endregion

    #region Member Methods

    /// <summary>Returns a vector whose elements are the absolute values of each of the vector's elements.</summary>
    public Vec3DLong Abs() => Abs(this);

    /// <summary>Restricts a vector between a minimum and a maximum value, inclusive.</summary>
    public Vec3DLong Clamp(Vec3DLong min, Vec3DLong max) => Clamp(min.X, max.X, min.Y, max.Y, min.Z, max.Z);

    /// <summary>Restricts a vector between a minimum and a maximum value, inclusive.</summary>
    public Vec3DLong Clamp(long minX, long maxX, long minY, long maxY, long minZ, long maxZ) =>
        new(Math.Clamp(X, minX, maxX), Math.Clamp(Y, minY, maxY), Math.Clamp(Z, minZ, maxZ));

    /// <summary>Returns the Cross Product of this and an <paramref name="other" /> vector.</summary>
    public Vec3DLong Cross(Vec3DLong other) => Cross(this, other);

    /// <summary>Computes the Euclidean distance between this and an <paramref name="other" /> vector.</summary>
    public double DistanceEuclidean(Vec3DLong other) => Math.Sqrt(DistanceSquared(other));

    /// <summary>
    ///     Computes the Chebyshev distance, also known as chessboard distance - the amount of moves a king would take to
    ///     get from a to b.
    /// </summary>
    public long DistanceChebyshev(Vec3DLong other) =>
        Math.Max(Math.Max(Math.Abs(other.X - X), Math.Abs(other.Y - Y)), Math.Abs(other.Z - Z));

    /// <summary>
    ///     Computes the Manhattan distance (a.k.a. Taxicab distance) between this and an <paramref name="other" /> vector. No
    ///     diagonal moves.
    /// </summary>
    public long DistanceManhattan(Vec3DLong other) =>
        Math.Abs(other.X - X) + Math.Abs(other.Y - Y) + Math.Abs(other.Z - Z);

    /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
    public long DistanceSquared(Vec3DLong other) =>
        (other.X - X) * (other.X - X) + (other.Y - Y) * (other.Y - Y) + (other.Z - Z) * (other.Z - Z);

    /// <summary>Returns the dot product of two vectors.</summary>
    public long Dot(Vec3DLong other) => Dot(this, other);

    /// <summary>Returns all points in a box between <see cref="Zero" /> and this vector, exclusive.</summary>
    public IEnumerable<Vec3DLong> GeneratePoints(long padding = 0) => GeneratePoints(X, Y, Z, padding);

    /// <summary>Returns all points in a box between this vector (inclusive) and <paramref name="max" /> (exclusive).</summary>
    public IEnumerable<Vec3DLong> GeneratePoints(Vec3DLong max, long padding = 0) => GeneratePoints(this, max, padding);

    /// <summary>
    ///     Returns all points in a box between <see cref="Zero" /> and this vector as the max (both
    ///     inclusive).
    /// </summary>
    public IEnumerable<Vec3DLong> GeneratePointsInclusive(long padding = 0) =>
        GeneratePointsInclusive(X, Y, Z, padding);

    /// <summary>
    ///     Returns all points in a box between this vector and an <paramref name="other" /> opposite
    ///     corner point, inclusive.
    /// </summary>
    public IEnumerable<Vec3DLong> GeneratePointsInclusive(Vec3DLong other, long padding = 0) =>
        GeneratePointsInclusive(this, other, padding);

    /// <summary>Determines if the two vectors are right next to each other laterally.</summary>
    public bool IsAdjacentTo(Vec3DLong other) => DistanceManhattan(other) == 1;

    /// <summary>
    ///     When treating X, Y, and Z as values that are proportional to each other (e.g. imagine a fraction X/Y and Y/Z),
    ///     this returns true if the values cannot be reduced any further. For example, if this were a direction vector,
    ///     then this is true when the values are in their smallest magnitude while maintaining the same direction.
    /// </summary>
    public bool IsCanonical() => MinCollinear(this) == this;

    /// <summary>
    ///     Determines if the two vectors are along the same diagonal (i.e. any number of units of any single
    ///     <see cref="TertiaryDirs" /> apart).
    /// </summary>
    public bool IsDiagonalTo(Vec3DLong other) =>
        Math.Abs(other.X - X) == Math.Abs(other.Y - Y) && Math.Abs(other.Y - Y) == Math.Abs(other.Z - Z);

    /// <summary>Horizontally, Vertically, or Depth-wise aligned at any distance, but not the same position</summary>
    public bool IsLateralTo(Vec3DLong other) => new[] { other.X == X, other.Y == Y, other.Z == Z }.Count(b => b) == 1;

    /// <summary>
    ///     When treated as directions, determines if the two are parallel and neither are <see cref="Zero" />
    /// </summary>
    public bool IsParallelTo(Vec3DLong other) =>
        this != Zero && other != Zero && MinCollinear(this) == MinCollinear(other);

    /// <summary>
    ///     When treated as directions, determines if the two are perpendicular and neither are <see cref="Zero" />
    /// </summary>
    public bool IsPerpendicularTo(Vec3DLong other) => this != Zero && other != Zero && Dot(this, other) == 0;

    /// <summary>Returns true if this vector is between <see cref="Zero" /> (inclusive) and max (exclusive).</summary>
    public bool IsWithinBounds(Vec3DLong max) => WithinBounds(this, Zero, max);

    /// <summary>Returns true if this vector is between min (inclusive) and max (exclusive).</summary>
    public bool IsWithinBounds(Vec3DLong min, Vec3DLong max) => WithinBounds(this, min, max);

    /// <summary>Returns true if this vector is between 0 (inclusive) and max (exclusive).</summary>
    public bool IsWithinBounds(long maxX, long maxY, long maxZ) =>
        WithinBounds(this, 0, maxX, 0, maxY, 0, maxZ);

    /// <summary>Returns true if this vector is between min (inclusive) and max (exclusive).</summary>
    public bool IsWithinBounds(long minX, long maxX, long minY, long maxY, long minZ, long maxZ) =>
        WithinBounds(this, minX, maxX, minY, maxY, minZ, maxZ);

    /// <summary>Returns true if this vector is between <see cref="Zero" /> and max (inclusive).</summary>
    public bool IsWithinBoundsInclusive(Vec3DLong max) => WithinBoundsInclusive(this, Zero, max);

    /// <summary>Returns true if this vector is between min and max (inclusive).</summary>
    public bool IsWithinBoundsInclusive(Vec3DLong min, Vec3DLong max) => WithinBoundsInclusive(this, min, max);

    /// <summary>Returns true if this vector is between 0 and max (inclusive).</summary>
    public bool IsWithinBoundsInclusive(long maxX, long maxY, long maxZ) =>
        WithinBoundsInclusive(this, 0, maxX, 0, maxY, 0, maxZ);

    /// <summary>Returns true if this vector is between min and max (inclusive).</summary>
    public bool IsWithinBoundsInclusive(long minX, long maxX, long minY, long maxY, long minZ, long maxZ) =>
        WithinBoundsInclusive(this, minX, maxX, minY, maxY, minZ, maxZ);

    /// <summary>Returns the length, or magnitude, of the vector (a.k.a. Euclidean Distance from origin).</summary>
    public double Length() => Length(this);

    /// <summary>Returns the squared length, or magnitude, of the vector.</summary>
    public long LengthSquared() => LengthSquared(this);

    /// <summary>Returns a vector whose elements are the maximum of each pair-wise.</summary>
    public Vec3DLong Max(Vec3DLong other) => Max(this, other);

    /// <summary>Returns a vector whose elements are the minimum of each pair-wise.</summary>
    public Vec3DLong Min(Vec3DLong other) => Min(this, other);

    /// <summary>Gets a new vector reduced to its smallest integer multiple, maintaining direction.</summary>
    public Vec3DLong MinCollinear() => MinCollinear(this);

    /// <summary>Returns a normalized vector, where each part is between -1 and 1.</summary>
    public Vec3DLong Normalized() => Normalize(this);

    #endregion

    #region Static Methods

    /// <summary>Returns a vector whose elements are the absolute values of each of the <paramref name="value" />'s elements.</summary>
    public static Vec3DLong Abs(Vec3DLong value) => new(Math.Abs(value.X), Math.Abs(value.Y), Math.Abs(value.Z));

    /// <summary>Returns the Cross Product of two vectors.</summary>
    public static Vec3DLong Cross(Vec3DLong a, Vec3DLong b) =>
        new(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);

    /// <summary>Returns the dot product of two vectors.</summary>
    public static long Dot(Vec3DLong a, Vec3DLong b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    /// <summary>Returns all points in a box between min (inclusive) and max (exclusive).</summary>
    public static IEnumerable<Vec3DLong> GeneratePoints(Vec3DLong min, Vec3DLong max, long padding = 0) =>
        GeneratePoints(min.X, max.X, min.Y, max.Y, min.Z, max.Z, padding);

    /// <summary>Returns all points in a rectangle, or line, between the min (inclusive) and max (exclusive).</summary>
    public static IEnumerable<Vec3DLong> GeneratePoints(long maxX, long maxY, long maxZ, long padding = 0) =>
        GeneratePoints(0, maxX, 0, maxY, 0, maxZ, padding);

    /// <summary>Returns all points in a rectangle, or line, between the min (inclusive) and max (exclusive).</summary>
    public static IEnumerable<Vec3DLong> GeneratePoints(long minX, long maxX, long minY, long maxY, long minZ,
        long maxZ, long padding = 0)
    {
        for (var x = minX + padding; x < maxX - padding; x++)
            for (var y = minY + padding; y < maxY - padding; y++)
                for (var z = minZ + padding; z < maxZ - padding; z++)
                    yield return new Vec3DLong(x, y, z);
    }

    /// <summary>Returns all points in a box between two vectors (inclusive).</summary>
    public static IEnumerable<Vec3DLong> GeneratePointsInclusive(Vec3DLong a, Vec3DLong b, long padding = 0)
    {
        long minX = Math.Min(a.X, b.X), maxX = Math.Max(a.X, b.X);
        long minY = Math.Min(a.Y, b.Y), maxY = Math.Max(a.Y, b.Y);
        long minZ = Math.Min(a.Z, b.Z), maxZ = Math.Max(a.Z, b.Z);
        return GeneratePointsInclusive(minX, maxX, minY, maxY, minZ, maxZ, padding);
    }

    /// <summary>Returns all points in a box between 0 and max (both inclusive).</summary>
    public static IEnumerable<Vec3DLong> GeneratePointsInclusive(long maxX, long maxY, long maxZ, long padding = 0) =>
        GeneratePointsInclusive(0, maxX, 0, maxY, 0, maxZ, padding);

    /// <summary>Returns all points in a box between the min and max (both inclusive).</summary>
    public static IEnumerable<Vec3DLong> GeneratePointsInclusive(long minX, long maxX, long minY, long maxY, long minZ,
        long maxZ,
        long padding = 0)
    {
        for (var x = minX + padding; x <= maxX - padding; x++)
            for (var y = minY + padding; y <= maxY - padding; y++)
                for (var z = minZ + padding; z <= maxZ - padding; z++)
                    yield return new Vec3DLong(x, y, z);
    }

    /// <summary>
    ///     Returns a value as the percentage that <paramref name="value" /> is between <paramref name="a" /> and
    ///     <paramref name="b" />. Not clamped: may return values outside the 0 to 1 range.
    /// </summary>
    public static double InverseLerp(Vec3DLong value, Vec3DLong a, Vec3DLong b)
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
    public static double InverseLerpClamped(Vec3DLong value, Vec3DLong a, Vec3DLong b) =>
        Math.Clamp(InverseLerp(value, a, b), 0, 1);

    /// <summary>Returns the length, or magnitude, of the vector <paramref name="value" />.</summary>
    public static double Length(Vec3DLong value) => Math.Sqrt(LengthSquared(value));

    /// <summary>Returns the squared length, or magnitude, of the vector <paramref name="value" />.</summary>
    public static long LengthSquared(Vec3DLong value) => value.X * value.X + value.Y * value.Y;

    /// <summary>Performs a linear interpolation between two vectors based on the given weighting (0f to 1f).</summary>
    public static Vec3DLong Lerp(Vec3DLong a, Vec3DLong b, double weight)
    {
        var x = (long)Math.Round(a.X + (b.X - a.X) * weight);
        var y = (long)Math.Round(a.Y + (b.Y - a.Y) * weight);
        var z = (long)Math.Round(a.Z + (b.Z - a.Z) * weight);
        return new Vec3DLong(x, y, z);
    }

    /// <summary>Re-maps a vector from one range to another.</summary>
    public static Vec3DLong Map(Vec3DLong value, Vec3DLong fromMin, Vec3DLong fromMax, Vec3DLong toMin,
        Vec3DLong toMax) => Lerp(toMin, toMax, InverseLerp(value, fromMin, fromMax));

    /// <summary>Returns a vector whose elements are the maximum of each pair-wise.</summary>
    public static Vec3DLong Max(Vec3DLong a, Vec3DLong b) =>
        new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

    /// <summary>Returns a vector whose elements are the minimum of each pair-wise.</summary>
    public static Vec3DLong Min(Vec3DLong a, Vec3DLong b) =>
        new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));

    /// <summary>Gets a new vector reduced to its smallest integer multiple, maintaining direction.</summary>
    public static Vec3DLong MinCollinear(Vec3DLong value)
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
    public static Vec3DLong Normalize(Vec3DLong value) =>
        new(Math.Sign(value.X), Math.Sign(value.Y), Math.Sign(value.Z));

    /// <summary>Returns true if <paramref name="value" /> is between <see cref="Zero" /> (inclusive) and max (exclusive).</summary>
    public static bool WithinBounds(Vec3DLong value, Vec3DLong max) =>
        WithinBounds(value, 0, max.X, 0, max.Y, 0, max.Z);

    /// <summary>Returns true if <paramref name="value" /> is between min (inclusive) and max (exclusive).</summary>
    public static bool WithinBounds(Vec3DLong value, Vec3DLong min, Vec3DLong max) =>
        WithinBounds(value, min.X, max.X, min.X, max.Y, min.Z, max.Z);

    /// <summary>Returns true if <paramref name="value" /> is between min (inclusive) and max (exclusive).</summary>
    public static bool WithinBounds(Vec3DLong value, long maxX, long maxY, long maxZ) =>
        WithinBounds(value, 0, maxX, 0, maxY, 0, maxZ);

    /// <summary>Returns true if <paramref name="value" /> is between min (inclusive) and max (exclusive).</summary>
    public static bool WithinBounds(Vec3DLong value, long minX, long maxX, long minY, long maxY, long minZ,
        long maxZ) =>
        value.X >= minX && value.X < maxX && value.Y >= minY && value.Y < maxY && value.Z >= minZ && value.Z < maxZ;

    /// <summary>Returns true if <paramref name="value" /> is between <see cref="Zero" /> and max (inclusive).</summary>
    public static bool WithinBoundsInclusive(Vec3DLong value, Vec3DLong max) => WithinBoundsInclusive(value, Zero, max);

    /// <summary>Returns true if <paramref name="value" /> is between min and max (inclusive).</summary>
    public static bool WithinBoundsInclusive(Vec3DLong value, Vec3DLong min, Vec3DLong max) =>
        WithinBoundsInclusive(value, min.X, max.X, min.Y, max.Y, min.Z, max.Z);

    /// <summary>Returns true if <paramref name="value" /> between 0 and max x and y (inclusive).</summary>
    public static bool WithinBoundsInclusive(Vec3DLong value, long maxX, long maxY, long maxZ) =>
        WithinBoundsInclusive(value, 0, maxX, 0, maxY, 0, maxZ);

    /// <summary>Returns true if <paramref name="value" /> between min and max (inclusive).</summary>
    public static bool WithinBoundsInclusive(Vec3DLong value, long minX, long maxX, long minY, long maxY, long minZ,
        long maxZ) =>
        value.X >= minX && value.X <= maxX && value.Y >= minY && value.Y <= maxY && value.Z >= minZ && value.Z <= maxZ;

    #endregion

    #region Parsing

    public static Vec3DLong Parse(string s, IFormatProvider? provider = null) =>
        Parse(s, NumberStyles.Integer, provider);

    public static Vec3DLong Parse(string s, NumberStyles style, IFormatProvider? provider = null) =>
        string.IsNullOrEmpty(s) ? throw new ArgumentNullException(s) : Parse(s.AsSpan(), style, provider);

    public static Vec3DLong Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null) =>
        Parse(s, NumberStyles.Integer, provider);

    public static Vec3DLong Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider = null)
    {
        var trimChars = new ReadOnlySpan<char>(['(', ')', '[', ']', '{', '}', ' ']);
        var delimiters = new ReadOnlySpan<char>([',', ';', ' ']);
        s = s.Trim(trimChars);

        Span<Range> ranges = stackalloc Range[3];
        var splitCount = s.SplitAny(ranges, delimiters,
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (splitCount != 3)
            throw new FormatException($"Invalid input: '{s}'.");

        var xPart = s[ranges[0]];
        if (!long.TryParse(xPart, out var x))
            throw new FormatException($"Invalid number format for X: '{xPart.ToString()}'.");

        var yPart = s[ranges[1]];
        if (!long.TryParse(yPart, out var y))
            throw new FormatException($"Invalid number format for Y: '{yPart.ToString()}'.");

        var zPart = s[ranges[2]];
        if (!long.TryParse(zPart, out var z))
            throw new FormatException($"Invalid number format for Z: '{zPart.ToString()}'.");

        return new Vec3DLong(x, y, z);
    }

    public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out Vec3DLong result) =>
        TryParse(s, out result);

    public static bool TryParse(string? s, IFormatProvider? provider, out Vec3DLong result) => TryParse(s, out result);

    public static bool TryParse(string? s, out Vec3DLong result) => string.IsNullOrEmpty(s)
        ? throw new ArgumentNullException(nameof(s))
        : TryParse(s.AsSpan(), out result);

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider,
        out Vec3DLong result) => TryParse(s, out result);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Vec3DLong result) =>
        TryParse(s, out result);

    public static bool TryParse(ReadOnlySpan<char> s, out Vec3DLong result)
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

    /// <summary>Returns the string representation of the current instance using default formatting: "X,Y,Z".</summary>
    public override string ToString() => $"{X},{Y},{Z}";

    #endregion

    #region Operators

    public static Vec3DLong operator ++(Vec3DLong value) => new(value.X + 1, value.Y + 1, value.Z + 1);
    public static Vec3DLong operator --(Vec3DLong value) => new(value.X - 1, value.Y - 1, value.Z - 1);
    public static Vec3DLong operator +(Vec3DLong value) => value;

    public static Vec3DLong operator +(Vec3DLong left, Vec3DLong right) =>
        new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

    public static Vec3DLong operator -(Vec3DLong value) => new(-value.X, -value.Y, -value.Z);

    public static Vec3DLong operator -(Vec3DLong left, Vec3DLong right) =>
        new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    public static Vec3DLong operator *(Vec3DLong left, long right) =>
        new(left.X * right, left.Y * right, left.Z * right);

    public static Vec3DLong operator *(long left, Vec3DLong right) =>
        new(left * right.X, left * right.Y, left * right.Z);

    public static Vec3DLong operator *(Vec3DLong left, Vec3DLong right) =>
        new(left.X * right.X, left.Y * right.Y, left.Z * right.Z);

    public static Vec3DLong operator /(Vec3DLong value, long divisor) =>
        new(value.X / divisor, value.Y / divisor, value.Z / divisor);

    public static Vec3DLong operator /(Vec3DLong left, Vec3DLong right) =>
        new(left.X / right.X, left.Y / right.Y, left.Z / right.Z);

    public static Vec3DLong operator %(Vec3DLong value, long mod) => new(value.X % mod, value.Y % mod, value.Z % mod);

    public static Vec3DLong operator %(Vec3DLong left, Vec3DLong right) =>
        new(left.X % right.X, left.Y % right.Y, left.Z % right.Z);

    #endregion
}
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Numerics;

/// <summary>
/// Created this to be useful for Grids, when floating point arithmetic could be a bad idea
/// </summary>
public struct Vector2Int : IEquatable<Vector2Int>, IFormattable
{
    public int X;
    public int Y;

    /// <summary>Creates a new Vector2Int object whose two elements have the same value</summary>
    public Vector2Int(int value)
    {
        X = value;
        Y = value;
    }
    /// <summary>Creates a vector whose elements have the specified values.</summary>
    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }

    /// <summary>Gets or sets the element at the specified index (0 = X, 1 = Y).</summary>
    public int this[int index]
    {
        get => index switch
        {
            0 => X,
            1 => Y,
            _ => throw new ArgumentOutOfRangeException($"{index} is not a valid index for Vector2Int")
        };
        set
        {
            switch (index)
            {
                case 0: X = value; break;
                case 1: Y = value; break;
                default: throw new ArgumentOutOfRangeException($"{index} is not a valid index for Vector2Int");
            }
        }
    }

    #region Static Properties

    /// <summary>Gets the vector (1,0).</summary>
    public static readonly Vector2Int Right = new(1, 0);
    /// <summary>Gets the vector (0,1).</summary>
    public static readonly Vector2Int Up = new(0, 1);
    /// <summary>Gets the vector (-1,0).</summary>
    public static readonly Vector2Int Left = new(-1, 0);
    /// <summary>Gets the vector (0,-1).</summary>
    public static readonly Vector2Int Down = new(0, -1);
    ///<summary>A vector whose elements are equal to one (that is, it returns the vector (1, 1)</summary>
    public static readonly Vector2Int One = new(1, 1);
    ///<summary>A vector whose elements are equal to zero (that is, it returns the vector (0, 0)</summary>
    public static readonly Vector2Int Zero = new(0, 0);

    /// <summary>Gets the vector (0,1).</summary>
    public static readonly Vector2Int N = Up;
    /// <summary>Gets the vector (1,0).</summary>
    public static readonly Vector2Int E = Right;
    /// <summary>Gets the vector (0,-1).</summary>
    public static readonly Vector2Int S = Down;
    /// <summary>Gets the vector (-1,0).</summary>
    public static readonly Vector2Int W = Left;
    /// <summary>Gets the vector (1,1).</summary>
    public static readonly Vector2Int NE = new(1, 1);
    /// <summary>Gets the vector (1,-1).</summary>
    public static readonly Vector2Int SE = new(1, -1);
    /// <summary>Gets the vector (-1,-1).</summary>
    public static readonly Vector2Int SW = new(-1, -1);
    /// <summary>Gets the vector (-1,1).</summary>
    public static readonly Vector2Int NW = new(-1, 1);
    /// <summary>Returns the four cardinal directions N, E, S, W in that order.</summary>
    public static readonly Vector2Int[] CardinalDirections = new Vector2Int[4] { N, E, S, W };
    /// <summary>Also called "intercardinal directions." Returns the four ordinal directions NE, SE, SW, NW in that order.</summary>
    public static readonly Vector2Int[] OrdinalDirections = new Vector2Int[4] { NE, SE, SW, NW };
    /// <summary>Returns all 8 points of the compass going clockwise from North around to NW.</summary>
    public static readonly Vector2Int[] CompassPoints = new Vector2Int[8] { N, NE, E, SE, S, SW, W, NW };

    #endregion

    #region Non-Static Methods

    /// <summary>Make both parts of this vector non-negative.</summary>
    public void Abs() { X = Math.Abs(X); Y = Math.Abs(Y); }
    /// <summary>Adds two values to this vector.</summary>
    public void Add(int x, int y) { X += x; Y += y; }
    /// <summary>Adds <paramref name="value"/> to both parts of this vector.</summary>
    public void Add(int value) => Add(value, value);
    /// <summary>Adds an <paramref name="other"/> vector to this one.</summary>
    public void Add(Vector2Int other) => Add(other.X, other.Y);
    /// <summary>Restricts this vector between minimum and maximum values.</summary>
    public void Clamp(int minX, int maxX, int minY, int maxY) { X = Math.Clamp(X, minX, maxX); Y = Math.Clamp(Y, minY, maxY); }
    /// <summary>Restricts this vector between minimum and maximum values.</summary>
    public void Clamp(Vector2Int min, Vector2Int max) => Clamp(min.X, max.X, min.Y, max.Y);
    /// <summary>Computes the Euclidian distance between <paramref name="other"/> and this vector.</summary>
    public readonly double DistanceEuclidianTo(Vector2Int other) => Math.Sqrt(DistanceSquaredTo(other));
    /// <summary>Computes the Chebyshev distance, also known as chessboard distance - the amount of moves a king would take to get from a to b.</summary>
    public readonly int DistanceChebyshevTo(Vector2Int other) => Math.Max(Math.Abs(other.X - X), Math.Abs(other.Y - Y));
    /// <summary>Computes the Manhattan distance between the two given points. No diagonal moves.</summary>
    public readonly int DistanceManhattanTo(Vector2Int other) => Math.Abs(other.X - X) + Math.Abs(other.Y - Y);
    /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
    public readonly int DistanceSquaredTo(Vector2Int other) => (other.X - X) * (other.X - X) + (other.Y - Y) * (other.Y - Y);
    /// <summary>Divides this vector by <paramref name="x"/> and <paramref name="y"/> using integer division.</summary>
    public void DivideBy(int x, int y) { X /= x; Y /= y; }
    /// <summary>Divides this vector by <paramref name="divisor"/> using integer division.</summary>
    public void DivideBy(int divisor) => DivideBy(divisor, divisor);
    /// <summary>Divides this vector by <paramref name="divisor"/> using piece-wise integer division.</summary>
    public void DivideBy(Vector2Int divisor) => DivideBy(divisor.X, divisor.Y);
    /// <summary>Returns a value that indicates if this vector and <paramref name="other"/> are next to each other laterally.</summary>
    public readonly bool IsAdjacentTo(Vector2Int other) => DistanceManhattanTo(other) == 1;
    /// <summary>Returns a value that indicates if this vector and <paramref name="other"/> are next to each other diagonally.</summary>
    public readonly bool IsDiagonalTo(Vector2Int other) => DistanceManhattanTo(other) == 2 && DistanceChebyshevTo(other) == 1;
    /// <summary>Horizontally or Vertically aligned at any distance, but not the same position</summary>
    public readonly bool IsLateralTo(Vector2Int other) => X == other.X ^ Y == other.Y;
    /// <summary>Returns a value that indicates if this vector and <paramref name="other"/> are parralel and neither are Vector2Int.Zero</summary>
    public readonly bool IsParallelTo(Vector2Int other) => this != Zero && other != Zero && X * other.Y == Y * other.X;
    /// <summary>Returns a value that indicates if this vector and <paramref name="other"/> are perpendicular and neither is Vector2Int.Zero</summary>
    public readonly bool IsPerpendicularTo(Vector2Int other) => this != Zero && other != Zero && X * other.X == -Y * other.Y;
    /// <summary>Returns the length of the vector.</summary>
    public readonly double Length() => Math.Sqrt(LengthSquared());
    /// <summary>Returns the length of the vector squared.</summary>
    public readonly int LengthSquared() => X * X + Y * Y;
    /// <summary>Multiplies this vector by a <paramref name="scalar"/> value.</summary>
    public void MultiplyBy(int scalar) { X *= scalar; Y *= scalar; }
    /// <summary>Multiplies this vector by another vector piece-wise.</summary>
    public void MultiplyBy(Vector2Int scalars) { X *= scalars.X; Y *= scalars.Y; }
    /// <summary>Negates this vector.</summary>
    public void Negate() { X = -X; Y = -Y; }
    /// <summary>Rotates this vector clockwise 90° from the perspective of the standard XY grid.</summary>
    public void RotateRight() { (X, Y) = (Y, -X); }
    /// <summary>Rotates this vector counter-clockwise 90° from the perspective of the standard XY grid.</summary>
    public void RotateLeft() { (X, Y) = (-Y, X); }
    /// <summary>Resets this vector to the origin (0, 0).</summary>
    public void Reset() { X = 0; Y = 0; }
    /// <summary>Subtracts <paramref name="x"/> and <paramref name="y"/> from this vector.</summary>
    public void Subtract(int x, int y) { X -= x; Y -= y; }
    /// <summary>Subtracts <paramref name="value"/> from both parts of this vector.</summary>
    public void Subtract(int value) => Subtract(value, value);
    /// <summary>Subtracts a vector from this vector.</summary>
    public void Subtract(Vector2Int other) => Subtract(other.X, other.Y);

    #endregion

    #region Static Methods

    /// <summary>Returns a vector whose elements are the absolute values of each of the specified vector's elements.</summary>
    public static Vector2Int Abs(Vector2Int value) => new(Math.Abs(value.X), Math.Abs(value.Y));
    /// <summary>Restricts a vector between a minimum and a maximum value.</summary>
    public static Vector2Int Clamp(Vector2Int value, Vector2Int min, Vector2Int max) => Clamp(value, min.X, max.X, min.Y, max.Y);
    /// <summary>Restricts a vector between a minimum and a maximum value.</summary>
    public static Vector2Int Clamp(Vector2Int value, int minX, int maxX, int minY, int maxY) => new(Math.Clamp(value.X, minX, maxX), Math.Clamp(value.Y, minY, maxY));
    /// <summary>Computes the Euclidian distance between the two given points.</summary>
    public static double DistanceEuclidian(Vector2Int a, Vector2Int b) => Math.Sqrt(DistanceSquared(a, b));
    /// <summary>Computes the Chebyshev distance, also known as chessboard distance - the amount of moves a king would take to get from a to b.</summary>
    public static int DistanceChebyshev(Vector2Int a, Vector2Int b) => Math.Max(Math.Abs(b.X - a.X), Math.Abs(b.Y - a.Y));
    /// <summary>Computes the Manhattan distance between the two given points. No diagonal moves.</summary>
    public static int DistanceManhattan(Vector2Int a, Vector2Int b) => Math.Abs(b.X - a.X) + Math.Abs(b.Y - a.Y);
    /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
    public static int DistanceSquared(Vector2Int a, Vector2Int b) => (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);
    /// <summary>Returns the dot product of two vectors.</summary>
    public static int Dot(Vector2Int a, Vector2Int b) => a.X * b.X + a.Y * b.Y;
    /// <summary>Returns all points in a rectangle (or line) from the min corner to the max corner, inclusive.</summary>
    public static IEnumerable<Vector2Int> GetAllPointsBetween(Vector2Int from, Vector2Int to)
    {
        int minX = Math.Min(from.X, to.X);
        int maxX = Math.Max(from.X, to.X);
        int minY = Math.Min(from.Y, to.Y);
        int maxY = Math.Max(from.Y, to.Y);
        foreach (var point in GetAllPointsBetween(minX, maxX, minY, maxY))
            yield return point;
    }
    /// <summary>Returns all points in a rectangle (or line) between the min and max points, inclusive.</summary>
    public static IEnumerable<Vector2Int> GetAllPointsBetween(int minX, int maxX, int minY, int maxY)
    {
        for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
                yield return new Vector2Int(x, y);
    }
    /// <summary>Returns a direct "king-walk" path with diagonals first, <paramref name="from"/> and <paramref name="to"/> inclusive.</summary>
    public static IEnumerable<Vector2Int> GetChebyshevPath(Vector2Int from, Vector2Int to)
    {
        while (from != to)
        {
            yield return from;
            from.X += Math.Sign(to.X - from.X);
            from.Y += Math.Sign(to.Y - from.Y);
        }
        yield return to;
    }
    /// <summary>
    /// Given two lines that each passes through a point with a slope, return their intersection.
    /// Note: This is a simplified version, only designed to work for diagonal slopes of 1 and -1.
    /// If their intersection doesn't land exactly on an integer, it will round down.
    /// </summary>
    /// <exception cref="Exception"></exception>
    public static Vector2Int GetLineIntersect(Vector2Int pt1, int slope1, Vector2Int pt2, int slope2)
    {
        if (slope1 == slope2) throw new Exception("Lines are parallel and don't have a single intersect");
        var x = (pt1.X - slope1 * pt1.Y + pt2.X - slope2 * pt2.Y) / 2;
        var y = (-slope1 * pt1.X + pt1.Y - slope2 * pt2.X + pt2.Y) / 2;
        return new Vector2Int(x, y);
    }
    /// <summary>Returns a float as the percentage that <paramref name="value"/> is between two vectors.</summary>
    public static float InverseLerp(Vector2Int from, Vector2Int to, Vector2Int value)
    {
        if (from.X != to.X) return (float)(value.X - from.X) / (to.X - from.X);
        if (from.Y != to.Y) return (float)(value.Y - from.Y) / (to.Y - from.Y);
        return 0f;
    }
    /// <summary>Performs a linear interpolation between two vectors based on the given weighting (0f to 1f).</summary>
    public static Vector2Int Lerp(Vector2Int from, Vector2Int to, float weight)
    {
        var x = (int)Math.Round(from.X + (to.X - from.X) * weight);
        var y = (int)Math.Round(from.Y + (to.Y - from.Y) * weight);
        return new(x, y);
    }
    /// <summary>Re-maps a vector from one range to another.</summary>
    public static Vector2Int Map(Vector2Int fromMin, Vector2Int fromMax, Vector2Int toMin, Vector2Int toMax, Vector2Int value) => Lerp(toMin, toMax, InverseLerp(fromMin, fromMax, value));
    /// <summary>Returns a vector whose elements are the maximum of each of the pairs of elements in two specified vectors.</summary>
    public static Vector2Int Max(Vector2Int a, Vector2Int b) => new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
    /// <summary>Returns a vector whose elements are the minimum of each of the pairs of elements in two specified vectors.</summary>
    public static Vector2Int Min(Vector2Int a, Vector2Int b) => new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
    /// <summary>Returns a vector that is the rotated clockwise 90° verion of <paramref name="other"/>.</summary>
    public static Vector2Int RotatedRight(Vector2Int other) => new(other.Y, -other.X);
    /// <summary>Returns a vector that is the rotated counter-clockwise 90° verion of <paramref name="other"/>.</summary>
    public static Vector2Int RotatedLeft(Vector2Int other) => new(-other.Y, other.X);

    #endregion

    #region Interface and Override Methods

    /// <summary>Returns a value that indicates whether this instance and another vector are equal.</summary>
    public readonly bool Equals(Vector2Int other) => this == other;
    /// <summary>Returns a value that indicates whether this instance and another vector are equal.</summary>
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Vector2Int other && Equals(other);
    /// <summary>Returns the hash code for this instance.</summary>
    public override int GetHashCode() => HashCode.Combine(X, Y);
    /// <summary>
    /// Returns the string representation of the current instance using the specified
    /// format string to format individual elements and the specified format provider
    /// to define culture-specific formatting.
    /// </summary>
    public string ToString([StringSyntax("NumericFormat")] string? format, IFormatProvider? formatProvider) => $"{X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)}";
    /// <summary>Returns the string representation of the current instance using default formatting: "X,Y".</summary>
    public readonly override string ToString() => $"{X}, {Y}";
    /// <summary>Returns the string representation of the current instance using the specified format string to format individual elements.</summary>
    public readonly string ToString([StringSyntax("NumericFormat")] string format) => $"{X.ToString(format)}, {Y.ToString(format)}";

    #endregion

    #region Operators

    /// <summary>Adds two vectors together.</summary>
    public static Vector2Int operator +(Vector2Int left, Vector2Int right) => new(left.X + right.X, left.Y + right.Y);
    /// <summary>Negates the specified vector.</summary>
    public static Vector2Int operator -(Vector2Int value) => new(-value.X, -value.Y);
    /// <summary>Subtracts the second vector from the first.</summary>
    public static Vector2Int operator -(Vector2Int left, Vector2Int right) => new(left.X - right.X, left.Y - right.Y);
    /// <summary>Returns the pair-wise multiple of the two vectors</summary>
    public static Vector2Int operator *(Vector2Int left, Vector2Int right) => new(left.X * right.X, left.Y * right.Y);
    /// <summary>Multiples the scalar value by the specified vector.</summary>
    public static Vector2Int operator *(int left, Vector2Int right) => new(left * right.X, left * right.Y);
    /// <summary>Multiples the scalar value by the specified vector.</summary>
    public static Vector2Int operator *(Vector2Int left, int right) => new(left.X * right, left.Y * right);
    /// <summary>Divides the first vector by the second pair-wise using integer division.</summary>
    public static Vector2Int operator /(Vector2Int left, Vector2Int right) => new(left.X / right.X, left.Y / right.Y);
    /// <summary>Divides the specified vector by a specified scalar value using integer division.</summary>
    public static Vector2Int operator /(Vector2Int value, int divisor) => new(value.X / divisor, value.Y / divisor);
    ///<summary>Returns a value that indicates whether each pair of elements in two specified vectors is equal.</summary>     
    public static bool operator ==(Vector2Int left, Vector2Int right) => left.X == right.X && left.Y == right.Y;
    ///<summary>Returns a value that indicates whether two specified vectors are not equal.</summary>     
    public static bool operator !=(Vector2Int left, Vector2Int right) => left.X != right.X || left.Y != right.Y;

    #endregion
}
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Numerics;

/// <summary>
/// Created this to be useful for 3D Grids, when floating point arithmetic could be a bad idea
/// </summary>
public struct Vector3Int
{
    public int X, Y, Z;

    public Vector3Int(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    #region Static Properties

    /// <summary>Gets the vector (1,0,0).</summary>
    public static readonly Vector3Int Right = new(1, 0, 0);
    /// <summary>Gets the vector (0,1,0).</summary>
    public static readonly Vector3Int Up = new(0, 1, 0);
    /// <summary>Gets the vector (0,0,1).</summary>
    public static readonly Vector3Int Forward = new(0, 0, 1);
    /// <summary>Gets the vector (-1,0,0).</summary>
    public static readonly Vector3Int Left = new(-1, 0, 0);
    /// <summary>Gets the vector (0,-1,0).</summary>
    public static readonly Vector3Int Down = new(0, -1, 0);
    /// <summary>Gets the vector (0,0,-1).</summary>
    public static readonly Vector3Int Backward = new(0, 0, -1);
    ///<summary>A vector whose elements are equal to one (that is, it returns the vector (1, 1, 1)</summary>
    public static readonly Vector3Int One = new(1, 1, 1);
    ///<summary>A vector whose elements are equal to zero (that is, it returns the vector (0, 0, 0)</summary>
    public static readonly Vector3Int Zero = new(0, 0, 0);
    /// <summary>Returns the six three-dimensional unit integer directions you can move in.</summary>
    public static readonly Vector3Int[] AllDirections = new Vector3Int[6] { Right, Up, Forward, Left, Down, Backward };

    #endregion

    #region Non-Static Methods

    /// <summary>Make both parts of this vector non-negative.</summary>
    public void Abs() { X = Math.Abs(X); Y = Math.Abs(Y); Z = Math.Abs(Z); }
    /// <summary>Adds two values to this vector.</summary>
    public void Add(int x, int y, int z) { X += x; Y += y; Z += z; }
    /// <summary>Adds <paramref name="value"/> to both parts of this vector.</summary>
    public void Add(int value) => Add(value, value, value);
    /// <summary>Adds an <paramref name="other"/> vector to this one.</summary>
    public void Add(Vector3Int other) => Add(other.X, other.Y, other.Z);
    /// <summary>Restricts this vector between minimum and maximum values.</summary>
    public void Clamp(int minX, int maxX, int minY, int maxY, int minZ, int maxZ) { X = Math.Clamp(X, minX, maxX); Y = Math.Clamp(Y, minY, maxY); Z = Math.Clamp(Z, minZ, maxZ); }
    /// <summary>Restricts this vector between minimum and maximum values.</summary>
    public void Clamp(Vector3Int min, Vector3Int max) => Clamp(min.X, max.X, min.Y, max.Y, min.Z, max.Z);
    /// <summary>Computes the Euclidian distance between <paramref name="other"/> and this vector.</summary>
    public readonly double DistanceEuclidianTo(Vector3Int other) => Math.Sqrt(DistanceSquaredTo(other));
    /// <summary>Computes the Chebyshev distance. In 2d, this is known as chessboard distance - the amount of moves a king would take to get from a to b.</summary>
    public readonly int DistanceChebyshevTo(Vector3Int other) => Math.Max(Math.Max(Math.Abs(other.X - X), Math.Abs(other.Y - Y)), Math.Abs(other.Z - Z));
    /// <summary>Computes the Manhattan distance between the two given points. No diagonal moves.</summary>
    public readonly int DistanceManhattanTo(Vector3Int other) => Math.Abs(other.X - X) + Math.Abs(other.Y - Y) + Math.Abs(other.Z - Z);
    /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
    public readonly int DistanceSquaredTo(Vector3Int other) => (other.X - X) * (other.X - X) + (other.Y - Y) * (other.Y - Y) + (other.Z - Z) * (other.Z - Z);
    /// <summary>Divides this vector by <paramref name="x"/> and <paramref name="y"/> using integer division.</summary>
    public void DivideBy(int x, int y, int z) { X /= x; Y /= y; Z /= z; }
    /// <summary>Divides this vector by <paramref name="divisor"/> using integer division.</summary>
    public void DivideBy(int divisor) => DivideBy(divisor, divisor, divisor);
    /// <summary>Divides this vector by <paramref name="divisor"/> using piece-wise integer division.</summary>
    public void DivideBy(Vector3Int divisor) => DivideBy(divisor.X, divisor.Y, divisor.Z);
    /// <summary>Returns a value that indicates if this vector and <paramref name="other"/> are next to each other laterally.</summary>
    public readonly bool IsAdjacentTo(Vector3Int other) => DistanceManhattanTo(other) == 1;
    /// <summary>Returns a value that indicates if this vector and <paramref name="other"/> are next to each other diagonally.</summary>
    public readonly bool IsDiagonalTo(Vector3Int other) => DistanceManhattanTo(other) == 2 && DistanceChebyshevTo(other) == 1;
    /// <summary>Lies along at least 1 same axis at any distance, but not the same position</summary>
    public readonly bool IsLateralTo(Vector3Int other) => other != this && (other.X == X || other.Y == Y || other.Z == Z);
    /* TODO: work out the math on this for 3D
    /// <summary>Returns a value that indicates if this vector and <paramref name="other"/> are parralel and neither are Vector2Int.Zero</summary>
    public readonly bool IsParallelTo(Vector3Int other) => this != Zero && other != Zero && X * other.Y == Y * other.X;
    /// <summary>Returns a value that indicates if this vector and <paramref name="other"/> are perpendicular and neither is Vector2Int.Zero</summary>
    public readonly bool IsPerpendicularTo(Vector2Int other) => this != Zero && other != Zero && X * other.X == -Y * other.Y;
    */
    /// <summary>Returns the length of the vector.</summary>
    public readonly double Length() => Math.Sqrt(LengthSquared());
    /// <summary>Returns the length of the vector squared.</summary>
    public readonly int LengthSquared() => X * X + Y * Y + Z * Z;
    /// <summary>Multiplies this vector by a <paramref name="scalar"/> value.</summary>
    public void MultiplyBy(int scalar) { X *= scalar; Y *= scalar; Z *= scalar; }
    /// <summary>Multiplies this vector by another vector piece-wise.</summary>
    public void MultiplyBy(Vector3Int scalars) { X *= scalars.X; Y *= scalars.Y; Z *= scalars.Z; }
    /// <summary>Negates this vector.</summary>
    public void Negate() { X = -X; Y = -Y; Z = -Z; }
    // TODO: this isn't really a thing anymore in 3D
    ///// <summary>Rotates this vector clockwise 90° from the perspective of the standard XY grid.</summary>
    //public void RotateRight() { (X, Y) = (Y, -X); }
    ///// <summary>Rotates this vector counter-clockwise 90° from the perspective of the standard XY grid.</summary>
    //public void RotateLeft() { (X, Y) = (-Y, X); }
    /// <summary>Resets this vector to the origin (0, 0).</summary>
    public void Reset() { X = 0; Y = 0; Z = 0; }
    /// <summary>Subtracts <paramref name="x"/> and <paramref name="y"/> from this vector.</summary>
    public void Subtract(int x, int y, int z) { X -= x; Y -= y; Y -= z; }
    /// <summary>Subtracts <paramref name="value"/> from both parts of this vector.</summary>
    public void Subtract(int value) => Subtract(value, value, value);
    /// <summary>Subtracts a vector from this vector.</summary>
    public void Subtract(Vector3Int other) => Subtract(other.X, other.Y, other.Z);

    #endregion

    #region Static Methods

    /// <summary>Returns a vector whose elements are the absolute values of each of the specified vector's elements.</summary>
    public static Vector3Int Abs(Vector3Int value) => new(Math.Abs(value.X), Math.Abs(value.Y), Math.Abs(value.Z));
    /// <summary>Restricts a vector between a minimum and a maximum value.</summary>
    public static Vector3Int Clamp(Vector3Int value, Vector3Int min, Vector3Int max) => Clamp(value, min.X, max.X, min.Y, max.Y, min.Z, max.Z);
    /// <summary>Restricts a vector between a minimum and a maximum value.</summary>
    public static Vector3Int Clamp(Vector3Int value, int minX, int maxX, int minY, int maxY, int minZ, int maxZ) => new(Math.Clamp(value.X, minX, maxX), Math.Clamp(value.Y, minY, maxY), Math.Clamp(value.Z, minZ, maxZ));
    // TODO: Add the Cross Product
    //public static Vector3Int Cross(Vector3Int left, Vector3Int right) =>
    /// <summary>Computes the Euclidian distance between the two given points.</summary>
    public static double DistanceEuclidian(Vector3Int a, Vector3Int b) => Math.Sqrt(DistanceSquared(a, b));
    /// <summary>Computes the Chebyshev distance, also known as chessboard distance - the amount of moves a king would take to get from a to b.</summary>
    public static int DistanceChebyshev(Vector3Int a, Vector3Int b) => Math.Max(Math.Max(Math.Abs(b.X - a.X), Math.Abs(b.Y - a.Y)), Math.Abs(b.Z - a.Z));
    /// <summary>Computes the Manhattan distance between the two given points. No diagonal moves.</summary>
    public static int DistanceManhattan(Vector3Int a, Vector3Int b) => Math.Abs(b.X - a.X) + Math.Abs(b.Y - a.Y) + Math.Abs(b.Z - a.Z);
    /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
    public static int DistanceSquared(Vector3Int a, Vector3Int b) => (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y) + (b.Z - a.Z) * (b.Z - a.Z);
    /* TODO: Compute the formula for the Dot product
    /// <summary>Returns the dot product of two vectors.</summary>
    public static int Dot(Vector2Int a, Vector2Int b) => a.X * b.X + a.Y * b.Y;
    */
    /// <summary>Returns all points in a box/plane/line between the corner points <paramref name="from"/> and <paramref name="to"/>, inclusive.</summary>
    public static IEnumerable<Vector3Int> GetAllPointsBetween(Vector3Int from, Vector3Int to)
    {

        int minX = Math.Min(from.X, to.X);
        int maxX = Math.Max(from.X, to.X);
        int minY = Math.Min(from.Y, to.Y);
        int maxY = Math.Max(from.Y, to.Y);
        int minZ = Math.Min(from.Z, to.Z);
        int maxZ = Math.Max(from.Z, to.Z);
        for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
                for (int z = minZ; z <= maxZ; z++)
                    yield return new Vector3Int(x, y, z);
    }
    /// <summary>Returns all points in a box/plane/line between the min and max points, inclusive.</summary>
    public static IEnumerable<Vector3Int> GetAllPointsBetween(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
    {
        for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
                for (int z = minZ; z <= maxZ; z++)
                    yield return new Vector3Int(x, y, z);
    }
    /// <summary>Returns a direct "king-walk" path with diagonals first, <paramref name="from"/> and <paramref name="to"/> inclusive.</summary>
    public static IEnumerable<Vector3Int> GetChebyshevPath(Vector3Int from, Vector3Int to)
    {
        while (from != to)
        {
            yield return from;
            from.X += Math.Sign(to.X - from.X);
            from.Y += Math.Sign(to.Y - from.Y);
            from.Z += Math.Sign(to.Z - from.Z);
        }
        yield return to;
    }
    // TODO: find a formula for getting the intersect of two lines in 3d space
    /// <summary>
    /// Given two lines that each passes through a point with a slope, return their intersection.
    /// Note: This is a simplified version, only designed to work for diagonal slopes of 1 and -1.
    /// If their intersection doesn't land exactly on an integer, it will round down.
    /// </summary>
    /// <exception cref="Exception"></exception>
    //public static Vector2Int GetLineIntersect(Vector2Int pt1, int slope1, Vector2Int pt2, int slope2)
    //{
    //    if (slope1 == slope2) throw new Exception("Lines are parallel and don't have a single intersect");
    //    var x = (pt1.X - slope1 * pt1.Y + pt2.X - slope2 * pt2.Y) / 2;
    //    var y = (-slope1 * pt1.X + pt1.Y - slope2 * pt2.X + pt2.Y) / 2;
    //    return new Vector2Int(x, y);
    //}
    /// <summary>Returns a float as the percentage that <paramref name="value"/> is between two vectors.</summary>
    public static float InverseLerp(Vector3Int from, Vector3Int to, Vector3Int value)
    {
        if (from.X != to.X) return (float)(value.X - from.X) / (to.X - from.X);
        if (from.Y != to.Y) return (float)(value.Y - from.Y) / (to.Y - from.Y);
        if (from.Z != to.Z) return (float)(value.Z - from.Z) / (to.Z - from.Z);
        return 0f;
    }
    /// <summary>Performs a linear interpolation between two vectors based on the given weighting (0f to 1f).</summary>
    public static Vector3Int Lerp(Vector3Int from, Vector3Int to, float weight)
    {
        weight = Math.Clamp(weight, 0f, 1f);
        var x = (int)Math.Round(from.X + (to.X - from.X) * weight);
        var y = (int)Math.Round(from.Y + (to.Y - from.Y) * weight);
        var z = (int)Math.Round(from.Z + (to.Z - from.Z) * weight);
        return new(x, y, z);
    }
    /// <summary>Re-maps a vector from one range to another.</summary>
    public static Vector3Int Map(Vector3Int fromMin, Vector3Int fromMax, Vector3Int toMin, Vector3Int toMax, Vector3Int value) => Lerp(toMin, toMax, InverseLerp(fromMin, fromMax, value));
    /// <summary>Returns a vector whose elements are the maximum of each of the individual elements in two specified vectors.</summary>
    public static Vector3Int Max(Vector3Int a, Vector3Int b) => new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
    /// <summary>Returns a vector whose elements are the minimum of each of the pairs of elements in two specified vectors.</summary>
    public static Vector3Int Min(Vector3Int a, Vector3Int b) => new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));

    #endregion

    #region Interface and Override Methods

    /// <summary>Returns a value that indicates whether this instance and another vector are equal.</summary>
    public readonly bool Equals(Vector3Int other) => this == other;
    /// <summary>Returns a value that indicates whether this instance and another vector are equal.</summary>
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Vector3Int other && Equals(other);
    /// <summary>Returns the hash code for this instance.</summary>
    public override int GetHashCode() => HashCode.Combine(X, Y, Z);
    /// <summary>
    /// Returns the string representation of the current instance using the specified
    /// format string to format individual elements and the specified format provider
    /// to define culture-specific formatting.
    /// </summary>
    public string ToString([StringSyntax("NumericFormat")] string format, IFormatProvider formatProvider) => $"{X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)}, {Z.ToString(format, formatProvider)}";
    /// <summary>Returns the string representation of the current instance using default formatting: "X,Y".</summary>
    public readonly override string ToString() => $"{X}, {Y}, {Z}";
    /// <summary>Returns the string representation of the current instance using the specified format string to format individual elements.</summary>
    public readonly string ToString([StringSyntax("NumericFormat")] string format) => $"{X.ToString(format)}, {Y.ToString(format)}, {Z.ToString(format)}";

    #endregion

    #region Operators

    /// <summary>Adds two vectors together.</summary>
    public static Vector3Int operator +(Vector3Int left, Vector3Int right) => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    /// <summary>Negates the specified vector.</summary>
    public static Vector3Int operator -(Vector3Int value) => new(-value.X, -value.Y, -value.Z);
    /// <summary>Subtracts the second vector from the first.</summary>
    public static Vector3Int operator -(Vector3Int left, Vector3Int right) => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    /// <summary>Returns the pair-wise multiple of the two vectors</summary>
    public static Vector3Int operator *(Vector3Int left, Vector3Int right) => new(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
    /// <summary>Multiples the scalar value by the specified vector.</summary>
    public static Vector3Int operator *(int left, Vector3Int right) => new(left * right.X, left * right.Y, left * right.Z);
    /// <summary>Multiples the scalar value by the specified vector.</summary>
    public static Vector3Int operator *(Vector3Int left, int right) => new(left.X * right, left.Y * right, left.Z * right);
    /// <summary>Divides the first vector by the second pair-wise using integer division.</summary>
    public static Vector3Int operator /(Vector3Int left, Vector3Int right) => new(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
    /// <summary>Divides the specified vector by a specified scalar value using integer division.</summary>
    public static Vector3Int operator /(Vector3Int value, int divisor) => new(value.X / divisor, value.Y / divisor, value.Z / divisor);
    ///<summary>Returns a value that indicates whether each pair of elements in two specified vectors is equal.</summary>     
    public static bool operator ==(Vector3Int left, Vector3Int right) => left.X == right.X && left.Y == right.Y && left.Z == right.Z;
    ///<summary>Returns a value that indicates whether two specified vectors are not equal.</summary>     
    public static bool operator !=(Vector3Int left, Vector3Int right) => left.X != right.X || left.Y != right.Y || left.Z != right.Z;
    
    #endregion
}

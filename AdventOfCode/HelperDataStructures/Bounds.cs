using System;
using System.Collections.Generic;
using System.Numerics;

namespace AoC;

public struct Bounds(int min, int max)
{
    public int XMin { get; private set; } = min;
    public int XMax { get; private set; } = max;

    public Bounds() : this(0, 0)
    {
    }

    public Bounds(Vector2Int range) : this(range.X, range.Y)
    {
    }

    /// <summary>
    ///     Returns the center point. If the *number of points* is even (even though Size will be odd),
    ///     it rounds down towards lower end. e.g. If XMin = 1, and XMax = 4, it will return 2 for the Center
    /// </summary>
    public readonly int Center => Min + Extents;

    /// <summary>
    ///     The extents of the Bounding Box. This is always half of the size of the Bounds.
    ///     Note: Integer division rounds down if Size is odd.
    /// </summary>
    public readonly int Extents => Size / 2;

    /// <summary>Right most point.</summary>
    public readonly int Max => XMax;

    /// <summary>Left most point.</summary>
    public readonly int Min => XMin;

    /// <summary>
    ///     Difference between XMin and XMax. e.g. If XMin = 1, and XMax = 4, it will return 3 even though there are 4
    ///     points contained.
    /// </summary>
    public readonly int Size => XMax - XMin;

    /// <summary>Returns a point on the border of this Bounds that is closest to <paramref name="x" />.</summary>
    public readonly int ClosestPointOnBorder(int x)
    {
        x = Math.Clamp(x, XMin, XMax);
        var dxLeft = x - XMin;
        var dxRight = XMax - x;
        return dxLeft < dxRight ? XMin : XMax;
    }

    /// <summary>Returns a value to indicate if another Bounds is fully within the bounding box, including sharing an edge.</summary>
    public readonly bool Contains(Bounds other)
    {
        return Contains(other.Min) && Contains(other.Max);
    }

    /// <summary>Returns a value to indicate if a point is within the bounding box.</summary>
    public readonly bool Contains(int x)
    {
        return x >= XMin && x <= XMax;
    }

    /// <summary>Returns a value that is the distance from the closest point on the Bounds' border.</summary>
    public readonly int DistanceFromBorder(int pos)
    {
        return Math.Min(Math.Abs(XMax - pos), Math.Abs(pos - XMin));
    }

    /// <summary>Grows the Bounds to include the point.</summary>
    public void Encapsulate(int x)
    {
        XMin = Math.Min(XMin, x);
        XMax = Math.Max(XMax, x);
    }

    /// <summary>Expand the bounds by increasing its size by amount along each side.</summary>
    public void Expand(int amount)
    {
        XMin -= amount;
        XMax += amount;
    }

    /// <summary>Generate all coordinates from XMin to XMax, inclusive.</summary>
    public readonly IEnumerable<int> GetAllPoints()
    {
        for (var x = XMin; x <= XMax; x++)
            yield return x;
    }

    /// <summary>Returns a value to indicate the position is directly on the edge of the Bounds.</summary>
    public readonly bool IsOnEdge(int x)
    {
        return x == XMin || x == XMax;
    }

    /// <summary>Returns a value to indicate if another bounding box intersects or shares an edge with this bounding box.</summary>
    public readonly bool Overlaps(Bounds other)
    {
        return other.XMax >= XMin && other.XMin <= XMax;
    }

    /// <summary>Sets the bounds to the min and max value of the box.</summary>
    public void SetMinMax(int min, int max)
    {
        XMin = min;
        XMax = max;
    }
}
﻿using System;
using System.Collections.Generic;
using System.Numerics;

namespace AoC;

public struct Bounds2D(int xMin, int xMax, int yMin, int yMax)
{
    public int XMin { get; private set; } = xMin;
    public int XMax { get; private set; } = xMax;
    public int YMin { get; private set; } = yMin;
    public int YMax { get; private set; } = yMax;

    public Bounds2D() : this(0, 0, 0, 0)
    {
    }

    public Bounds2D(Vector2Int point) : this(point.X, point.X, point.Y, point.Y)
    {
    }

    public Bounds2D(Vector2Int center, Vector2Int extents) :
        this(center.X - extents.X, center.X + extents.X, center.Y - extents.Y, center.Y + extents.Y)
    {
    }

    /// <summary>
    ///     Returns the center point. If the *number of points* on a side is even (even though Size will be odd),
    ///     it rounds down towards lower end. e.g. If XMin = 1, and XMax = 4, it will return 2 for the Center.X
    /// </summary>
    public readonly Vector2Int Center => Min + Extents;

    /// <summary>
    ///     The extents of the Bounding Box. This is always half of the size of the Bounds.
    ///     Note: Integer division rounds down if Width or Height is odd.
    /// </summary>
    public readonly Vector2Int Extents => Size / 2;

    /// <summary>Top-right most point.</summary>
    public readonly Vector2Int Max => new(XMax, YMax);

    /// <summary>Bottom-left most point.</summary>
    public readonly Vector2Int Min => new(XMin, YMin);

    /// <summary>
    ///     The total size of the bounding box. Note: This is not the number of distinct points along the edges, but
    ///     rather the space between.
    /// </summary>
    public readonly Vector2Int Size => new(Width, Height);

    /// <summary>
    ///     Difference between XMin and XMax. e.g. If XMin = 1, and XMax = 4, it will return 3 even though there are 4
    ///     points contained.
    /// </summary>
    public readonly int Width => XMax - XMin;

    /// <summary>
    ///     Difference between YMin and YMax. e.g. If YMin = 1, and YMax = 4, it will return 3 even though there are 4
    ///     points contained.
    /// </summary>
    public readonly int Height => YMax - YMin;

    /// <summary>Returns a point on the border of this Bounds that is closest to <paramref name="pos" />.</summary>
    public readonly Vector2Int ClosestPointOnBorder(Vector2Int pos) => ClosestPointOnBorder(pos.X, pos.Y);

    /// <summary>
    ///     Returns a point on the border of this Bounds that is closest to (<paramref name="x" />, <paramref name="y" />).
    ///     In case of a tie, this favors Y-edge over the X-edge.
    /// </summary>
    public readonly Vector2Int ClosestPointOnBorder(int x, int y)
    {
        x = Math.Clamp(x, XMin, XMax);
        y = Math.Clamp(y, YMin, YMax);

        var dxLeft = x - XMin;
        var dxRight = XMax - x;
        var dyBot = y - YMin;
        var dyTop = YMax - y;

        return Math.Min(dxLeft, dxRight) < Math.Min(dyBot, dyTop)
            ? new Vector2Int(dxLeft < dxRight ? XMin : XMax, y)
            : new Vector2Int(x, dyBot < dyTop ? YMin : YMax);
    }

    /// <summary>Returns a value to indicate if another Bounds is fully within the bounding box, including sharing an edge.</summary>
    public readonly bool Contains(Bounds2D other) => Contains(other.Min) && Contains(other.Max);

    /// <summary>Returns a value to indicate if a point is within the bounding box.</summary>
    public readonly bool Contains(Vector2Int pos) => Contains(pos.X, pos.Y);

    /// <summary>Returns a value to indicate if a point is within the bounding box.</summary>
    public readonly bool Contains(int x, int y) => IsInHorizontalBounds(x) && IsInVerticalBounds(y);

    /// <summary>Returns a value that is the distance from the closest point on the Bounds' border.</summary>
    public readonly int DistanceFromBorder(Vector2Int pos) => pos.DistanceManhattanTo(ClosestPointOnBorder(pos));

    /// <summary>Returns a value that is the distance from the closest point on the Bounds' border.</summary>
    public readonly int DistanceFromBorder(int x, int y) =>
        new Vector2Int(x, y).DistanceManhattanTo(ClosestPointOnBorder(x, y));

    /// <summary>Grows the Bounds to include the point.</summary>
    public void Encapsulate(Vector2Int point) => Encapsulate(point.X, point.Y);

    /// <summary>Grows the Bounds to include the point.</summary>
    public void Encapsulate(int x, int y)
    {
        XMin = Math.Min(XMin, x);
        XMax = Math.Max(XMax, x);
        YMin = Math.Min(YMin, y);
        YMax = Math.Max(YMax, y);
    }

    /// <summary>Expand the bounds by increasing its size by amount along each side.</summary>
    public void Expand(int amount) => Expand(amount, amount);

    /// <summary>Expand the bounds by increasing its size by each amount along their respective side.</summary>
    public void Expand(int xAmount, int yAmount)
    {
        XMin -= xAmount;
        XMax += xAmount;
        YMin -= yAmount;
        YMax += yAmount;
    }

    /// <summary>
    ///     Generate all coordinates from Min to Max. e.g. (XMin, YMin), (XMin, YMin + 1), ..., (XMax, YMax - 1), (XMax,
    ///     YMax).
    /// </summary>
    public readonly IEnumerable<Vector2Int> GetAllCoordinates()
    {
        for (var x = XMin; x <= XMax; x++)
        for (var y = YMin; y <= YMax; y++)
            yield return new Vector2Int(x, y);
    }

    /// <summary>Returns true if <paramref name="x" /> is on or inside the Bounds.</summary>
    public readonly bool IsInHorizontalBounds(int x) => XMin <= x && x <= XMax;

    /// <summary>Returns true if <paramref name="y" /> is on or inside the Bounds.</summary>
    public readonly bool IsInVerticalBounds(int y) => YMin <= y && y <= YMax;

    /// <summary>Returns a value to indicate the position is directly on the edge of the Bounds.</summary>
    public readonly bool IsOnEdge(Vector2Int pos) => IsOnEdge(pos.X, pos.Y);

    /// <summary>Returns a value to indicate the position is directly on the edge of the Bounds.</summary>
    public readonly bool IsOnEdge(int x, int y) => Contains(x, y) && (x == XMin || x == XMax || y == XMin || y == YMax);

    /// <summary>Returns a value to indicate if another bounding box intersects or shares an edge with this bounding box.</summary>
    public readonly bool Overlaps(Bounds2D other) =>
        XMin <= other.XMax && other.XMin <= XMax && YMin <= other.YMax && other.YMin <= YMax;

    /// <summary>Sets the bounds to the min and max value of the box.</summary>
    public void SetMinMax(Vector2Int min, Vector2Int max)
    {
        XMin = min.X;
        XMax = max.X;
        YMin = min.Y;
        YMax = max.Y;
    }
}
using System;
using System.Collections.Generic;
using System.Numerics;

namespace AoC;

public struct Bounds3D(int xMin, int xMax, int yMin, int yMax, int zMin, int zMax)
{
    public int XMin { get; private set; } = xMin;
    public int XMax { get; private set; } = xMax;
    public int YMin { get; private set; } = yMin;
    public int YMax { get; private set; } = yMax;
    public int ZMin { get; private set; } = zMin;
    public int ZMax { get; private set; } = zMax;

    public Bounds3D() : this(0, 0, 0, 0, 0, 0)
    {
    }

    public Bounds3D(Bounds3D other) : this(other.XMin, other.XMax, other.YMin, other.YMax, other.ZMin, other.ZMax)
    {
    }

    public Bounds3D(Vector3Int point) : this(point.X, point.X, point.Y, point.Y, point.Z, point.Z)
    {
    }

    public Bounds3D(Vector3Int center, Vector3Int extents) :
        this(center.X - extents.X, center.X + extents.X, center.Y - extents.Y, center.Y + extents.Y,
            center.Z - extents.Z, center.Z + extents.Z)
    {
    }

    /// <summary>
    ///     Returns the center point. If the *number of points* on a side is even (even though Size will be odd),
    ///     it rounds down towards lower end. e.g. If XMin = 1, and XMax = 4, it will return 2 for the Center.X
    /// </summary>
    public readonly Vector3Int Center => Min + Extents;

    /// <summary>
    ///     The extents of the Bounding Box. This is always half of the size of the Bounds.
    ///     Note: Integer division rounds down if Width or Height is odd.
    /// </summary>
    public readonly Vector3Int Extents => Size / 2;

    /// <summary>Top-right-forward most point.</summary>
    public readonly Vector3Int Max => new(XMax, YMax, ZMax);

    /// <summary>Bottom-left-backward most point.</summary>
    public readonly Vector3Int Min => new(XMin, YMin, ZMin);

    /// <summary>
    ///     The total size of the bounding box. Note: This is not the number of distinct points along the edges, but
    ///     rather the space between.
    /// </summary>
    public readonly Vector3Int Size => new(Width, Height, Depth);

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

    /// <summary>
    ///     Difference between ZMin and ZMax. e.g. If ZMin = 1, and ZMax = 4, it will return 3 even though there are 4
    ///     points contained.
    /// </summary>
    public readonly int Depth => ZMax - ZMin;

    /// <summary>Returns a point on the border of this Bounds that is closest to <paramref name="pos" />.</summary>
    public readonly Vector3Int ClosestPointOnBorder(Vector3Int pos)
    {
        return ClosestPointOnBorder(pos.X, pos.Y, pos.Z);
    }

    /// <summary>
    ///     Returns a point on the border of this Bounds that is closest to (<paramref name="x" />, <paramref name="y" />).
    ///     Ties go in favor of Z-face, then Y-face, then X-face.
    /// </summary>
    public readonly Vector3Int ClosestPointOnBorder(int x, int y, int z)
    {
        x = Math.Clamp(x, XMin, XMax);
        y = Math.Clamp(y, YMin, YMax);
        z = Math.Clamp(z, ZMin, ZMax);

        var dxLeft = x - XMin;
        var dxRight = XMax - x;
        var dyBot = y - YMin;
        var dyTop = YMax - y;
        var dzBack = z - ZMin;
        var dzForward = ZMax - z;

        if (Math.Min(dxLeft, dxRight) < Math.Min(dyBot, dyTop))
            if (Math.Min(dxLeft, dxRight) < Math.Min(dzBack, dzForward))
                return new Vector3Int(dxLeft < dxRight ? XMin : XMax, y, z); // closest to the left/right faces
            else
                return new Vector3Int(x, y, dzBack < dzForward ? ZMin : ZMax); // closest to the front/back faces
        if (Math.Min(dyBot, dyTop) < Math.Min(dzBack, dzForward))
            return new Vector3Int(x, dyBot < dyTop ? YMin : YMax, z); // closest to the top/bot faces
        return new Vector3Int(x, y, dzBack < dzForward ? ZMin : ZMax); // closest to the front/back faces
    }

    /// <summary>Returns a value to indicate if another Bounds is fully within the bounding box, including sharing an edge.</summary>
    public readonly bool Contains(Bounds3D other)
    {
        return Contains(other.Min) && Contains(other.Max);
    }

    /// <summary>Returns a value to indicate if a point is within the bounding box.</summary>
    public readonly bool Contains(Vector3Int pos)
    {
        return Contains(pos.X, pos.Y, pos.Z);
    }

    /// <summary>Returns a value to indicate if a point is within the bounding box.</summary>
    public readonly bool Contains(int x, int y, int z)
    {
        return IsInXBounds(x) && IsInYBounds(y) && IsInZBounds(z);
    }

    /// <summary>Returns a value that is the distance from the closest point on the Bounds' border.</summary>
    public readonly int DistanceFromBorder(Vector3Int pos)
    {
        return pos.DistanceManhattanTo(ClosestPointOnBorder(pos));
    }

    /// <summary>Returns a value that is the distance from the closest point on the Bounds' border.</summary>
    public readonly int DistanceFromBorder(int x, int y, int z)
    {
        return new Vector3Int(x, y, z).DistanceManhattanTo(ClosestPointOnBorder(x, y, z));
    }

    /// <summary>Grows the Bounds to include the point.</summary>
    public void Encapsulate(Vector3Int point)
    {
        Encapsulate(point.X, point.Y, point.Z);
    }

    /// <summary>Grows the Bounds to include the point.</summary>
    public void Encapsulate(int x, int y, int z)
    {
        XMin = Math.Min(XMin, x);
        XMax = Math.Max(XMax, x);
        YMin = Math.Min(YMin, y);
        YMax = Math.Max(YMax, y);
        ZMin = Math.Min(ZMin, z);
        ZMax = Math.Max(ZMax, z);
    }

    /// <summary>Expand the bounds by increasing its size by amount along each side.</summary>
    public void Expand(int amount)
    {
        Expand(amount, amount, amount);
    }

    /// <summary>Expand the bounds by increasing its size by each amount along their respective side.</summary>
    public void Expand(int xAmount, int yAmount, int zAmount)
    {
        XMin -= xAmount;
        XMax += xAmount;
        YMin -= yAmount;
        YMax += yAmount;
        ZMin -= zAmount;
        ZMax += zAmount;
    }

    /// <summary>
    ///     Generate all coordinates from Min to Max. e.g. (XMin, YMin), (XMin, YMin + 1), ..., (XMax, YMax - 1), (XMax,
    ///     YMax).
    /// </summary>
    public readonly IEnumerable<Vector3Int> GetAllCoordinates()
    {
        for (var x = XMin; x <= XMax; x++)
        for (var y = YMin; y <= YMax; y++)
        for (var z = ZMin; z < ZMax; z++)
            yield return new Vector3Int(x, y, z);
    }

    /// <summary>Returns true if <paramref name="x" /> is on or inside the Bounds.</summary>
    public readonly bool IsInXBounds(int x)
    {
        return XMin <= x && x <= XMax;
    }

    /// <summary>Returns true if <paramref name="y" /> is on or inside the Bounds.</summary>
    public readonly bool IsInYBounds(int y)
    {
        return YMin <= y && y <= YMax;
    }

    /// <summary>Returns true if <paramref name="z" /> is on or inside the Bounds.</summary>
    public readonly bool IsInZBounds(int z)
    {
        return ZMin <= z && z <= ZMax;
    }

    /// <summary>Returns a value to indicate the position is directly on the edge of the Bounds.</summary>
    public readonly bool IsOnEdge(Vector3Int pos)
    {
        return IsOnEdge(pos.X, pos.Y, pos.Z);
    }

    /// <summary>Returns a value to indicate the position is directly on the edge of the Bounds.</summary>
    public readonly bool IsOnEdge(int x, int y, int z)
    {
        return Contains(x, y, z) && (x == XMin || x == XMax || y == XMin || y == YMax || z == ZMin || z == ZMax);
    }

    /// <summary>Returns a value to indicate if another bounding box intersects or shares an edge with this bounding box.</summary>
    public readonly bool Overlaps(Bounds3D other)
    {
        return XMin <= other.XMax && other.XMin <= XMax && YMin <= other.YMax && other.YMin <= YMax &&
               ZMin <= other.ZMax && other.ZMin <= ZMax;
    }

    /// <summary>Sets the bounds to the min and max value of the box.</summary>
    public void SetMinMax(Vector3Int min, Vector3Int max)
    {
        XMin = min.X;
        XMax = max.X;
        YMin = min.Y;
        YMax = max.Y;
        ZMin = min.Z;
        ZMax = max.Z;
    }
}
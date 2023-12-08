using System;
using System.Collections.Generic;
using System.Numerics;

namespace AoC;

public interface INode<T> : INode
{
    T Value { get; }
}

public interface INode
{
    /// <summary>Coordinate of this Node</summary>
    Vector2Int Pos { get; set; }
    /// <summary>Cost to go to this node. Make sure the magnitude of this lines up with the scale of the HCost (a.k.a. Distance) calculation.</summary>
    int BaseCost { get; set; }
    /// <summary>Total Heuristic for traveling to this node.</summary>
    int F => G + H;
    /// <summary>Cost from Start (all previous Costs + this BaseCost).</summary>
    int G { get; set; }
    /// <summary>Distance to target node. Aids in selecting the next best node.</summary>
    int H { get; set; }
    /// <summary>Used for backtracking when pathfinding. Will be one of its neighbors.</summary>
    INode? Connection { get; set; }
    /// <summary>
    /// Returns the cost to go to the <paramref name="target"/> if it were a clear path. 
    /// Make sure this and the BaseCost have the same magnitude. e.g. BaseCost of 10 for 10*Euclidian, BaseCost of 1 for Manhattan/Chebyshev, etc.
    /// </summary>
    int GetHCostTo(INode target);
    /// <summary>Connected nodes that we can go to from this node (may not be symmetric for directed graphs).</summary>
    IEnumerable<INode> Neighbors { get; }
}

/// <summary>Generic class to represent a point on a grid, and store extra info in Value. Used for A* Pathfinding and Floodfill.</summary>
public class Node<T>(T value, Vector2Int pos, IGrid<T> grid, int cost = 10) : INode<T>
{
    protected readonly IGrid<T> _grid = grid;

    public T Value { get; set; } = value;
    public Vector2Int Pos { get; set; } = pos;
    public int BaseCost { get; set; } = cost;
    public int G { get; set; }
    public int H { get; set; }
    public INode? Connection { get; set; }
    public virtual IEnumerable<INode> Neighbors => _grid.GetNeighborsOf(this);
    public virtual int GetHCostTo(INode target) => (int)Math.Round(10 * Pos.DistanceEuclidianTo(target.Pos)); // Pos.DistanceChebyshevTo(target.Pos); // Pos.DistanceManhattanTo(target.Pos);
    public override int GetHashCode() => Pos.GetHashCode();
}
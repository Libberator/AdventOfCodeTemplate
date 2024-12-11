using System;
using System.Collections.Generic;
using System.Numerics;

namespace AoC;

public interface IGrid<T>
{
    /// <summary>
    ///     Extra constraint to determine if two nodes are connected.
    ///     First argument is primary node, second argument is a prospective neighbor.
    /// </summary>
    public Func<INode<T>, INode<T>, bool> AreValidNeighbors { get; set; }

    /// <summary>Directions to search for neighbors.</summary>
    public Vector2Int[] NeighborDirections { get; set; }

    public IEnumerable<INode<T>> GetNeighborsOf(INode<T> node);
}

public class Grid<T>(T[][] data) : IGrid<T>
{
    private readonly Bounds2D _bounds = new(0, data.Length - 1, 0, data[0].Length - 1);
    private readonly T[][] _data = data;
    private readonly Dictionary<Vector2Int, INode<T>> _nodes = [];

    public Grid(T[][] data, Func<INode<T>, INode<T>, bool> validNeighborCheck) : this(data) =>
        AreValidNeighbors = validNeighborCheck;

    public Grid(T[][] data, Vector2Int[] neighborDirections) : this(data) => NeighborDirections = neighborDirections;

    public T this[int row, int col]
    {
        get => _data[row][col];
        set => _data[row][col] = value;
    }

    public virtual Func<INode<T>, INode<T>, bool> AreValidNeighbors { get; set; } = (node, neighbor) => true;
    public virtual Vector2Int[] NeighborDirections { get; set; } = Vector2Int.CardinalDirections;

    public virtual IEnumerable<INode<T>> GetNeighborsOf(INode<T> node)
    {
        foreach (var dir in NeighborDirections)
            if (TryGetNode(node.Pos + dir, out var neighbor) && AreValidNeighbors(node, neighbor!))
                yield return neighbor!;
    }

    public virtual bool TryGetNode(Vector2Int pos, out INode<T>? node)
    {
        node = default;
        if (!_bounds.Contains(pos)) return false;

        if (!_nodes.TryGetValue(pos, out node))
        {
            node = new Node<T>(_data[pos.X][pos.Y], pos, this);
            _nodes.Add(pos, node);
        }

        return true;
    }

    public bool AddNode(INode<T> node) => _nodes.TryAdd(node.Pos, node);
}
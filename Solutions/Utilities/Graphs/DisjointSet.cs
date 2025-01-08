using System;
using System.Collections.Generic;

namespace AoC.Utilities.Graphs;

/// <summary>
///     Represents a disjoint-set (or union-find) data structure, which is used to keep track of
///     a partition of a set into disjoint subsets.
/// </summary>
/// <typeparam name="T">The type of elements in the disjoint set. Must support equality comparison.</typeparam>
public class DisjointSet<T> where T : notnull
{
    private readonly Dictionary<T, T> _parent;
    private readonly Dictionary<T, int> _rank;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DisjointSet{T}" /> class with the specified elements.
    /// </summary>
    public DisjointSet(IEnumerable<T> elements)
    {
        _parent = new Dictionary<T, T>();
        _rank = new Dictionary<T, int>();
        foreach (var element in elements)
        {
            _parent[element] = element;
            _rank[element] = 0;
        }
    }

    /// <summary>
    ///     Finds the representative (or root) of the set that contains the specified element.
    /// </summary>
    public T Find(T item)
    {
        if (!_parent.TryGetValue(item, out var value))
            throw new ArgumentException($"Item '{item}' does not exist in the disjoint set.", nameof(item));

        if (!value.Equals(item))
            _parent[item] = Find(value); // Path compression

        return _parent[item];
    }

    /// <summary>
    ///     Unites the sets containing the two specified elements into a single set.
    /// </summary>
    public void Union(T item1, T item2)
    {
        var root1 = Find(item1);
        var root2 = Find(item2);

        if (root1.Equals(root2)) return;

        if (_rank[root1] < _rank[root2])
        {
            _parent[root1] = root2;
        }
        else if (_rank[root2] < _rank[root1])
        {
            _parent[root2] = root1;
        }
        else
        {
            _parent[root2] = root1;
            _rank[root1]++;
        }
    }
}
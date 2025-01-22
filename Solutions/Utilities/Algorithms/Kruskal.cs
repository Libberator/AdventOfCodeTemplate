using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC.Utilities.Algorithms;

public static partial class Graph
{
    /// <summary>
    ///     This executes <see href="https://en.wikipedia.org/wiki/Kruskal%27s_algorithm">Kruskal's algorithm</see> to
    ///     find the <see href="https://en.wikipedia.org/wiki/Minimum_spanning_tree">minimum spanning tree</see> of an
    ///     undirected edge-weighted graph.
    /// </summary>
    /// <param name="vertices">List of all the vertices in the graph</param>
    /// <param name="edges">List of all the edges in the edge-weighted graph</param>
    /// <returns>List of all the edges in the minimum spanning tree</returns>
    public static List<Edge<T>> GetMinimumSpanningTree<T>(List<T> vertices, List<Edge<T>> edges) where T : notnull
    {
        // Initialize disjoint set
        var disjointSet = new DisjointSet<T>(vertices);

        var mst = new List<Edge<T>>();

        foreach (var edge in edges.OrderBy(e => e.Weight))
        {
            var rootSource = disjointSet.Find(edge.Source);
            var rootDestination = disjointSet.Find(edge.Destination);

            // If including this edge does not form a cycle
            if (rootSource.Equals(rootDestination)) continue;
            mst.Add(edge);
            disjointSet.Union(rootSource, rootDestination);
        }

        return mst;
    }

    /// <summary>
    ///     This executes <see href="https://en.wikipedia.org/wiki/Kruskal%27s_algorithm">Kruskal's algorithm</see> to
    ///     find the <see href="https://en.wikipedia.org/wiki/Minimum_spanning_tree">minimum spanning tree</see> of an
    ///     undirected edge-weighted graph.
    /// </summary>
    /// <param name="edges">List of all the edges in the edge-weighted graph</param>
    /// <returns>List of all the edges in the minimum spanning tree</returns>
    public static List<Edge<T>> GetMinimumSpanningTree<T>(List<Edge<T>> edges) where T : notnull
    {
        // Get all unique vertices from the list of edges
        var vertices = edges.SelectMany(e => new[] { e.Source, e.Destination }).Distinct().ToList();
        return GetMinimumSpanningTree(vertices, edges);
    }

    /// <summary>
    ///     This executes <see href="https://en.wikipedia.org/wiki/Kruskal%27s_algorithm">Kruskal's algorithm</see> to
    ///     find the <see href="https://en.wikipedia.org/wiki/Minimum_spanning_tree">minimum spanning tree</see> of an
    ///     undirected edge-weighted graph.
    /// </summary>
    /// <param name="edgeCosts">Lookup table of all the vertex-pairs (edges) with their weights in the edge-weighted graph</param>
    /// <returns>List of all the edges in the minimum spanning tree</returns>
    public static List<Edge<T>> GetMinimumSpanningTree<T>(Dictionary<(T From, T To), int> edgeCosts) where T : notnull
    {
        // convert provided edge weights into usable list of vertices and edges
        var vertices = edgeCosts.SelectMany(e => new[] { e.Key.From, e.Key.To }).Distinct().ToList();
        var edgesAsList = new List<Edge<T>>();
        foreach (var ((from, to), weight) in edgeCosts)
            edgesAsList.Add(new Edge<T>(from, to, weight));

        return GetMinimumSpanningTree(vertices, edgesAsList);
    }

    /// <summary>
    ///     This executes <see href="https://en.wikipedia.org/wiki/Kruskal%27s_algorithm">Kruskal's algorithm</see> to
    ///     find the <see href="https://en.wikipedia.org/wiki/Minimum_spanning_tree">minimum spanning tree</see> of an
    ///     edge-weighted complete graph (i.e. any vertex can go to any other vertex).
    /// </summary>
    /// <param name="vertices">List of all the vertices in the graph</param>
    /// <param name="weightFunction">Function to calculate the weight between any two vertices</param>
    /// <returns>List of all the edges in the minimum spanning tree</returns>
    public static List<Edge<T>> GetMinimumSpanningTree<T>(List<T> vertices, Func<T, T, int> weightFunction)
        where T : notnull
    {
        // Generate all unique vertex pairs (edges) in the complete graph (~O(V^2) where V = #vertices)
        var edges = new List<Edge<T>>();
        for (var i = 0; i < vertices.Count; i++)
        {
            var u = vertices[i];
            for (var j = i + 1; j < vertices.Count; j++)
            {
                var v = vertices[j];
                var weight = weightFunction(u, v);
                edges.Add(new Edge<T>(u, v, weight));
            }
        }

        return GetMinimumSpanningTree(vertices, edges);
    }
}

public readonly record struct Edge<T>(T Source, T Destination, int Weight) where T : notnull;

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
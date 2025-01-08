using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC.Utilities.Graphs;

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
    /// <param name="edges">Lookup table of all the vertex-pairs (edges) with their weights in the edge-weighted graph</param>
    /// <returns>List of all the edges in the minimum spanning tree</returns>
    public static List<Edge<T>> GetMinimumSpanningTree<T>(Dictionary<(T, T), int> edges) where T : notnull
    {
        var vertices = edges.SelectMany(e => new[] { e.Key.Item1, e.Key.Item2 }).Distinct().ToList();
        var edgesAsList = new List<Edge<T>>();
        foreach (var ((from, to), weight) in edges)
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
        // Generate all possible edges in the complete graph (expensive! O(v^2) where v = #vertices)
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

    /// <summary>Get the total weight of the edges (typically used for a minimum spanning tree).</summary>
    public static int TotalWeight<T>(this List<Edge<T>> edges) where T : notnull => edges.Sum(edge => edge.Weight);
}
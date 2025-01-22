using System;
using System.Collections.Generic;

namespace AoC.Utilities.Algorithms;

public static partial class Graph
{
    /// <summary>
    ///     Execute the
    ///     <see href="https://en.wikipedia.org/wiki/Floyd%E2%80%93Warshall_algorithm">Floyd-Warshall algorithm</see>
    ///     to find the shortest path cost from each vertex to all other vertices.
    /// </summary>
    /// <param name="adjacencyList">
    ///     An adjacency table to lookup which vertices can be immediately reached from a vertex. Include any end
    ///     vertices with an empty collection
    /// </param>
    /// <param name="getCost">Get the cost between two vertices. Defaults to 1 (unweighted)</param>
    /// <returns>A lookup table of the cost from any vertex to any other vertex</returns>
    public static IDictionary<(T From, T To), int> GetAllShortestCosts<T>(IDictionary<T, HashSet<T>> adjacencyList,
        Func<T, T, int>? getCost = null) where T : notnull
    {
        const int inf = int.MaxValue / 2 - 1;
        getCost ??= (u, v) => adjacencyList[u].Contains(v) ? 1 : inf; // unweighted graph

        var vertices = adjacencyList.Keys;
        var costs = new Dictionary<(T From, T To), int>();

        foreach (var u in vertices)
            foreach (var v in vertices)
                costs.Add((u, v), u.Equals(v) ? 0 : getCost(u, v));

        foreach (var k in vertices)
            foreach (var i in vertices)
                foreach (var j in vertices)
                    if (costs[(i, j)] > costs[(i, k)] + costs[(k, j)])
                        costs[(i, j)] = costs[(i, k)] + costs[(k, j)];

        return costs;
    }
}
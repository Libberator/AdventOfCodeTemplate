using System;
using System.Collections.Generic;

namespace AoC.Utilities.Graphs;

public static partial class Graph
{
    /// <summary>
    ///     Execute the <see href="https://en.wikipedia.org/wiki/Edmonds%E2%80%93Karp_algorithm">Edmonds-Karp algorithm</see>
    ///     (an implementation of the
    ///     <see href="https://en.wikipedia.org/wiki/Ford%E2%80%93Fulkerson_algorithm">Ford-Fulkerson method</see>) to
    ///     compute the max flow in a flow network. This is also equivalent to total weight of the edges in a minimum cut
    ///     via the <see href="https://en.wikipedia.org/wiki/Max-flow_min-cut_theorem">max-flow min-cut theorem</see>.
    /// </summary>
    /// <param name="adjacencyList">A lookup table for which vertices can be immediately reached from a vertex</param>
    /// <param name="capacities">Cost, or maximum capacity, for the edge between two vertices</param>
    /// <param name="source">Start vertex to originate from</param>
    /// <param name="sink">The target vertex to end our flow into</param>
    /// <returns>The maximum flow that can travel through the flow network</returns>
    public static int GetMaxFlow<T>(Dictionary<T, HashSet<T>> adjacencyList, Dictionary<(T, T), int> capacities,
        T source, T sink) where T : notnull
    {
        // Create residual capacities (copy of capacities which we will adjust)
        var residualCapacities = new Dictionary<(T, T), int>(capacities);

        var maxFlow = 0;

        while (true)
        {
            if (!TryFindShortestPath(source, sink, GetNeighbors, out var path)) break; // BFS

            // Find the minimum residual capacity along the path
            var pathFlow = int.MaxValue;
            for (var i = 0; i < path.Count - 1; i++)
            {
                var u = path[i];
                var v = path[i + 1];
                pathFlow = Math.Min(pathFlow, residualCapacities[(u, v)]);
            }

            // Update residual capacities
            for (var i = 0; i < path.Count - 1; i++)
            {
                var u = path[i];
                var v = path[i + 1];
                residualCapacities[(u, v)] -= pathFlow;
                if (!residualCapacities.ContainsKey((v, u)))
                    residualCapacities[(v, u)] = 0;
                residualCapacities[(v, u)] += pathFlow;
            }

            maxFlow += pathFlow;
        }

        return maxFlow;

        IEnumerable<T> GetNeighbors(T node)
        {
            if (!adjacencyList.TryGetValue(node, out var neighbors) || neighbors.Count == 0)
                yield break;
            foreach (var neighbor in neighbors)
                if (residualCapacities.TryGetValue((node, neighbor), out var capacity) && capacity > 0)
                    yield return neighbor;
        }
    }

    /// <summary>
    ///     Execute the <see href="https://en.wikipedia.org/wiki/Edmonds%E2%80%93Karp_algorithm">Edmonds-Karp algorithm</see>
    ///     (an implementation of the
    ///     <see href="https://en.wikipedia.org/wiki/Ford%E2%80%93Fulkerson_algorithm">Ford-Fulkerson method</see>) to
    ///     compute the max flow in a flow network. This is also equivalent to total weight of the edges in a minimum cut
    ///     via the <see href="https://en.wikipedia.org/wiki/Max-flow_min-cut_theorem">max-flow min-cut theorem</see>.
    /// </summary>
    /// <param name="capacities">Cost, or maximum capacity, for the edge between two vertices</param>
    /// <param name="source">Start vertex to originate from</param>
    /// <param name="sink">The target vertex to end our flow into</param>
    /// <returns>The maximum flow that can travel through the flow network</returns>
    public static int GetMaxFlow<T>(Dictionary<(T, T), int> capacities, T source, T sink) where T : notnull
    {
        var adjacencyList = new Dictionary<T, HashSet<T>>();

        foreach (var (from, to) in capacities.Keys)
        {
            if (!adjacencyList.ContainsKey(from))
                adjacencyList[from] = [];
            adjacencyList[from].Add(to);
        }

        return GetMaxFlow(adjacencyList, capacities, source, sink);
    }
}
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
    /// <returns>
    ///     The maximum flow that can travel through the flow network (a.k.a. min cut) and the subgraph with source
    ///     after the cut (the other subgraph can be easily inferred)
    /// </returns>
    public static (int MaxFlowMinCut, IList<T> Subgraph) GetMaxFlowMinCut<T>(Dictionary<T, HashSet<T>> adjacencyList,
        Dictionary<(T From, T To), int> capacities, T source, T sink) where T : notnull
    {
        // Create residual capacities (copy of capacities which we will adjust)
        var residualCapacities = new Dictionary<(T, T), int>(capacities);

        var maxFlow = 0;

        while (true)
        {
            // BFS
            if (!TryFindShortestPath(source, sink, GetNeighbors, out var path))
                return (maxFlow, path);

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
    /// <remarks>This overload will use the capacities to create a directed adjacency list given the (From, To) keys</remarks>
    /// <param name="capacities">Cost, or maximum capacity, for the edge between two vertices</param>
    /// <param name="source">Start vertex to originate from</param>
    /// <param name="sink">The target vertex to end our flow into</param>
    /// <returns>
    ///     The maximum flow that can travel through the flow network (a.k.a. min cut) and the subgraph with source
    ///     after the cut (the other subgraph can be easily inferred)
    /// </returns>
    public static (int MaxFlowMinCut, IList<T> Subgraph) GetMaxFlowMinCut<T>(Dictionary<(T From, T To), int> capacities,
        T source, T sink) where T : notnull
    {
        var adjacencyList = new Dictionary<T, HashSet<T>>();

        foreach (var (from, to) in capacities.Keys)
        {
            adjacencyList.TryAdd(from, []);
            adjacencyList[from].Add(to);
        }

        return GetMaxFlowMinCut(adjacencyList, capacities, source, sink);
    }

    /// <summary>
    ///     Execute the <see href="https://en.wikipedia.org/wiki/Edmonds%E2%80%93Karp_algorithm">Edmonds-Karp algorithm</see>
    ///     (an implementation of the
    ///     <see href="https://en.wikipedia.org/wiki/Ford%E2%80%93Fulkerson_algorithm">Ford-Fulkerson method</see>) to
    ///     compute the max flow in a flow network. This is also equivalent to total weight of the edges in a minimum cut
    ///     via the <see href="https://en.wikipedia.org/wiki/Max-flow_min-cut_theorem">max-flow min-cut theorem</see>.
    /// </summary>
    /// <remarks>This overload creates an unweighted graph. In other words, all capacities will default to 1</remarks>
    /// <param name="adjacencyList">A lookup table for which vertices can be immediately reached from a vertex</param>
    /// <param name="source">Start vertex to originate from</param>
    /// <param name="sink">The target vertex to end our flow into</param>
    /// <returns>
    ///     The maximum flow that can travel through the flow network (a.k.a. min cut) and the subgraph with source
    ///     after the cut (the other subgraph can be easily inferred)
    /// </returns>
    public static (int MaxFlowMinCut, IList<T> Subgraph) GetMaxFlowMinCut<T>(Dictionary<T, HashSet<T>> adjacencyList,
        T source, T sink) where T : notnull
    {
        var capacities = new Dictionary<(T, T), int>();

        foreach (var (from, toSet) in adjacencyList)
            foreach (var to in toSet)
            {
                capacities[(from, to)] = 1;
                capacities[(to, from)] = 1;
            }

        return GetMaxFlowMinCut(adjacencyList, capacities, source, sink);
    }
}
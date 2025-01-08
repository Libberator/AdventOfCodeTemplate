using System.Collections.Generic;
using System.Linq;

namespace AoC.Utilities.Graphs;

public static partial class Graph
{
    /// <summary>
    ///     Execute the <see href="https://en.wikipedia.org/wiki/Bron%E2%80%93Kerbosch_algorithm">Bron-Kerbosch algorithm</see>
    ///     to find the largest clique in an undirected graph (i.e. biggest subset of vertices which are all connected).
    /// </summary>
    /// <param name="adjacencyList">An adjacency table to lookup which vertices can be immediately reached from a vertex</param>
    /// <returns>The set of vertices which form the largest complete (i.e. all vertices connected) subgraph</returns>
    public static HashSet<T> FindMaxClique<T>(Dictionary<T, HashSet<T>> adjacencyList) where T : notnull
    {
        var maxClique = new HashSet<T>();
        BronKerboschRecursive([], [..adjacencyList.Keys], [], adjacencyList, ref maxClique);
        return maxClique;
    }

    // Recursive Bron-Kerbosch algorithm
    private static void BronKerboschRecursive<T>(HashSet<T> currentClique, HashSet<T> candidateNodes,
        HashSet<T> processed, Dictionary<T, HashSet<T>> graph, ref HashSet<T> maxClique) where T : notnull
    {
        // Found a clique
        if (candidateNodes.Count == 0 && processed.Count == 0)
        {
            // Check if it's the largest
            if (currentClique.Count > maxClique.Count)
                maxClique = [..currentClique];
            return;
        }

        // Iterate over a copy to allow modifications
        foreach (var vertex in candidateNodes.ToArray())
        {
            var neighbors = graph[vertex];
            BronKerboschRecursive(
                [..currentClique, vertex],
                [..candidateNodes.Intersect(neighbors)],
                [..processed.Intersect(neighbors)],
                graph, ref maxClique);
            candidateNodes.Remove(vertex);
            processed.Add(vertex);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AoC;

public static class Pathfinding
{
    #region A-Star

    /// <summary>
    /// A* Pathfinding. Returns a list of nodes in reverse order from the target destination (included) to the starting point (excluded).
    /// Before calling this, ensure the Nodes' Neighbors have already been populated.
    /// </summary>
    public static List<INode> FindPath_AStar(INode start, INode end)
    {
        SortedSet<INode> toSearch = new(new AStarHeuristic()) { start };
        HashSet<INode> processed = new();

        while (toSearch.Count > 0)
        {
            var current = toSearch.Min!;

            toSearch.Remove(current);
            processed.Add(current);

            if (current == end)
                return BacktrackRoute(end, start);

            foreach (var neighbor in current.Neighbors)
            {
                if (processed.Contains(neighbor)) continue;

                var inSearch = toSearch.Contains(neighbor);
                var costToNeighbor = current.G + neighbor.BaseCost;

                if (!inSearch || costToNeighbor < neighbor.G)
                {
                    toSearch.Remove(neighbor);

                    neighbor.G = costToNeighbor;
                    neighbor.Connection = current;

                    if (!inSearch)
                        neighbor.H = neighbor.GetHCostTo(end);

                    toSearch.Add(neighbor);
                }
            }
        }
        return new List<INode>();
    }

    private class AStarHeuristic : IComparer<INode>
    {
        public int Compare(INode? current, INode? next) => current!.F != next!.F ? current.F.CompareTo(next.F) :
            current.H != next.H ? current.H.CompareTo(next.H) :
            current.GetHashCode().CompareTo(next.GetHashCode()); // SortedSet requirement to differentiate ties
    }

    #endregion A-Star

    #region Dijkstra / FloodFill/BFS

    /// <summary>
    /// If the graph is unweighted (same cost to go to each node), then this is just Floodfill (a.k.a. Breadth-First Search). 
    /// If it's a weighted graph, then this is Dijkstra. Use this when you don't have a specific singular target in mind.
    /// Returns a list of nodes in reverse order from the node passing the target condition (included) to the starting point (excluded).
    /// </summary>
    public static List<INode> FindPath_Dijkstra(INode start, Predicate<INode> endCondition)
    {
        SortedSet<INode> toSearch = new(new DijkstraHeuristic()) { start };
        HashSet<INode> processed = new();

        while (toSearch.Count > 0)
        {
            var current = toSearch.Min!;

            toSearch.Remove(current);
            processed.Add(current);

            if (endCondition(current))
                return BacktrackRoute(current, start);

            foreach (var neighbor in current.Neighbors)
            {
                if (processed.Contains(neighbor)) continue;

                var inSearch = toSearch.Contains(neighbor);
                var costToNeighbor = current.G + neighbor.BaseCost;

                if (!inSearch || costToNeighbor < neighbor.G)
                {
                    toSearch.Remove(neighbor);

                    neighbor.G = costToNeighbor;
                    neighbor.Connection = current;

                    toSearch.Add(neighbor);
                }
            }
        }
        return new List<INode>();
    }

    private class DijkstraHeuristic : IComparer<INode>
    {
        public int Compare(INode? current, INode? next) => current!.G != next!.G ? current.G.CompareTo(next.G) :
            current.GetHashCode().CompareTo(next.GetHashCode()); // SortedSet requirement to differentiate ties
    }

    #endregion Dijkstra / FloodFill/BFS

    #region Shared Methods

    /// <summary>Returns a list of nodes in reverse order from the target destination (included) to the starting point (excluded).</summary>
    private static List<INode> BacktrackRoute(INode target, INode start)
    {
        var path = new List<INode>();
        var currentNode = target;
        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.Connection!;
        }
        return path;
    }

    #endregion Shared Methods

    // TODO: Consider adding the following...
    // Depth First Search (DFS)
    // Best-First
    // Bi-directional A*
    // Iterative Deeping A* (IDA*)
    // Minimum Spanning Tree (MSP)
    // For reference: https://github.com/brean/python-pathfinding 
}
using System;
using System.Collections.Generic;
using AoC.Utilities.Collections;

namespace AoC.Utilities.Graphs;

public static partial class Graph
{
    #region Floyd-Warshall

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
    public static Dictionary<(T From, T To), int> FindShortestPathCosts<T>(IDictionary<T, HashSet<T>> adjacencyList,
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

    #endregion

    #region Backtracking

    /// <summary>
    ///     Backtracks from <paramref name="goal" /> to <paramref name="start" /> using the <paramref name="previous" />
    ///     loookup table. Reverses the list before returning.
    /// </summary>
    /// <returns>The path from <paramref name="start" /> to <paramref name="goal" />, inclusive</returns>
    public static IList<T> Backtrack<T>(this IDictionary<T, T> previous, T goal, T start) where T : notnull
    {
        List<T> path = [];
        var current = goal;
        while (!current.Equals(start))
        {
            path.Add(current);
            current = previous[current];
        }

        path.Add(start);
        path.Reverse();
        return path;
    }

    #endregion

    #region AStar

    /// <summary>
    ///     This executes the <see href="https://en.wikipedia.org/wiki/A*_search_algorithm">A* search algorithm</see>
    ///     to find the shortest paths between vertices in a weighted graph.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="goal">The target vertex to end our search</param>
    /// <param name="adjacencyList">An adjacency table to lookup which vertices can be immediately reached from a vertex</param>
    /// <param name="getCost">Function to get cost to a neighbor</param>
    /// <param name="getHeuristic">Function to get an estimated min cost from a vertex to the <paramref name="goal" /></param>
    /// <param name="path">
    ///     When true, outputs the path from <paramref name="start" /> to the vertex we stopped at, inclusive.
    ///     Otherwise, this outputs the list of all vertices that can be reached from <paramref name="start" />, inclusive.
    /// </param>
    /// <returns>True if we successfully find the <paramref name="goal" /></returns>
    public static bool FindShortestPath<T>(T start, T goal,
        IDictionary<T, HashSet<T>> adjacencyList, Func<T, T, int> getCost, Func<T, T, int> getHeuristic,
        out IList<T> path) where T : notnull =>
        TryFindShortestPath(start, goal, node => adjacencyList[node], getCost, getHeuristic, out path);

    /// <summary>
    ///     This executes the <see href="https://en.wikipedia.org/wiki/A*_search_algorithm">A* search algorithm</see>
    ///     to find the shortest paths between vertices in a weighted graph.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="goal">The target vertex to end our search</param>
    /// <param name="getNeighbors">Function to get the neighbors of a given vertex</param>
    /// <param name="getCost">Function to get cost to a neighbor</param>
    /// <param name="getHeuristic">Function to get an estimated min cost from a vertex to the <paramref name="goal" /></param>
    /// <param name="path">
    ///     When true, outputs the path from <paramref name="start" /> to the vertex we stopped at, inclusive.
    ///     Otherwise, this outputs the list of all vertices that can be reached from <paramref name="start" />, inclusive.
    /// </param>
    /// <returns>True if we successfully find the <paramref name="goal" /></returns>
    public static bool TryFindShortestPath<T>(T start, T goal, Func<T, IEnumerable<T>> getNeighbors,
        Func<T, T, int> getCost, Func<T, T, int> getHeuristic, out IList<T> path) where T : notnull
    {
        PriorityQueueWithLookup<T, int> queue = new();
        Dictionary<T, T> previous = new();
        Dictionary<T, int> costs = new() { [start] = 0 };

        queue.Enqueue(start, getHeuristic(start, goal));

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current.Equals(goal))
            {
                path = previous.Backtrack(current, start);
                return true;
            }

            foreach (var neighbor in getNeighbors(current))
            {
                var altCost = costs[current] + getCost(current, neighbor);

                if (costs.TryGetValue(neighbor, out var cost) && altCost >= cost) continue;
                previous[neighbor] = current;
                costs[neighbor] = altCost;

                if (queue.Contains(neighbor)) queue.Remove(neighbor);
                queue.Enqueue(neighbor, altCost + getHeuristic(neighbor, goal));
            }
        }

        path = new List<T>(costs.Keys);
        return false;
    }

    #endregion

    #region Breadth-First Search

    /// <summary>
    ///     This executes a <see href="https://en.wikipedia.org/wiki/Breadth-first_search">breadth-first search</see>
    ///     in an unweighted graph.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="goal">The target vertex to end our search</param>
    /// <param name="adjacencyList">An adjacency table to lookup which vertices can be immediately reached from a vertex</param>
    /// <param name="path">
    ///     When true, outputs the path from <paramref name="start" /> to the vertex we stopped at, inclusive.
    ///     Otherwise, this outputs the list of all vertices that can be reached from <paramref name="start" />, inclusive.
    /// </param>
    /// <returns>True if we successfully find the <paramref name="goal" /></returns>
    public static bool TryFindShortestPath<T>(T start, T goal, IDictionary<T, HashSet<T>> adjacencyList,
        out IList<T> path) where T : notnull =>
        TryFindShortestPath(start, node => node.Equals(goal), node => adjacencyList[node], out path);

    /// <summary>
    ///     This executes a <see href="https://en.wikipedia.org/wiki/Breadth-first_search">breadth-first search</see>
    ///     in an unweighted graph.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="stopCondition">A required condition of a vertex to consider the path complete</param>
    /// <param name="adjacencyList">An adjacency table to lookup which vertices can be immediately reached from a vertex</param>
    /// <param name="path">
    ///     When true, outputs the path from <paramref name="start" /> to the vertex we stopped at, inclusive.
    ///     Otherwise, this outputs the list of all vertices that can be reached from <paramref name="start" />, inclusive.
    /// </param>
    /// <returns>True if we successfully meet the <paramref name="stopCondition" /></returns>
    public static bool TryFindShortestPath<T>(T start, Predicate<T> stopCondition,
        IDictionary<T, HashSet<T>> adjacencyList, out IList<T> path) where T : notnull =>
        TryFindShortestPath(start, stopCondition, node => adjacencyList[node], out path);

    /// <summary>
    ///     This executes a <see href="https://en.wikipedia.org/wiki/Breadth-first_search">breadth-first search</see>
    ///     in an unweighted graph.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="goal">The target vertex to end our search</param>
    /// <param name="getNeighbors">Function to get the neighbors of a given vertex</param>
    /// <param name="path">
    ///     When true, outputs the path from <paramref name="start" /> to the vertex we stopped at, inclusive.
    ///     Otherwise, this outputs the list of all vertices that can be reached from <paramref name="start" />, inclusive.
    /// </param>
    /// <returns>True if we successfully find the <paramref name="goal" /></returns>
    public static bool TryFindShortestPath<T>(T start, T goal, Func<T, IEnumerable<T>> getNeighbors, out IList<T> path)
        where T : notnull => TryFindShortestPath(start, node => node.Equals(goal), getNeighbors, out path);

    /// <summary>
    ///     This executes a <see href="https://en.wikipedia.org/wiki/Breadth-first_search">breadth-first search</see>
    ///     in an unweighted graph.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="stopCondition">A required condition of a vertex to consider the path complete</param>
    /// <param name="getNeighbors">Function to get the neighbors of a given vertex</param>
    /// <param name="path">
    ///     When true, outputs the path from <paramref name="start" /> to the vertex we stopped at, inclusive.
    ///     Otherwise, this outputs the list of all vertices that can be reached from <paramref name="start" />, inclusive.
    /// </param>
    /// <returns>True if we successfully meet the <paramref name="stopCondition" /></returns>
    public static bool TryFindShortestPath<T>(T start, Predicate<T> stopCondition, Func<T, IEnumerable<T>> getNeighbors,
        out IList<T> path) where T : notnull
    {
        Queue<T> queue = new();
        HashSet<T> visited = [start];
        Dictionary<T, T> previous = new();

        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (stopCondition(current))
            {
                path = previous.Backtrack(current, start);
                return true;
            }

            foreach (var neighbor in getNeighbors(current))
            {
                if (!visited.Add(neighbor)) continue;
                previous[neighbor] = current;
                queue.Enqueue(neighbor);
            }
        }

        path = new List<T>(visited);
        return false;
    }

    #endregion

    #region Depth-First Search

    /// <summary>
    ///     Execute an iterative <see href="https://en.wikipedia.org/wiki/Depth-first_search">depth-first search</see>
    ///     to determine if it's possible to reach <paramref name="goal" /> from the <paramref name="start" />.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="goal">The target vertex to end our search</param>
    /// <param name="adjacencyList">An adjacency table to lookup which vertices can be immediately reached from a vertex</param>
    /// <returns>True if it's possible to reach the <paramref name="goal" />. False, otherwise</returns>
    public static bool HasCompletePath<T>(T start, T goal, IDictionary<T, HashSet<T>> adjacencyList)
        where T : notnull =>
        HasCompletePath(start, node => node.Equals(goal), node => adjacencyList[node]);

    /// <summary>
    ///     Execute an iterative <see href="https://en.wikipedia.org/wiki/Depth-first_search">depth-first search</see>
    ///     to determine if it's possible to meet a <paramref name="stopCondition" /> from the <paramref name="start" />.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="stopCondition">A required condition of a vertex to consider the path complete</param>
    /// <param name="adjacencyList">An adjacency table to lookup which vertices can be immediately reached from a vertex</param>
    /// <returns>True if the <paramref name="stopCondition" /> is met. False, otherwise</returns>
    public static bool HasCompletePath<T>(T start, Predicate<T> stopCondition, IDictionary<T, HashSet<T>> adjacencyList)
        where T : notnull => HasCompletePath(start, stopCondition, node => adjacencyList[node]);

    /// <summary>
    ///     Execute an iterative <see href="https://en.wikipedia.org/wiki/Depth-first_search">depth-first search</see>
    ///     to determine if it's possible to reach <paramref name="goal" /> from the <paramref name="start" />.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="goal">The target vertex to end our search</param>
    /// <param name="getNeighbors">Function to get the neighbors of a given vertex</param>
    /// <returns>True if it's possible to reach the <paramref name="goal" />. False, otherwise</returns>
    public static bool HasCompletePath<T>(T start, T goal, Func<T, IEnumerable<T>> getNeighbors) where T : notnull =>
        HasCompletePath(start, node => node.Equals(goal), getNeighbors);

    /// <summary>
    ///     Execute an iterative <see href="https://en.wikipedia.org/wiki/Depth-first_search">depth-first search</see>
    ///     to determine if it's possible to meet a <paramref name="stopCondition" /> from the <paramref name="start" />.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="stopCondition">A required condition of a vertex to consider the path complete</param>
    /// <param name="getNeighbors">Function to get the neighbors of a given vertex</param>
    /// <returns>True if the <paramref name="stopCondition" /> is met. False, otherwise</returns>
    public static bool HasCompletePath<T>(T start, Predicate<T> stopCondition, Func<T, IEnumerable<T>> getNeighbors)
        where T : notnull
    {
        Stack<T> stack = new();
        HashSet<T> visited = [start];

        stack.Push(start);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (stopCondition(current)) return true;
            foreach (var neighbor in getNeighbors(current))
            {
                if (!visited.Add(neighbor)) continue;
                stack.Push(neighbor);
            }
        }

        return false;
    }

    #endregion

    #region Dijkstra

    /// <summary>
    ///     This executes the <see href="https://en.wikipedia.org/wiki/Dijkstra's_algorithm">Dijkstra's algorithm</see>,
    ///     finding the shortest path in a weighted graph until the <paramref name="goal" /> is reached.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="goal">The target vertex to end our search</param>
    /// <param name="adjacencyList">An adjacency table to lookup which vertices can be immediately reached from a vertex</param>
    /// <param name="getCost">Function to get cost to a neighbor</param>
    /// <param name="path">
    ///     When true, outputs the path from <paramref name="start" /> to the vertex we stopped at, inclusive.
    ///     Otherwise, this outputs the list of all vertices that can be reached from <paramref name="start" />, inclusive.
    /// </param>
    /// <returns>True if we successfully find the <paramref name="goal" /></returns>
    public static bool TryFindShortestPath<T>(T start, T goal, IDictionary<T, HashSet<T>> adjacencyList,
        Func<T, T, int> getCost, out IList<T> path) where T : notnull =>
        TryFindShortestPath(start, node => node.Equals(goal), node => adjacencyList[node], getCost, out path);

    /// <summary>
    ///     This executes the <see href="https://en.wikipedia.org/wiki/Dijkstra's_algorithm">Dijkstra's algorithm</see>,
    ///     finding the shortest path in a weighted graph until the <paramref name="goal" /> is reached.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="goal">The target vertex to end our search</param>
    /// <param name="getNeighbors">Function to get the neighbors of a given vertex</param>
    /// <param name="getCost">Function to get cost to a neighbor</param>
    /// <param name="path">
    ///     When true, outputs the path from <paramref name="start" /> to the vertex we stopped at, inclusive.
    ///     Otherwise, this outputs the list of all vertices that can be reached from <paramref name="start" />, inclusive.
    /// </param>
    /// <returns>True if we successfully find the <paramref name="goal" /></returns>
    public static bool TryFindShortestPath<T>(T start, T goal, Func<T, IEnumerable<T>> getNeighbors,
        Func<T, T, int> getCost, out IList<T> path) where T : notnull =>
        TryFindShortestPath(start, node => node.Equals(goal), getNeighbors, getCost, out path);

    /// <summary>
    ///     This executes the <see href="https://en.wikipedia.org/wiki/Dijkstra's_algorithm">Dijkstra's algorithm</see>,
    ///     finding the shortest path in a weighted graph until the <paramref name="stopCondition" /> is met.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="stopCondition">The required condition before stopping</param>
    /// <param name="adjacencyList">An adjacency table to lookup which vertices can be immediately reached from a vertex</param>
    /// <param name="getCost">Function to get cost to a neighbor</param>
    /// <param name="path">
    ///     When true, outputs the path from <paramref name="start" /> to the vertex we stopped at, inclusive.
    ///     Otherwise, this outputs the list of all vertices that can be reached from <paramref name="start" />, inclusive.
    /// </param>
    /// <returns>True if we successfully meet the <paramref name="stopCondition" /></returns>
    public static bool TryFindShortestPath<T>(T start, Predicate<T> stopCondition,
        IDictionary<T, HashSet<T>> adjacencyList, Func<T, T, int> getCost, out IList<T> path) where T : notnull =>
        TryFindShortestPath(start, stopCondition, node => adjacencyList[node], getCost, out path);

    /// <summary>
    ///     This executes the <see href="https://en.wikipedia.org/wiki/Dijkstra's_algorithm">Dijkstra's algorithm</see>,
    ///     finding the shortest path in a weighted graph until the <paramref name="stopCondition" /> is met.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="stopCondition">The required condition before stopping</param>
    /// <param name="getNeighbors">Function to get the neighbors of a given vertex</param>
    /// <param name="getCost">Function to get cost to a neighbor</param>
    /// <param name="path">
    ///     When true, outputs the path from <paramref name="start" /> to the vertex we stopped at, inclusive.
    ///     Otherwise, this outputs the list of all vertices that can be reached from <paramref name="start" />, inclusive.
    /// </param>
    /// <returns>True if we successfully meet the <paramref name="stopCondition" /></returns>
    public static bool TryFindShortestPath<T>(T start, Predicate<T> stopCondition, Func<T, IEnumerable<T>> getNeighbors,
        Func<T, T, int> getCost, out IList<T> path) where T : notnull
    {
        PriorityQueueWithLookup<T, int> queue = new();
        Dictionary<T, T> previous = new();
        Dictionary<T, int> costs = new() { [start] = 0 };

        queue.Enqueue(start, 0);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (stopCondition(current))
            {
                path = previous.Backtrack(current, start);
                return true;
            }

            foreach (var neighbor in getNeighbors(current))
            {
                var altCost = costs[current] + getCost(current, neighbor);

                if (costs.TryGetValue(neighbor, out var cost) && altCost >= cost) continue;
                previous[neighbor] = current;
                costs[neighbor] = altCost;

                if (queue.Contains(neighbor)) queue.Remove(neighbor);
                queue.Enqueue(neighbor, altCost);
            }
        }

        path = new List<T>(costs.Keys);
        return false;
    }

    #endregion

    #region Flood Fill

    /// <summary>
    ///     This executes the <see href="https://en.wikipedia.org/wiki/Dijkstra's_algorithm">Dijkstra's algorithm</see>
    ///     for a weighted graph, or <see href="https://en.wikipedia.org/wiki/Breadth-first_search">breadth-first search</see>
    ///     in an unweighted graph. Note: No early exits; this maps out the fully traversable graph from
    ///     <paramref name="start" />.
    /// </summary>
    /// <param name="start">Start vertex to search from</param>
    /// <param name="adjacencyList">An adjacency table to lookup which vertices can be immediately reached from a vertex</param>
    /// <param name="getCost">Function to get cost to a neighbor. Defaults to 1 (unweighted)</param>
    /// <returns>
    ///     The minimum total Costs to get to each vertex from <paramref name="start" /> and a lookup table of
    ///     Previous vertices for <see cref="Backtrack{T}">Backtrack</see>ing.
    /// </returns>
    public static (IDictionary<T, int> Costs, IDictionary<T, T> Previous) FloodFill<T>(T start,
        IDictionary<T, HashSet<T>> adjacencyList, Func<T, T, int>? getCost = null) where T : notnull =>
        FloodFill(start, node => adjacencyList[node], getCost);

    /// <summary>
    ///     This executes the <see href="https://en.wikipedia.org/wiki/Dijkstra's_algorithm">Dijkstra's algorithm</see>
    ///     for a weighted graph, or <see href="https://en.wikipedia.org/wiki/Breadth-first_search">breadth-first search</see>
    ///     in an unweighted graph. Note: No early exits; this maps out the fully traversable graph from
    ///     <paramref name="start" />.
    /// </summary>
    /// <param name="start">Start vertex to search from</param>
    /// <param name="getNeighbors">Function to get the neighbors of a given vertex</param>
    /// <param name="getCost">Function to get cost to a neighbor. Defaults to 1 (unweighted)</param>
    /// <returns>
    ///     The minimum total Costs to get to each vertex from <paramref name="start" /> and a lookup table of
    ///     Previous vertices for <see cref="Backtrack{T}">Backtrack</see>ing.
    /// </returns>
    public static (IDictionary<T, int> Costs, IDictionary<T, T> Previous) FloodFill<T>(T start,
        Func<T, IEnumerable<T>> getNeighbors, Func<T, T, int>? getCost = null) where T : notnull
    {
        getCost ??= (_, _) => 1;

        PriorityQueueWithLookup<T, int> queue = new();
        Dictionary<T, T> previous = new();
        Dictionary<T, int> costs = new() { [start] = 0 };

        queue.Enqueue(start, 0);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var neighbor in getNeighbors(current))
            {
                var altCost = costs[current] + getCost(current, neighbor);

                if (costs.TryGetValue(neighbor, out var cost) && altCost >= cost) continue;
                previous[neighbor] = current;
                costs[neighbor] = altCost;

                if (queue.Contains(neighbor)) queue.Remove(neighbor);
                queue.Enqueue(neighbor, altCost);
            }
        }

        return (costs, previous);
    }

    #endregion
}
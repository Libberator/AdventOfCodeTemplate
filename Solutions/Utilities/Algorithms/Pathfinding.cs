using System;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Collections;

namespace AoC.Utilities.Algorithms;

public static class Pathfinding
{
    #region Backtracking

    /// <summary>
    ///     Backtracks from <paramref name="current" /> to <paramref name="start" /> using the <paramref name="previous" />
    ///     loookup table. Reverses the list before returning.
    /// </summary>
    /// <returns>The path from <paramref name="start" /> to <paramref name="current" />, inclusive</returns>
    public static IList<T> Backtrack<T>(this IDictionary<T, T> previous, T current, T start) where T : notnull
    {
        List<T> path = [];
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

    #region Find Shortest Path

    /// <summary>
    ///     This executes the <see href="https://en.wikipedia.org/wiki/A*_search_algorithm">A* search algorithm</see>
    ///     to find the shortest paths between vertices in a weighted graph.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="goal">The target vertex to end our search</param>
    /// <param name="adjacencyList">An adjacency table to lookup which vertices can be immediately reached from a vertex</param>
    /// <param name="getCost">Function to get cost to a neighbor. First argument is current node, second is neighbor</param>
    /// <param name="getHeuristic">Function to get an estimated min cost from a vertex to the <paramref name="goal" /></param>
    /// <returns>
    ///     Outputs the path from <paramref name="start" /> to the <paramref name="goal" /> if successful, inclusive.
    ///     Otherwise, this outputs an empty list.
    /// </returns>
    public static IList<T> FindShortestPath<T>(T start, T goal, IDictionary<T, HashSet<T>> adjacencyList,
        Func<T, T, int> getCost, Func<T, T, int> getHeuristic) where T : notnull =>
        FindShortestPath(start, goal, node => adjacencyList[node], getCost, getHeuristic);

    /// <summary>
    ///     This executes the <see href="https://en.wikipedia.org/wiki/A*_search_algorithm">A* search algorithm</see>
    ///     to find the shortest paths between vertices in a weighted graph.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="goal">The target vertex to end our search</param>
    /// <param name="getNeighbors">Function to get the neighbors of a given vertex</param>
    /// <param name="getCost">Function to get cost to a neighbor. First argument is current node, second is neighbor</param>
    /// <param name="getHeuristic">Function to get an estimated min cost from a vertex to the <paramref name="goal" /></param>
    /// <returns>
    ///     Outputs the path from <paramref name="start" /> to the <paramref name="goal" /> if successful, inclusive.
    ///     Otherwise, this outputs an empty list.
    /// </returns>
    public static IList<T> FindShortestPath<T>(T start, T goal, Func<T, IEnumerable<T>> getNeighbors,
        Func<T, T, int> getCost, Func<T, T, int> getHeuristic) where T : notnull
    {
        PriorityQueueWithLookup<T, int> queue = new();
        Dictionary<T, T> previous = new();
        Dictionary<T, int> costs = new() { [start] = 0 };

        queue.Enqueue(start, getHeuristic(start, goal));

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current.Equals(goal))
                return previous.Backtrack(current, start);

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

        return [];
    }

    /// <summary>
    ///     This executes a <see href="https://en.wikipedia.org/wiki/Breadth-first_search">breadth-first search</see>
    ///     in an unweighted graph.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="goal">The target vertex to end our search</param>
    /// <param name="adjacencyList">An adjacency table to lookup which vertices can be immediately reached from a vertex</param>
    /// <returns>
    ///     Outputs the path from <paramref name="start" /> to the <paramref name="goal" /> if successful, inclusive.
    ///     Otherwise, this outputs an empty list.
    /// </returns>
    public static IList<T> FindShortestPath<T>(T start, T goal, IDictionary<T, HashSet<T>> adjacencyList)
        where T : notnull =>
        FindShortestPath(start, node => node.Equals(goal), node => adjacencyList[node]);

    /// <summary>
    ///     This executes a <see href="https://en.wikipedia.org/wiki/Breadth-first_search">breadth-first search</see>
    ///     in an unweighted graph.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="stopCondition">A required condition of a vertex to consider the path complete</param>
    /// <param name="adjacencyList">An adjacency table to lookup which vertices can be immediately reached from a vertex</param>
    /// <returns>
    ///     Outputs the path from <paramref name="start" /> to the node we stopped at if successful, inclusive.
    ///     Otherwise, this outputs an empty list.
    /// </returns>
    public static IList<T> FindShortestPath<T>(T start, Predicate<T> stopCondition,
        IDictionary<T, HashSet<T>> adjacencyList) where T : notnull =>
        FindShortestPath(start, stopCondition, node => adjacencyList[node]);

    /// <summary>
    ///     This executes a <see href="https://en.wikipedia.org/wiki/Breadth-first_search">breadth-first search</see>
    ///     in an unweighted graph.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="goal">The target vertex to end our search</param>
    /// <param name="getNeighbors">Function to get the neighbors of a given vertex</param>
    /// <returns>
    ///     Outputs the path from <paramref name="start" /> to the <paramref name="goal" /> if successful, inclusive.
    ///     Otherwise, this outputs an empty list.
    /// </returns>
    public static IList<T> FindShortestPath<T>(T start, T goal, Func<T, IEnumerable<T>> getNeighbors)
        where T : notnull => FindShortestPath(start, node => node.Equals(goal), getNeighbors);

    /// <summary>
    ///     This executes a <see href="https://en.wikipedia.org/wiki/Breadth-first_search">breadth-first search</see>
    ///     in an unweighted graph.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="stopCondition">A required condition of a vertex to consider the path complete</param>
    /// <param name="getNeighbors">Function to get the neighbors of a given vertex</param>
    /// <returns>
    ///     Outputs the path from <paramref name="start" /> to the node we stopped at if successful, inclusive.
    ///     Otherwise, this outputs an empty list.
    /// </returns>
    public static IList<T> FindShortestPath<T>(T start, Predicate<T> stopCondition,
        Func<T, IEnumerable<T>> getNeighbors) where T : notnull
    {
        Queue<T> queue = new();
        HashSet<T> visited = [start];
        Dictionary<T, T> previous = new();

        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (stopCondition(current))
                return previous.Backtrack(current, start);

            foreach (var neighbor in getNeighbors(current))
            {
                if (!visited.Add(neighbor)) continue;
                previous[neighbor] = current;
                queue.Enqueue(neighbor);
            }
        }

        return [];
    }

    /// <summary>
    ///     This executes the <see href="https://en.wikipedia.org/wiki/Dijkstra's_algorithm">Dijkstra's algorithm</see>,
    ///     finding the shortest path in a weighted graph until the <paramref name="goal" /> is reached.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="goal">The target vertex to end our search</param>
    /// <param name="adjacencyList">An adjacency table to lookup which vertices can be immediately reached from a vertex</param>
    /// <param name="getCost">Function to get cost to a neighbor. First argument is current node, second is neighbor</param>
    /// <returns>
    ///     Outputs the path from <paramref name="start" /> to the <paramref name="goal" /> if successful, inclusive.
    ///     Otherwise, this outputs an empty list.
    /// </returns>
    public static IList<T> FindShortestPath<T>(T start, T goal, IDictionary<T, HashSet<T>> adjacencyList,
        Func<T, T, int> getCost) where T : notnull =>
        FindShortestPath(start, node => node.Equals(goal), node => adjacencyList[node], getCost);

    /// <summary>
    ///     This executes the <see href="https://en.wikipedia.org/wiki/Dijkstra's_algorithm">Dijkstra's algorithm</see>,
    ///     finding the shortest path in a weighted graph until the <paramref name="goal" /> is reached.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="goal">The target vertex to end our search</param>
    /// <param name="getNeighbors">Function to get the neighbors of a given vertex</param>
    /// <param name="getCost">Function to get cost to a neighbor. First argument is current node, second is neighbor</param>
    /// <returns>
    ///     Outputs the path from <paramref name="start" /> to the <paramref name="goal" /> if successful, inclusive.
    ///     Otherwise, this outputs an empty list.
    /// </returns>
    public static IList<T> FindShortestPath<T>(T start, T goal, Func<T, IEnumerable<T>> getNeighbors,
        Func<T, T, int> getCost) where T : notnull =>
        FindShortestPath(start, node => node.Equals(goal), getNeighbors, getCost);

    /// <summary>
    ///     This executes the <see href="https://en.wikipedia.org/wiki/Dijkstra's_algorithm">Dijkstra's algorithm</see>,
    ///     finding the shortest path in a weighted graph until the <paramref name="stopCondition" /> is met.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="stopCondition">The required condition before stopping</param>
    /// <param name="adjacencyList">An adjacency table to lookup which vertices can be immediately reached from a vertex</param>
    /// <param name="getCost">Function to get cost to a neighbor. First argument is current node, second is neighbor</param>
    /// <returns>
    ///     Outputs the path from <paramref name="start" /> to the node we stopped at if successful, inclusive.
    ///     Otherwise, this outputs an empty list.
    /// </returns>
    public static IList<T> FindShortestPath<T>(T start, Predicate<T> stopCondition,
        IDictionary<T, HashSet<T>> adjacencyList, Func<T, T, int> getCost) where T : notnull =>
        FindShortestPath(start, stopCondition, node => adjacencyList[node], getCost);

    /// <summary>
    ///     This executes the <see href="https://en.wikipedia.org/wiki/Dijkstra's_algorithm">Dijkstra's algorithm</see>,
    ///     finding the shortest path in a weighted graph until the <paramref name="stopCondition" /> is met.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="stopCondition">The required condition before stopping</param>
    /// <param name="getNeighbors">Function to get the neighbors of a given vertex</param>
    /// <param name="getCost">Function to get cost to a neighbor. First argument is current node, second is neighbor</param>
    /// <returns>
    ///     Outputs the path from <paramref name="start" /> to the node we stopped at if successful, inclusive.
    ///     Otherwise, this outputs an empty list.
    /// </returns>
    public static IList<T> FindShortestPath<T>(T start, Predicate<T> stopCondition,
        Func<T, IEnumerable<T>> getNeighbors, Func<T, T, int> getCost) where T : notnull
    {
        PriorityQueueWithLookup<T, int> queue = new();
        Dictionary<T, T> previous = new();
        Dictionary<T, int> costs = new() { [start] = 0 };

        queue.Enqueue(start, 0);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (stopCondition(current))
                return previous.Backtrack(current, start);

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

        return [];
    }

    #endregion

    #region Find Any Path

    /// <summary>
    ///     This executes a <see href="https://en.wikipedia.org/wiki/Depth-first_search">depth-first search</see>
    ///     to find any path (not guaranteed to be shortest) between two vertices in a graph.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="goal">The target vertex to end our search</param>
    /// <param name="adjacencyList">An adjacency table to lookup which vertices can be immediately reached from a vertex</param>
    /// <param name="getHeuristic">Optional function to get an estimated cost from any vertex to the goal. Makes it Greedy DFS</param>
    /// <returns>
    ///     Outputs the path from <paramref name="start" /> to the <paramref name="goal" /> if successful, inclusive.
    ///     Otherwise, this outputs an empty list.
    /// </returns>
    public static IList<T> FindAnyPath<T>(T start, T goal, IDictionary<T, HashSet<T>> adjacencyList,
        Func<T, int>? getHeuristic = null) where T : notnull =>
        FindAnyPath(start, node => node.Equals(goal), node => adjacencyList[node], getHeuristic);

    /// <summary>
    ///     This executes a <see href="https://en.wikipedia.org/wiki/Depth-first_search">depth-first search</see>
    ///     to find any path (not guaranteed to be shortest) between two vertices in a graph.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="stopCondition">A required condition of a vertex to consider the path complete</param>
    /// <param name="adjacencyList">An adjacency table to lookup which vertices can be immediately reached from a vertex</param>
    /// <param name="getHeuristic">Optional function to get an estimated cost from any vertex to the goal. Makes it Greedy DFS</param>
    /// <returns>
    ///     Outputs the path from <paramref name="start" /> to the node we stopped at if successful, inclusive.
    ///     Otherwise, this outputs an empty list.
    /// </returns>
    public static IList<T> FindAnyPath<T>(T start, Predicate<T> stopCondition, IDictionary<T, HashSet<T>> adjacencyList,
        Func<T, int>? getHeuristic = null) where T : notnull =>
        FindAnyPath(start, stopCondition, node => adjacencyList[node], getHeuristic);

    /// <summary>
    ///     This executes a <see href="https://en.wikipedia.org/wiki/Depth-first_search">depth-first search</see>
    ///     to find any path (not guaranteed to be shortest) between two vertices in a graph.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="goal">The target vertex to end our search</param>
    /// <param name="getNeighbors">Function to get the neighbors of a given vertex</param>
    /// <param name="getHeuristic">Optional function to get an estimated cost from any vertex to the goal. Makes it Greedy DFS</param>
    /// <returns>
    ///     Outputs the path from <paramref name="start" /> to the <paramref name="goal" /> if successful, inclusive.
    ///     Otherwise, this outputs an empty list.
    /// </returns>
    public static IList<T> FindAnyPath<T>(T start, T goal, Func<T, IEnumerable<T>> getNeighbors,
        Func<T, int>? getHeuristic = null) where T : notnull =>
        FindAnyPath(start, node => node.Equals(goal), getNeighbors, getHeuristic);

    /// <summary>
    ///     This executes a <see href="https://en.wikipedia.org/wiki/Depth-first_search">depth-first search</see>
    ///     to find any path (not guaranteed to be shortest) between two vertices in a graph.
    /// </summary>
    /// <param name="start">The start vertex to search from</param>
    /// <param name="stopCondition">A required condition of a vertex to consider the path complete</param>
    /// <param name="getNeighbors">Function to get the neighbors of a given vertex</param>
    /// <param name="getHeuristic">Optional function to get an estimated cost from any vertex to the goal. Makes it Greedy DFS</param>
    /// <returns>
    ///     Outputs the path from <paramref name="start" /> to the node we stopped at if successful, inclusive.
    ///     Otherwise, this outputs an empty list.
    /// </returns>
    public static IList<T> FindAnyPath<T>(T start, Predicate<T> stopCondition, Func<T, IEnumerable<T>> getNeighbors,
        Func<T, int>? getHeuristic = null) where T : notnull
    {
        getHeuristic ??= _ => 0;
        Stack<T> stack = new();
        HashSet<T> visited = [start];
        Dictionary<T, T> previous = new();
        stack.Push(start);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (stopCondition(current))
                return Backtrack(previous, current, start);

            foreach (var neighbor in getNeighbors(current).OrderByDescending(getHeuristic))
            {
                if (!visited.Add(neighbor)) continue;
                previous[neighbor] = current;
                stack.Push(neighbor);
            }
        }

        return [];
    }

    #endregion

    #region Flood Fill

    /// <summary>
    ///     This executes a <see href="https://en.wikipedia.org/wiki/Depth-first_search">depth-first search</see>.
    ///     Note: No early exits; this maps out the fully traversable graph from <paramref name="start" />.
    /// </summary>
    /// <param name="start">Start vertex to search from</param>
    /// <param name="adjacencyList">An adjacency table to lookup which vertices can be immediately reached from a vertex</param>
    /// <returns>All the reachable nodes/vertices from <paramref name="start" /> as a list</returns>
    public static IList<T> FloodFill<T>(T start, IDictionary<T, HashSet<T>> adjacencyList) where T : notnull =>
        FloodFill(start, node => adjacencyList[node]);

    /// <summary>
    ///     This executes a <see href="https://en.wikipedia.org/wiki/Depth-first_search">depth-first search</see>.
    ///     Note: No early exits; this maps out the fully traversable graph from <paramref name="start" />.
    /// </summary>
    /// <param name="start">Start vertex to search from</param>
    /// <param name="getNeighbors">Function to get the neighbors of a given vertex</param>
    /// <returns>All the reachable nodes/vertices from <paramref name="start" /></returns>
    public static IList<T> FloodFill<T>(T start, Func<T, IEnumerable<T>> getNeighbors) where T : notnull
    {
        Stack<T> stack = new();
        HashSet<T> visited = [start];
        stack.Push(start);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            foreach (var neighbor in getNeighbors(current))
            {
                if (!visited.Add(neighbor)) continue;
                stack.Push(neighbor);
            }
        }

        return [..visited];
    }

    #endregion
}
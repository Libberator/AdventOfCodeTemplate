namespace AoC.Utilities.Graphs;

public readonly record struct Edge<T>(T Source, T Destination, int Weight) where T : notnull;
using System;
using System.Diagnostics.CodeAnalysis;

namespace AoC.Solutions;

/// <summary>Interface all solutions will implement.</summary>
public interface ISolver
{
    public void Setup(string[] input);
    public object? SolvePart1();
    public object? SolvePart2() => null;
}

public static class SolverFactory
{
    // unused, but here's how you *could* get Year and Day from an instance of a Solution
    // public static int Year(this ISolver solver) => int.Parse(solver.GetType().Namespace![^8..^4]);
    // public static int Day(this ISolver solver) => int.Parse(solver.GetType().Namespace![^2..]);

    public static bool TryCreateSolver(int year, int day, [NotNullWhen(true)] out ISolver? solver)
    {
        solver = CreateSolver(year, day);
        return solver != null;
    }

    private static ISolver? CreateSolver(int year, int day)
    {
        var assembly = typeof(ISolver).Assembly;
        var nameSpace = $"{typeof(ISolver).Namespace}.Y{year}.D{day:D2}";
        var fullyQualifiedName = $"{nameSpace}.Solution";
        var type = assembly.GetType(fullyQualifiedName);

        return type != null ? Activator.CreateInstance(type) as ISolver : null;
    }
}
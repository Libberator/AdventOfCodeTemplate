using AoC;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
public class PuzzleBenchmarks
{
    private readonly ILogger _logger = new BenchmarkLogger();
    private string _path = string.Empty;

    [GlobalSetup]
    public void Setup()
    {
        _path = Utils.FullPath(1);
    }

    [Benchmark]
    public void MyPuzzle()
    {
        var puzzle = new Day1(_logger, _path);
        puzzle.Setup();
        puzzle.SolvePart1();
        puzzle.SolvePart2();
    }
}

public class BenchmarkLogger : ILogger
{
    public string? LastMessage => string.Empty;
    public void Log(string? msg) { }
}
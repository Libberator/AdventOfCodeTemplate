using System.Collections.Generic;

namespace AoC;

/// <summary>Base Class for every puzzle.</summary>
public abstract class Puzzle
{
    protected readonly ILogger _logger;
    protected readonly string _path;
    public Puzzle(ILogger logger, string path)
    {
        _logger = logger;
        _path = path;
    }

    protected string[] ReadAllLines() => Utils.ReadAllLines(_path);
    protected IEnumerable<string> ReadFromFile(bool ignoreWhiteSpace = false) => Utils.ReadFrom(_path, ignoreWhiteSpace);

    public abstract void Setup();
    public abstract void SolvePart1();
    public abstract void SolvePart2();
}
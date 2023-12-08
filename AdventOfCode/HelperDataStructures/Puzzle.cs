using System.Collections.Generic;

namespace AoC;

/// <summary>Base Class for every puzzle.</summary>
public abstract class Puzzle(ILogger logger, string path)
{
    protected readonly ILogger _logger = logger;
    protected readonly string _path = path;

    protected string[] ReadAllLines() => Utils.ReadAllLines(_path);
    protected IEnumerable<string> ReadFromFile(bool ignoreWhiteSpace = false) => Utils.ReadFrom(_path, ignoreWhiteSpace);

    public abstract void Setup();
    public abstract void SolvePart1();
    public abstract void SolvePart2();
}
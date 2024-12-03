using System.Collections.Generic;

namespace AoC;

/// <summary>Base Class for every puzzle.</summary>
public abstract class Puzzle(ILogger logger, string path)
{
    public abstract void Setup();
    public abstract void SolvePart1();
    public abstract void SolvePart2();

    protected string[] ReadAllLines()
    {
        return Utils.ReadAllLines(path);
    }

    protected IEnumerable<string> ReadFromFile(bool ignoreWhiteSpace = false)
    {
        return Utils.ReadFrom(path, ignoreWhiteSpace);
    }

    protected void Answer(string answer)
    {
        logger.Log(answer);
    }

    protected void Answer(int answer)
    {
        logger.Log(answer);
    }

    protected void Answer(long answer)
    {
        logger.Log(answer);
    }

    protected void Answer(float answer)
    {
        logger.Log(answer);
    }

    protected void Answer(double answer)
    {
        logger.Log(answer);
    }

    protected void Answer(char answer)
    {
        logger.Log(answer);
    }

    protected void Answer(object answer)
    {
        logger.Log(answer);
    }
}
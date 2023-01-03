namespace AoC;

public class Day1 : Puzzle
{
    public Day1(ILogger logger, string path) : base(logger, path) { }

    public override void Setup()
    {
        // Access file with ReadFromFile() for 1 line at a time or ReadAllLines() as a string dump.
        // For example:

        //foreach (var line in ReadFromFile())
        //{
        //    // parse the input.txt here line-by-line
        //}

        // or...

        //var data = ReadAllLines();
    }

    public override void SolvePart1()
    {
        _logger.Log("Part 1 Answer");
    }

    public override void SolvePart2()
    {
        _logger.Log("Part 2 Answer");
    }
}
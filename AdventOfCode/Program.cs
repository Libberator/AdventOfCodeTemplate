#define USE_STOPWATCH
using System;
using System.Diagnostics;
using AoC;

const int START_DAY = 1;
const int STOP_DAY = 25;

var logger = new ConsoleLogger();

for (var i = START_DAY; i <= STOP_DAY; i++)
{
    Puzzle puzzle;
    try
    {
        puzzle = Utils.GetClassOfType<Puzzle>($"Day{i}", logger, Utils.FullPath(i));
        logger.Log($"\e[32m-- Day {i} --\e[0m");
    }
    catch (Exception) // e)
    {
        //logger.Log(e.Message);
        continue;
    }

#if USE_STOPWATCH // Note: Use Benchmarks instead if you're looking for a more accurate performance measurement
    var timer = new Stopwatch();

    timer.Start();
    puzzle.Setup();
    var setup = timer.ElapsedMilliseconds;

    timer.Restart();
    puzzle.SolvePart1();
    var part1 = timer.ElapsedMilliseconds;

    timer.Restart();
    puzzle.SolvePart2();
    var part2 = timer.ElapsedMilliseconds;

    logger.Log($"Setup: {setup}ms. Part1: {part1}ms. Part2: {part2}ms. Total: {setup + part1 + part2}ms");
#else
    puzzle.Setup();
    puzzle.SolvePart1();
    puzzle.SolvePart2();
#endif
}

#if !DEBUG
Console.ReadLine(); // prevent closing a build automatically
#endif
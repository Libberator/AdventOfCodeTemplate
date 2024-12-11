#define USE_STOPWATCH // Comment out if you don't want times displayed
using System;
using System.Diagnostics;
using AoC;

const int startDay = 1;
const int stopDay = 25;

var logger = new ConsoleLogger();

for (var i = startDay; i <= stopDay; i++)
{
    Solver solver;
    try
    {
        solver = Utils.GetClassOfType<Solver>($"Day{i}", logger, Utils.FullPath(i));
        logger.Log($"\e[32m-- Day {i} --\e[0m");
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        continue;
    }

#if USE_STOPWATCH // Note: Use Benchmarks instead if you're looking for a more accurate performance measurement
    var timer = new Stopwatch();

    timer.Start();
    solver.Setup();
    var setup = timer.ElapsedMilliseconds;

    timer.Restart();
    solver.SolvePart1();
    var part1 = timer.ElapsedMilliseconds;

    timer.Restart();
    solver.SolvePart2();
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
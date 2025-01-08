using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AoC.Solutions;
using AoC.Utilities.Extensions;

namespace AoC.Automation;

public class SolverRunner(AoCServiceProvider aoCServiceProvider)
{
    public async Task RunAll(int[] years, int[] days, bool noFetch, bool noTimer)
    {
        foreach (var year in years)
            foreach (var day in days)
                await Run(year, day, null, noFetch, noTimer);
    }

    public async Task Run(int year, int day, string? inputPath, bool noFetch, bool noTimer, int submitPart = -1, bool wait = false)
    {
        ThrowHelper.ThrowIfOutOfRange(year, 2015, DateTime.Now.Year);
        ThrowHelper.ThrowIfOutOfRange(day, 1, 25);
        inputPath = PathHelper.ValidateInputPath(year, day, inputPath);

        // Generate Solver file if it doesn't exist
        var generatedSolver = SolverTemplate.Generate(year, day);
        if (generatedSolver)
            Logger.Log($"Notice: The Assembly must be reloaded (i.e. run it again) before the generated solver " +
                       $"'Solutions/{year}/{day:D2}/Solution.cs' can be used", ConsoleColor.Yellow);

        // Check locally for input file. If it's not found, and we can fetch, fetch for it
        var hasInput = File.Exists(inputPath);
        if (!hasInput && !noFetch) hasInput = await aoCServiceProvider.Fetch(year, day, inputPath);
        if (!hasInput)
        {
            Logger.Log($"No input file found at '{inputPath}'", ConsoleColor.Red);
            if (noFetch)
                Logger.Log($"Fix: Create an 'input.txt' file in this location, provide a valid `--input` file path, " +
                           $"OR remove `--no-fetch` flag to let us fetch it for you :)", ConsoleColor.Yellow);
            return;
        }

        // Return if we just generated a Solver or can't create an instance
        if (generatedSolver || !SolverFactory.TryCreateSolver(year, day, out var solver)) return;

        Logger.Log($"-- Year {year} Day {day} --", ConsoleColor.Green);
        var input = await File.ReadAllLinesAsync(inputPath);
        var (answer1, answer2) = Solve(solver, input, noTimer);

        switch (submitPart)
        {
            case 1: await aoCServiceProvider.Submit(year, day, submitPart, answer1, wait); break;
            case 2: await aoCServiceProvider.Submit(year, day, submitPart, answer2, wait); break;
        }
    }

    private static (string? Answer1, string? Answer2) Solve(ISolver solver, string[] input, bool noTimer)
    {
        var timer = new Stopwatch();
        timer.Restart();
        solver.Setup(input);
        var setup = timer.Elapsed;

        timer.Restart();
        var p1Result = solver.SolvePart1();
        var part1 = timer.Elapsed;

        timer.Restart();
        var p2Result = solver.SolvePart2();
        var part2 = timer.Elapsed;

        Logger.Log($"Part 1: {p1Result}", ConsoleColor.White);
        Logger.Log($"Part 2: {p2Result}", ConsoleColor.White);

        if (!noTimer) LogTimes(("Setup", setup), ("Part 1", part1), ("Part 2", part2));

        return (p1Result?.ToString(), p2Result?.ToString());
    }

    private static void LogTimes(params (string Label, TimeSpan Duration)[] times)
    {
        var total = times.Sum(t => t.Duration.TotalMilliseconds);
        var maxDuration = times.Max(t => t.Duration.TotalMilliseconds);
        var maxBarWidth = Math.Min(50, Console.WindowWidth - 30);
        var sb = new StringBuilder("\n");

        foreach (var (label, duration) in times)
        {
            var barWidth = (int)Math.Round(maxBarWidth * duration.TotalMilliseconds / maxDuration);
            sb.AppendLine($"{label.PadRight(11)}|{'█'.Repeat(barWidth).PadRight(maxBarWidth)}| {Format(duration)}");
        }

        var totalText = $"Total time | {FormatTotal(total)}";
        sb.Append(totalText.PadLeft(maxBarWidth + totalText.Length + 1));
        Logger.Log(sb.ToString(), ConsoleColor.Yellow);
    }

    private static string FormatTotal(double ms) => ms >= 1000 ? $"{ms / 1000:N}s"
        : ms >= 1 ? $"{ms:N}ms" : $"{ms * 1000:N}µs";

    private static string Format(TimeSpan time) => time.TotalSeconds >= 1f ? $"{time.TotalSeconds:N}s"
        : time.TotalMilliseconds >= 1 ? $"{time.TotalMilliseconds:N}ms" : $"{time.TotalMicroseconds:N}µs";
}
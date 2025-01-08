using System;
using System.IO;
using AoC.Solutions;

namespace AoC.Automation;

public static class SolverTemplate
{
    public static void Generate(int[] years, int[] days, bool force = false)
    {
        foreach (var year in years)
            foreach (var day in days)
                Generate(year, day, force);
    }

    /// <summary>
    ///     Returns true if we successfully created a new Solution.cs file in the Solutions project.
    /// </summary>
    public static bool Generate(int year, int day, bool force = false)
    {
        // allowing 1 year ahead seems generous enough ¯\_(ツ)_/¯
        ThrowHelper.ThrowIfOutOfRange(year, 2015, DateTime.Now.Year + 1);
        ThrowHelper.ThrowIfOutOfRange(day, 1, 25);

        var fileDir = PathHelper.GetDirectoryForDay(year, day);
        if (!Directory.Exists(fileDir)) Directory.CreateDirectory(fileDir);

        var filePath = PathHelper.GetFullFilePath(year, day, "Solution.cs");
        if (!force && File.Exists(filePath)) return false;

        var msg = force && File.Exists(filePath) ? "[NOTICE] Forcefully overwrote" : "Created a new";
        File.WriteAllText(filePath, NewTemplate(year, day));
        Logger.Log($"{msg} script at '{filePath}'", ConsoleColor.DarkGreen);
        return true;
    }

    private static string NewTemplate(int year, int day) =>
        $$"""
          namespace {{typeof(ISolver).Namespace}}.Y{{year}}.D{{day:D2}};

          public class Solution : ISolver
          {
              public void Setup(string[] input)
              {
                  // process input
              }
          
              public object SolvePart1()
              {
                  return "Part 1";
              }
          
              public object SolvePart2()
              {
                  return "Part 2";
              }
          }
          """;
}
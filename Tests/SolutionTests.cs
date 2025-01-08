using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AoC.Solutions;
using NUnit.Framework;

// ReSharper disable UnusedMember.Local

namespace Aoc.Tests;

/// <summary>
///     Automates tests to match what solutions are available. To generate test files, run the standard tests
///     (it will call the generator if it's missing test files) or run the Generator method manually.
/// </summary>
public static class SolutionTests
{
    /* Adjust year(s) here */
    private const int StartYear = 2015;

    private const int StopYear = 2024;

    /* Can also create array manually if you want to test a sparse selection: Years = [2021, 2023, 2024]; */
    private static readonly IEnumerable<int> Years = Enumerable.Range(StartYear, StopYear - StartYear + 1);

    [Test]
    [TestCaseSource(nameof(GenerateTestsForYears))]
    public static void TestSolution(ISolver solver, AoCTestCase testCase)
    {
        if (testCase is { Part1Expected: null, Part2Expected: null })
            Assert.Warn("Missing expected results.");

        // Arrange
        var assertions = new List<Action>();
        var input = testCase.Input.Split('\r', '\n');
        var expected1 = testCase.Part1Expected;
        var expected2 = testCase.Part2Expected;

        // Act
        solver.Setup(input);
        var part1Result = solver.SolvePart1()?.ToString();
        var part2Result = solver.SolvePart2()?.ToString();

        // Assert
        if (!string.IsNullOrEmpty(expected1))
            assertions.Add(() =>
                Assert.That(part1Result, Is.EqualTo(expected1), "Part 1 does not match expected results."));

        if (!string.IsNullOrEmpty(expected2))
            assertions.Add(() =>
                Assert.That(part2Result, Is.EqualTo(expected2), "Part 2 does not match expected results."));

        Assert.Multiple(() => assertions.ForEach(assertion => assertion.Invoke()));
    }

    private static IEnumerable<TestCaseData> GenerateTestsForYears() => Years.SelectMany(GenerateTestsForYear);

    private static IEnumerable<TestCaseData> GenerateTestsForYear(int year)
    {
        foreach (var (day, tests) in TestCaseFileManager.GetTestCasesYear(year))
        {
            var i = 1;
            foreach (var aoCTestCase in tests)
            {
                if (!SolverFactory.TryCreateSolver(year, day, out var solver)) break;
                solver.ApplyTestValues(); // overwrites fields and properties with [TestValue] value
                yield return new TestCaseData(solver, aoCTestCase)
                    .SetCategory($"Y{year}")
                    .SetName($"Y{year}.D{day:D2}{(i > 1 ? $".{i}" : string.Empty)}");
                i++;
            }
        }
    }

    [Test]
    [Explicit]
    [Category("Generator")]
    public static void GenerateMissingTestFilesAndFolders()
    {
        foreach (var definedType in typeof(ISolver).Assembly.DefinedTypes)
        {
            if (definedType.IsInterface || !typeof(ISolver).IsAssignableFrom(definedType)) continue;

            var match = Regex.Match(definedType.Namespace!, @"Y(\d{4})\.D(\d{2})");
            if (!match.Success) continue;

            var year = int.Parse(match.Groups[1].ValueSpan);
            var day = int.Parse(match.Groups[2].ValueSpan);

            TestCaseFileManager.EnsureTestCasesExist(year, day);
        }
    }
}
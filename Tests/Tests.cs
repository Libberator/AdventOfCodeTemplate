using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace AoC.Tests;

public class Tests
{
    const int START_DAY = 1;
    const int STOP_DAY = 25;

    private static readonly ILogger _logger = new TestLogger();

    [Test]
    [TestCaseSource(nameof(TestCaseGenerator))]
    public void TestPuzzle(Puzzle puzzle, string[] expected)
    {
        // Arrange
        var expectedResult1 = expected.Length switch
        {
            0 => "Part 1 expected results not provided.",
            > 0 => expected[0], // TODO: consider a more flexible approach for part1 multi-line results (see expectedResult2)
            _ => throw new Exception("Expected Results 1: This error should not occur."),
        };
        var expectedResult2 = expected.Length switch
        {
            0 or 1 => "Part 2 expected results not provided.",
            2 => expected[1],
            > 2 => string.Join(Environment.NewLine, expected[1..]).Trim(), // multi-line answers
            _ => throw new Exception("Expected Results 2: This error should not occur."),
        };

        // Act
        puzzle.Setup();
        puzzle.SolvePart1();
        var part1Result = _logger.LastMessage;
        puzzle.SolvePart2();
        var part2Result = _logger.LastMessage?.Trim();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(part1Result, Is.EqualTo(expectedResult1), "Part 1");
            Assert.That(part2Result, Is.EqualTo(expectedResult2), "Part 2");
        });
    }

    private static IEnumerable<TestCaseData> TestCaseGenerator()
    {
        Puzzle puzzle;
        string[] expected;
        for (int i = START_DAY; i <= STOP_DAY; i++)
        {
            try
            {
                if (!TestUtils.TryGetTestPath(i, out var inputPath)) continue;
                if (!TestUtils.TryGetTestPath(i, out var expectedPath, "results.txt")) continue;

                puzzle = Utils.GetClassOfType<Puzzle>($"Day{i}", _logger, inputPath);
                expected = Utils.ReadAllLines(expectedPath);
            }
            catch (Exception)
            {
                continue;
            }
            yield return new TestCaseData(puzzle, expected).SetName($"Day {i:D2}");
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Aoc.Tests;

/// <summary>
///     Manages all test case-related IO operations, including reading, writing, and generating folders and templates.
/// </summary>
public static class TestCaseFileManager
{
    private static readonly ISerializer Serializer = new SerializerBuilder().Build();
    private static readonly IDeserializer Deserializer = new DeserializerBuilder().Build();

    private static readonly string TestsDirectory =
        Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));

    private static string GetYearDirectory(int year) => Path.Combine(TestsDirectory, $"Y{year}");

    private static string GetTestCasesPath(int year, int day) =>
        Path.Combine(TestsDirectory, $"Y{year}", $"D{day:D2}TestCases.yaml");

    public static IEnumerable<(int Day, List<AoCTestCase> Tests)> GetTestCasesYear(int year)
    {
        for (var day = 1; day <= 25; day++)
        {
            var testCases = GetTestCaseData(year, day);
            if (testCases == null) continue;
            yield return (day, testCases);
        }
    }

    public static void EnsureTestCasesExist(int year, int day)
    {
        if (year < 2015 || year > DateTime.Now.Year)
            throw new ArgumentOutOfRangeException(nameof(year), year, "Year must be between 2015 and current year");
        if (day is < 1 or > 25)
            throw new ArgumentOutOfRangeException(nameof(day), day, "Day must be between 1 and 25");

        var directory = GetYearDirectory(year);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            Console.WriteLine($"Created Directory '{directory}'");
        }

        var yamlPath = GetTestCasesPath(year, day);
        if (File.Exists(yamlPath)) return;

        var yamlTemplate = Serializer.Serialize(new AoCTestCases());
        File.WriteAllText(yamlPath, yamlTemplate);
        Console.WriteLine($"Created File '{yamlPath}'");
    }

    private static List<AoCTestCase>? GetTestCaseData(int year, int day)
    {
        try
        {
            var yamlPath = GetTestCasesPath(year, day);
            var yamlText = File.ReadAllText(yamlPath);
            var aocTestCases = Deserializer.Deserialize<AoCTestCases>(yamlText);
            return aocTestCases.TestCases;
        }
        catch (Exception)
        {
            return null;
        }
    }
}

public class AoCTestCases
{
    public List<AoCTestCase> TestCases { get; set; } = [new()];
}

public class AoCTestCase
{
    [YamlMember(ScalarStyle = ScalarStyle.Literal)]
    public string Input { get; set; } =
        """
        Multi-line Input Data Goes Here.
        Indentation level / whitespace is important in Yaml.

        Update expected results as needed - leave expected
        answer blank if the input doesn't apply for that part.

        For more test cases for this day, duplicate everything
        under "TestCases:" (i.e. excluding that first line).
        """;

    public string? Part1Expected { get; set; } = "Part 1";
    public string? Part2Expected { get; set; } = "Part 2";
}
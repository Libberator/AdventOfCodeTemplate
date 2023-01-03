using System;
using System.IO;

namespace AoC.Tests;

public static class TestUtils
{
    public static bool TryGetTestPath(int number, out string fullPath, string file = "input.txt")
    {
        var folder = $"Day{number:D2}Test";
        fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder, file);
        return File.Exists(fullPath);
    }

    public static string FullPathTests(int number, string file = "input.txt")
    {
        var folder = $"Day{number:D2}Test";
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder, file);
    }
}

public class TestLogger : ILogger
{
    public string? LastMessage { get; private set; }

    public void Log(string? msg) => LastMessage = msg;
}
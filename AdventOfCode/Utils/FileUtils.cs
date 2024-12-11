using System;
using System.Collections.Generic;
using System.IO;

namespace AoC;

public static partial class Utils
{
    public static string FullPath(int number, string file = "input.txt")
    {
        var folder = $"Day{number:D2}";
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder, file);
    }

    public static string[] ReadAllLines(string path) => File.Exists(path) ? File.ReadAllLines(path) : [];

    public static IEnumerable<string> ReadFrom(string path, bool ignoreWhiteSpace = false)
    {
        if (!File.Exists(path)) yield break;

        using var reader = File.OpenText(path);
        while (reader.ReadLine() is { } line)
        {
            if (ignoreWhiteSpace && string.IsNullOrWhiteSpace(line)) continue;
            yield return line;
        }
    }
}
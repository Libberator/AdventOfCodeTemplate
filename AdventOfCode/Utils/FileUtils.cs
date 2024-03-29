﻿using System;
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

    public static string[] ReadAllLines(string path) => File.Exists(path) ? File.ReadAllLines(path) : Array.Empty<string>();

    public static IEnumerable<string> ReadFrom(string path, bool ignoreWhiteSpace = false)
    {
        if (!File.Exists(path)) yield break;

        string? line;
        using var reader = File.OpenText(path);
        while ((line = reader.ReadLine()) != null)
        {
            if (ignoreWhiteSpace && string.IsNullOrWhiteSpace(line)) continue;
            yield return line;
        }
    }
}
using System;
using System.IO;

namespace AoC;

internal static class PathHelper
{
    // <path>/AoC/bin/Debug/net9.0 --> <path>/Solutions
    private static readonly string SolutionsProjectDir = Path.GetFullPath(Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..", "Solutions"));

    public static string GetDirectoryForDay(int year, int day) =>
        Path.Combine(SolutionsProjectDir, $"Y{year}", $"D{day:D2}");

    public static string GetFullFilePath(int year, int day, string fileName) =>
        Path.Combine(SolutionsProjectDir, $"Y{year}", $"D{day:D2}", fileName);

    /// <summary>
    ///     Transforms the provided <paramref name="filePath" /> into the FullPath for more informative logging.
    /// </summary>
    /// <exception cref="ArgumentException">Throws if the provided <paramref name="filePath" /> could not be found</exception>
    public static string ValidateInputPath(int year, int day, string? filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return GetFullFilePath(year, day, "input.txt");

        if (File.Exists(filePath)) return Path.GetFullPath(filePath);

        var directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
            throw new ArgumentException($"Could not validate path '{filePath}'. Ensure the directory or file exists.");

        var fileName = Path.GetFileName(filePath);
        if (string.IsNullOrEmpty(fileName)) fileName = "input.txt";
        return Path.GetFullPath(Path.Combine(directory, fileName));
    }
}
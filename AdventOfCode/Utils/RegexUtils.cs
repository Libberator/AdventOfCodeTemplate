using System.Text.RegularExpressions;

namespace AoC;

public static partial class Utils
{
    [GeneratedRegex(@"(-?\d+)")]
    public static partial Regex NumberPattern();
}
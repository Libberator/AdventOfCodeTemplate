using System;
using System.Text;
using System.Threading.Tasks;

namespace AoC;

internal static class Logger
{
    public static void Log(string message, ConsoleColor color = ConsoleColor.Gray)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    /// <summary>
    ///     This will wait until the <paramref name="predicate" /> has been met (returns true).
    /// </summary>
    public static async Task WaitUntilAsync(Func<bool> predicate, Func<string> waitMsg, string completeMsg,
        int updateMs = 500)
    {
        const string ellipsis = ".....";
        int dynamicPadding = 0, i = 0;
        Console.ForegroundColor = ConsoleColor.White;
        while (!predicate())
        {
            var msg = waitMsg();
            dynamicPadding = Math.Max(dynamicPadding, msg.Length + 6);
            Console.Write($"\r{msg}{ellipsis[..(i++ % (ellipsis.Length + 1))]}".PadRight(dynamicPadding));
            await Task.Delay(updateMs);
        }

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine($"\r{completeMsg.PadRight(dynamicPadding)}");
        Console.ResetColor();
    }

    public static string ToReadableString(this TimeSpan span)
    {
        var formatted = new StringBuilder()
            .Append(span.Duration().Days > 0
                ? $"{span.Days:0} day{(span.Days == 1 ? string.Empty : "s")}, "
                : string.Empty)
            .Append(span.Duration().Hours > 0
                ? $"{span.Hours:0} hour{(span.Hours == 1 ? string.Empty : "s")}, "
                : string.Empty)
            .Append(span.Duration().Minutes > 0
                ? $"{span.Minutes:0} minute{(span.Minutes == 1 ? string.Empty : "s")}, "
                : string.Empty)
            .Append(span.Duration().Seconds > 0
                ? $"{span.Seconds:0} second{(span.Seconds == 1 ? string.Empty : "s")}"
                : string.Empty)
            .ToString().TrimEnd(',', ' ');

        return string.IsNullOrEmpty(formatted) ? "0 seconds" : formatted;
    }
}
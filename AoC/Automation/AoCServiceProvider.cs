using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AoC.Utilities.Extensions;

namespace AoC.Automation;

/// <summary>A utility which manages fetching puzzle input files and submitting answers</summary>
public class AoCServiceProvider(string? userSession)
{
    public async Task FetchAll(int[] years, int[] days, bool force, bool wait)
    {
        foreach (var year in years)
            foreach (var day in days)
                await Fetch(year, day, null, force, wait);
    }

    /// <summary>
    ///     Ensures input data exists. If not found locally, this will send a GET request to the server to retrieve it.
    /// </summary>
    /// <param name="year">Year</param>
    /// <param name="day">Day</param>
    /// <param name="saveDir">
    ///     Directory to save any fetched input. Optional. If a file exists and <see cref="force" /> is false, returns true
    ///     early
    /// </param>
    /// <param name="force">Flag to force fetching from the AoC server, overwriting local file if it exists</param>
    /// <param name="wait">Flag to allow waiting idly until the requested day's puzzle is released</param>
    /// <returns>
    ///     True if the file already exists OR we successfully fetched. False if [no file exists OR <see cref="force" />
    ///     is true] AND [we failed fetching OR couldn't attempt to fetch due to missing <see cref="userSession" /> cookies].
    /// </returns>
    public async Task<bool> Fetch(int year, int day, string? saveDir = null, bool force = false, bool wait = false)
    {
        ThrowHelper.ThrowIfOutOfRange(year, 2015, DateTime.Now.Year);
        ThrowHelper.ThrowIfOutOfRange(day, 1, 25);
        var savePath = PathHelper.ValidateInputPath(year, day, saveDir);

        if (!force && File.Exists(savePath))
        {
            Logger.Log($"Found existing input at '{savePath}'", ConsoleColor.DarkGreen);
            return true;
        }

        var requestedDay = Program.MidnightEastern(year, day);
        if (DateTimeOffset.Now < requestedDay)
        {
            var waitTime = requestedDay.Subtract(DateTimeOffset.Now);
            if (!wait)
            {
                Logger.Log(
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Input for {year}-12-{day:D2} not available for {waitTime.ToReadableString()}",
                    ConsoleColor.Red);
                Logger.Log("Tip: Use `--wait`/`-w` to wait around for it!", ConsoleColor.Yellow);
                return false;
            }

            await Logger.WaitUntilAsync(() => DateTimeOffset.Now >= requestedDay, () =>
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Waiting for {(requestedDay - DateTimeOffset.Now).ToReadableString()}",
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Done waiting!", 1000);
        }

        try
        {
            using var client = new AoCHttpClient(userSession);
            var response = await client.GetRequest(year, day);

            Logger.Log($"GET response received [{response.StatusCode}]", ConsoleColor.DarkGreen);
            var inputContent = await response.Content.ReadAsStringAsync();

            Directory.CreateDirectory(Path.GetDirectoryName(savePath)!); // ensure folders exist
            await File.WriteAllTextAsync(savePath, inputContent);

            Logger.Log($"Input written to file '{savePath}'", ConsoleColor.DarkGreen);
            return true;
        }
        catch (Exception e)
        {
            Logger.Log($"Fetch Exception: {e.Message}", ConsoleColor.Red);
        }

        return false;
    }

    /// <summary>
    ///     Submits an answer to the server. Will wait to resubmit if you've answered too recently.
    /// </summary>
    /// <param name="year">Year</param>
    /// <param name="day">Day</param>
    /// <param name="part">The part of the puzzle which we're submitting for (1 or 2)</param>
    /// <param name="answer">The answer to submit for the day's part</param>
    /// <param name="wait">Flag to allow waiting idly until we can resubmit an answer</param>
    public async Task Submit(int year, int day, int part, string? answer, bool wait)
    {
        ThrowHelper.ThrowIfNullOrEmpty(answer);
        ThrowHelper.ThrowIfOutOfRange(year, 2015, DateTime.Now.Year);
        ThrowHelper.ThrowIfOutOfRange(day, 1, 25);
        ThrowHelper.ThrowIfOutOfRange(part, 1, 2);

        Resubmit:
        var submitTime = DateTimeOffset.Now;
        try
        {
            using var client = new AoCHttpClient(userSession);
            var response = await client.PostRequest(year, day, part, answer);
            Logger.Log($"POST response received [{response.StatusCode}]", ConsoleColor.DarkGreen);

            var (result, article) = await ParseResponse(response.Content);
            var color = result switch
            {
                SubmissionResult.Correct => ConsoleColor.DarkGreen,
                SubmissionResult.Incorrect or SubmissionResult.TooSoon => ConsoleColor.Red,
                _ => ConsoleColor.White
            };
            Logger.Log($"[{DateTimeOffset.Now:HH:mm:ss}] {article}", color);

            if (result != SubmissionResult.TooSoon) return;

            var waitTime = GetWaitTime(article);
            if (!wait)
            {
                Logger.Log(
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Must wait {waitTime.ToReadableString()} before resubmitting",
                    ConsoleColor.Red);
                Logger.Log("Tip: Use `--wait`/`-w` to wait around for it!", ConsoleColor.Yellow);
                return;
            }

            var nextSubmissionTime = submitTime + waitTime;
            await Logger.WaitUntilAsync(() => DateTimeOffset.Now >= nextSubmissionTime, () =>
                    $"[{DateTime.Now:HH:mm:ss}] Waiting for {(nextSubmissionTime - DateTimeOffset.Now).ToReadableString()} before resubmitting (Ctrl+C to cancel)",
                $"[{DateTime.Now:HH:mm:ss}] Done waiting! Resubmitting!", 100);

            goto Resubmit;
        }
        catch (Exception e)
        {
            Logger.Log($"Submit Exception: {e.Message}", ConsoleColor.Red);
        }
    }

    private static async Task<(SubmissionResult Result, string Article)> ParseResponse(HttpContent content)
    {
        var response = await content.ReadAsStringAsync();

        var context = BrowsingContext.New(Configuration.Default);
        var document = await context.OpenAsync(req => req.Content(response));
        var article = document.QuerySelector("body > main > article")!.TextContent;

        article = Regex.Replace(article, @"\[Continue to Part Two.*", "", RegexOptions.Singleline);
        article = Regex.Replace(article, "You have completed Day.*", "", RegexOptions.Singleline);
        article = Regex.Replace(article, @"\(You guessed.*", "", RegexOptions.Singleline);
        article = Regex.Replace(article, "  ", "\n", RegexOptions.Singleline);

        var result = article switch
        {
            _ when article.StartsWith("That's the right answer") || article.Contains("You've finished every puzzle") =>
                SubmissionResult.Correct,
            _ when article.StartsWith("That's not the right answer") => SubmissionResult.Incorrect,
            _ when article.StartsWith("You gave an answer too recently") => SubmissionResult.TooSoon,
            _ => SubmissionResult.Unknown
        };

        return (result, article);
    }

    private static TimeSpan GetWaitTime(string article)
    {
        var timeRemaining = Regex.Match(article, "You have (.+) left to wait").Value;
        var hours = Regex.Match(timeRemaining, @"(\d+)h").ParseOrDefault(0);
        var minutes = Regex.Match(timeRemaining, @"(\d+)m").ParseOrDefault(0);
        var seconds = Regex.Match(timeRemaining, @"(\d+)s").ParseOrDefault(0);
        return new TimeSpan(hours, minutes, seconds);
    }

    private enum SubmissionResult
    {
        Correct,
        Incorrect,
        TooSoon,
        Unknown
    }
}
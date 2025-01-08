using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AoC.Automation;

/// <summary>
///     A utility class for sending HTTP requests (GET and POST) to Advent of Code
/// </summary>
public class AoCHttpClient : IDisposable
{
    private const string Domain = "https://adventofcode.com";
    private const string UserSessionName = "session";
    private const string HeaderName = "user-agent";
    private const string HeaderValue = $".NET/9.0 (github.com/libberator/AdventOfCode via {nameof(AoCHttpClient)}.cs)";

    private static DateTimeOffset _nextAllowedRequestTime = DateTimeOffset.UnixEpoch;
    private static readonly SemaphoreSlim Semaphore = new(1, 1);
    private static readonly TimeSpan RateLimit = TimeSpan.FromSeconds(3);

    private readonly HttpClient _httpClient;

    public AoCHttpClient(string? userSessionCookie)
    {
        if (string.IsNullOrEmpty(userSessionCookie))
            throw new ArgumentNullException(nameof(userSessionCookie),
                "Missing a UserSession cookie value\n" +
                "Fix: Go to https://adventofcode.com, log in, hit F12, find your Cookies (e.g. Application Tab>Storage>Cookies), copy 'session' Value to appsettings.json\n" +
                "Bonus: If you use git, run `git update-index --assume-unchanged appsettings.json` so your private cookies don't end up public ;^)");

        var handler = new HttpClientHandler();
        handler.CookieContainer = new CookieContainer();
        handler.CookieContainer.Add(new Uri(Domain), new Cookie(UserSessionName, userSessionCookie));

        _httpClient = new HttpClient(handler);
        _httpClient.BaseAddress = new Uri(Domain);
        _httpClient.DefaultRequestHeaders.Add(HeaderName, HeaderValue);
    }

    public void Dispose() => _httpClient.Dispose();

    /// <summary>
    ///     Send an HTTP GET request to Advent of Code [<see cref="Domain" />]
    /// </summary>
    /// <param name="year">Year</param>
    /// <param name="day">Day</param>
    /// <returns>The request response</returns>
    public async Task<HttpResponseMessage> GetRequest(int year, int day)
    {
        await Semaphore.WaitAsync();

        if (DateTimeOffset.Now < _nextAllowedRequestTime)
            await Logger.WaitUntilAsync(() => DateTimeOffset.Now >= _nextAllowedRequestTime,
                () => $"[Self-imposed rate-limit] Waiting {_nextAllowedRequestTime - DateTimeOffset.Now:s\\.F}s",
                "[Self-imposed rate-limit] Done waiting!", 100);

        // https://adventofcode.com/{year}/day/{day}/input
        var route = $"{year}/day/{day}/input";

        Logger.Log($"[{DateTimeOffset.Now:HH:mm:ss}] Requesting input [GET {Domain}/{route}]");
        try
        {
            var response = await _httpClient.GetAsync(route);
            response.EnsureSuccessStatusCode();
            return response;
        }
        finally
        {
            _nextAllowedRequestTime = DateTimeOffset.Now.Add(RateLimit);
            Semaphore.Release();
        }
    }

    /// <summary>
    ///     Send an HTTP POST request to Advent of Code [<see cref="Domain" />]
    /// </summary>
    /// <param name="year">Year</param>
    /// <param name="day">Day</param>
    /// <param name="part">The puzzle part we're submitting for (1 or 2)</param>
    /// <param name="answer">The user-provided answer for their puzzle</param>
    /// <returns>The request response</returns>
    public async Task<HttpResponseMessage> PostRequest(int year, int day, int part, string answer)
    {
        await Semaphore.WaitAsync();

        // https://adventofcode.com/{year}/day/{day}/answer
        var route = $"{year}/day/{day}/answer";

        // level={part}&answer={answer}
        var content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("level", part.ToString()),
            new KeyValuePair<string, string>("answer", answer)
        ]);

        Logger.Log(
            $"[{DateTimeOffset.Now:HH:mm:ss}] Submitting answer for Day {day} Part {part} [POST {Domain}/{route}]");
        try
        {
            var response = await _httpClient.PostAsync(route, content);
            response.EnsureSuccessStatusCode();
            return response;
        }
        finally
        {
            Semaphore.Release();
        }
    }
}
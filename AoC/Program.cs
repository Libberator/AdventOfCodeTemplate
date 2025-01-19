using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using AoC.Automation;

namespace AoC;

public static class Program
{
    // New AoC puzzle release at midnight east coast time
    private static readonly TimeZoneInfo Est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
    private static readonly AoCServiceProvider Provider;
    private static readonly SolverRunner Runner;

    static Program()
    {
        var settings = JsonNode.Parse(File.ReadAllText("appsettings.json"));
        var userSession = settings?["UserSession"]?.ToString();
        Provider = new AoCServiceProvider(userSession);
        Runner = new SolverRunner(Provider);
    }

    private static DateTimeOffset NowEastern => TimeZoneInfo.ConvertTime(DateTimeOffset.Now, Est);

    private static int Day => NowEastern.Month switch
    {
        11 => 1,
        12 => Math.Clamp(NowEastern.Day, 1, 25),
        _ => 25
    };

    private static int Year => NowEastern.Month >= 11 ? NowEastern.Year : NowEastern.Year - 1;
    public static DateTimeOffset MidnightEastern(int year, int day) => new(year, 12, day, 0, 0, 0, Est.BaseUtcOffset);

    private static async Task Main(string[] args)
    {
        #region Initializing

        var yearsOptionLatest = new Option<string>(["-y", "--year"], () => Year.ToString(),
            "Year(s) selection (e.g. 2015-2024). Defaults to last year until December (EST)");
        var daysOptionAll = new Option<string>(["-d", "--day"], () => "1-25",
            "Day(s) selection (e.g. 17 or 6,9,20-25)");
        var forceOption = new Option<bool>(["-f", "--force"], () => false,
            "Forcefully overwrite any existing file");
        var quitOption = new Option<bool>(["-q", "--quit"], () => false, "Quit after running command");

        // init [-y <string>] [-d <string>] [-f]
        var initCommand = new Command("init", "Generates Solution.cs files (and folders) for you to solve AoC puzzles")
        {
            yearsOptionLatest, daysOptionAll, forceOption
        };
        initCommand.SetHandler(InitHandler, yearsOptionLatest, daysOptionAll, forceOption, quitOption);

        #endregion

        #region Fetching

        var daysOptionLatest = new Option<string>(["-d", "--day"], () => Day.ToString(),
            "Day(s) selection (e.g. 17 or 6,9,20-25)");
        var saveDirOption = new Option<string?>(["-o", "--output"], () => null,
            "Manually specify the directory the fetched input.txt file will be saved to");
        var waitOption = new Option<bool>(["-w", "--wait"], () => false,
            "Allow waiting idly until the AoC server is ready to handle your request");

        // fetch [-y <string>] [-d <string>] [-o <string?>] [-f] [-w]
        var fetchCommand = new Command("fetch", "Fetch input files from the server")
        {
            yearsOptionLatest, daysOptionLatest, saveDirOption, forceOption, waitOption
        };
        fetchCommand.SetHandler(FetchHandler, yearsOptionLatest, daysOptionLatest, saveDirOption, forceOption,
            waitOption, quitOption);

        #endregion

        #region Solving

        var inputFileOption = new Option<string?>(["-i", "--input"],
            "Specify the full file which we should use to solve with (only used when a singular day is selected)");
        var noFetchOption = new Option<bool>("--no-fetch", () => false,
            "Flag to not automatically attempt to fetch input from the server if it's missing");
        var noTimerOption = new Option<bool>("--no-timer", () => false,
            "Flag to not display the runtime of each part");
        var submitPartOption = new Option<int>("--submit",
            "Which part you want to submit [1 or 2] (only used when a singular day is selected)");

        // solve [-y <string>] [-d <string>] [--i <string?>] [--no-fetch] [--no-timer] [--submit <int>]
        var solveCommand = new Command("solve",
            "Run specified solution(s). Will automatically generate a solution and fetch input (unless --no-fetch) if missing. " +
            "Runs the latest solver by default")
        {
            yearsOptionLatest, daysOptionLatest, inputFileOption, noFetchOption, noTimerOption, submitPartOption,
            waitOption
        };
        solveCommand.SetHandler(SolveHandler, yearsOptionLatest, daysOptionLatest, inputFileOption,
            noFetchOption, noTimerOption, submitPartOption, waitOption, quitOption);

        #endregion

        #region Submitting

        var partArgument = new Argument<int>("part", () => 0, "Which part you want to submit: 1 or 2?");
        var answerArgument = new Argument<string?>("answer", () => null, "The answer you want to be submitted");
        var yearOption = new Option<int>(["-y", "--year"], () => Year, "Which year are you submitting for?");
        var dayOption = new Option<int>(["-d", "--day"], () => Day, "Which day are you submitting for?");

        // submit <int> <string?> [-y <int>] [-d <int>]
        var submitCommand = new Command("submit", "Submit an answer directly without running a solver")
        {
            partArgument, answerArgument, yearOption, dayOption, waitOption
        };
        submitCommand.SetHandler(SubmitHandler, yearOption, dayOption, partArgument, answerArgument, waitOption,
            quitOption);

        #endregion

        #region Root Command

        var rootCommand = new RootCommand("Tool built to automate and streamline your Advent of Code puzzles\n" +
                                          "Type '[command] --help' for more info on the command")
        {
            initCommand, fetchCommand, solveCommand, submitCommand
        };
        rootCommand.AddGlobalOption(quitOption);
        rootCommand.SetHandler(async quit => await DefaultHandler(rootCommand, quit), quitOption);

        #endregion

        await rootCommand.InvokeAsync(args);

        while (true)
        {
            Logger.Log("Please enter a command. '-h' for help options. 'exit'/'close'/'quit' to quit.",
                ConsoleColor.Cyan);
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) continue;
            if (input is "quit" or "exit" or "close") return;
            await rootCommand.InvokeAsync(input);
        }
    }

    private static int[] ParseNumbers(string numbers) =>
        numbers.Split(',')
            .SelectMany(part =>
            {
                if (!part.Contains('-'))
                    return int.TryParse(part, out var value)
                        ? [value]
                        : throw new FormatException($"Invalid number format. Cannot parse '{part}'.");

                var range = part.Split('-');
                return range.Length == 2 && int.TryParse(range[0], out var start) &&
                       int.TryParse(range[1], out var end)
                    ? Enumerable.Range(start, end - start + 1)
                    : throw new FormatException($"Invalid number format. Cannot parse '{part}'.");
            })
            .Distinct().Order().ToArray();

    #region Command Handlers

    private static void InitHandler(string yearsInput, string daysInput, bool force, bool quit)
    {
        var years = ParseNumbers(yearsInput);
        var days = ParseNumbers(daysInput);
        SolverTemplate.Generate(years, days, force);
        if (quit) Environment.Exit(0);
    }

    private static async Task FetchHandler(string yearsInput, string daysInput, string? inputDir, bool force, bool wait,
        bool quit)
    {
        var years = ParseNumbers(yearsInput);
        var days = ParseNumbers(daysInput);
        await (years.Length == 1 && days.Length == 1
            ? Provider.Fetch(years[0], days[0], inputDir, force, wait)
            : Provider.FetchAll(years, days, force, wait));
        if (quit) Environment.Exit(0);
    }

    private static async Task SolveHandler(string yearsInput, string daysInput, string? inputPath, bool noFetch,
        bool noTimer, int submitPart, bool wait, bool quit)
    {
        var years = ParseNumbers(yearsInput);
        var days = ParseNumbers(daysInput);
        await (years.Length == 1 && days.Length == 1
            ? Runner.Run(years[0], days[0], inputPath, noFetch, noTimer, submitPart, wait)
            : Runner.RunAll(years, days, noFetch, noTimer));
        if (quit) Environment.Exit(0);
    }

    private static async Task SubmitHandler(int year, int day, int part, string? answer, bool wait, bool quit)
    {
        await Provider.Submit(year, day, part, answer, wait);
        if (quit) Environment.Exit(0);
    }

    // Note: This only gets called when nothing or an empty string is sent into Main(string[] args)
    // ...which is the default state when you press the Play button in your IDE
    private static async Task DefaultHandler(RootCommand root, bool quit)
    {
        if (quit) Environment.Exit(0);
        var defaultArgs = "--help";
#if DEBUG
        try
        {
            var devSettings = JsonNode.Parse(await File.ReadAllTextAsync("appsettings.Development.json"));
            defaultArgs = devSettings?["DefaultArgs"]?.ToString();
        }
        catch (Exception e)
        {
            Logger.Log(
                $"An error occurred while trying to read or parse DefaultArgs from 'appsettings.Development.json'. " +
                $"Did you add it to your .csproj and <CopyToOutputDirectory>?\n{e.Message}", ConsoleColor.DarkRed);
            return;
        }
#endif
        if (string.IsNullOrEmpty(defaultArgs)) return; // prevent endless recursion
        await root.InvokeAsync(defaultArgs);
    }

    #endregion
}
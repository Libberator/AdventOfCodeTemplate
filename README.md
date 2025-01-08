# Advent Of Code

Thanks for using [this C# Template](https://github.com/Libberator/AdventOfCodeTemplate) for your [Advent of Code](https://adventofcode.com) solutions.
<br>:star: If you've gotten any value from it, leave a star! :star:
<br>:thought_balloon: If you have any feature requests, suggestions or bug reports, open an Issue or Pull Request - your feedback is invaluable! :thought_balloon:

## Features

- Automates solver file creation, input fetching, solving, and answer submission
- Automates test file creation and testing your solutions
- Useful data structures (CircularBuffer, Deque, Vec2D, Vec2DLong, Vec3D, etc.)
- Pathfinding (A*, Dijkstra, BFS, DFS, etc.)
- Graph Theory Algorithms (MaxClique, MaxFlow, MinSpanTree, etc.)
- And tons of helpful extension methods!

## Getting Started

Fork, clone, or click "Use this template". You can figure that part out

In the provided `appsettings.json` file, you want to include the cookie for your Advent of Code user session
<br>This is browser-dependent but here's an example on how to find your cookie: Log into [AdventOfCode](https://adventofcode.com) > hit F12 > Application Tab > Storage > Cookies > copy 'session' Value
<br>:warning: <b>Important</b> :warning:: Don't push that cookie to a public repo! To stop tracking any changes to that file, run:
```
git update-index --assume-unchanged AoC/appsettings.json
``` 

You should be good to go! Press Play in your IDE and start running commands and filling out solution files

You can also run it via command line. In the `AoC/` directory, run `dotnet run`
<br>Note: if you want to add any options to that, you need to add an extra separating `--` so `dotnet` doesn't think
they're for it. Example: `dotnet run -- solve --day 20`

## Commands

If you run the program as-is, it will execute the default command found in `appsettings.Development.json` to save you
from having to type your preferred default command every time. Adjust it to suit your personal preference

There are many Options for each Command (and some commands differ in default values):
- `--year`/`-y` option supports comma-separated and hyphenated ranges of years, between 2015 (start of AoC) and now
(some exceptions apply)
- `--day`/`-d` option supports comma-separated and hyphenated ranges of days, between 1 and 25
- `--quit`/`-q` option will stop the program after executing the command
- `--help`/`-h`/`-?` for more info

### Initializing
Set up folders and `Solution.cs` files.
```
init  // defaults to latest year, *all 25 days*
```
The `--force`/`-f`  optional flag will forcefully overwrite any existing file
```
init -y 2015-2024 -d 1-25 -f // sets up all 25 days for every year, forcing overwrites. Fresh start
```

### Fetching
Get input from the server.
```
fetch  // defaults to latest year, *single latest day* (to not overload server)
```
It won't attempt to fetch if:
- an input file already exists for that day (unless bypassed with `--force`/`-f`)
- if you're missing a user session cookie (see [Getting Started](#getting-started))

Too early for tomorrow's puzzle to be released? We've got you covered with `--wait`/`-w`:
```
fetch -d 25 --wait  // assuming tomorrow's xmas, this will wait until midnight EST before fetching
```

### Solving

Solve a puzzle (or multiple).
```
solve  // defaults to latest year, *single latest day*
```
If you don't want it to automatically attempt to fetch missing input (it will by default), add the `--no-fetch` flag
<br>If you don't want to see the timer results, add the `--no-timer` flag

When you're solving for a single day, you have access to a few extra options:
<br>You can specify a specific input file with `--input`/`-i` `<file_path>`
<br>You can submit your answer for a specific part of the solution with `--submit <part>` (1 or 2)
```
solve --submit 2  // runs the latest day's solver and submits part 2 results
```
If you've submitted an incorrect answer too recently, it won't wait around before resubmitting UNLESS you also add the
`--wait`/`-w` flag

### Submitting

Submit a solution's part directly (without running the Solver).
```
submit 1 MyAnswerForPartOne  // submits for part 1 of the latest day
```
Note: The `--year`/`-y` and `--day`/`-d` options only support single values for this
<br>If your answer has any spaces, you should "wrap it in quotation marks"
<br>If you've submitted an incorrect answer too recently, it won't wait around before resubmitting UNLESS you also add the
`--wait`/`-w` flag

## Test Cases

When you run the Test Runner, it will create test cases for you based off of your `D##TestCase.yaml` files

Don't have any test files yet? Run the generator (made as an Explicit Test) and it will automatically generate test case
template files for you, matching which `Solution.cs` files you have. Recommend doing that after you've run `init`

Why Yaml? It supports multi-line inputs very nicely (JSON does not), and there's very little boilerplate or distracting
syntax (e.g. XML/HTML tags). I could've created my own format and parser, but that would needlessly add a barrier to
entry for other people to use this project

### Custom Test Values
Some puzzles have test cases that require a different parameter for your solver to use (e.g. specific grid size or number
of iterations), and it can't always be inferred from the input data. I've included a special Attribute that will override
any value when the Test Runner is running. It's called `TestValueAttribute` and here's how to use it:
```csharp
public class Solution : ISolver
{
    [TestValue(11, 13)]  // will create a Vec2D from args and use the smaller grid size when testing
    public Vec2D GridSize { get; set; } = new Vec2D(101, 103);

    [TestValue(12)]  // will use 12 when the tests are running
    private readonly int _iterations = 125; // will use 125 by default otherwise

    // ... rest of solution ...
}
```
Some restrictions on using this attribute:
- A field with this Attribute cannot be `const`, nor `static readonly`. It can be `static`, it can be `readonly`, but not both
- An auto-property with this Attribute must have a `set;`, otherwise it can't be overwritten
- You must put something in the Attribute's constructor (i.e. a standalone `[TestValue]` is not allowed)

## Stay Up To Date
If you want to get any updates from the template, you can do it automatically via setting up a
[GitHub Action to Sync with Template](https://github.com/marketplace/actions/actions-template-sync) or you can do it manually with the following git commands:
```
git remote add template https://github.com/Libberator/AdventOfCodeTemplate.git
git fetch template   // or `git fetch --all`
git merge --squash template/main --allow-unrelated-histories
```

## Dependencies

.NET 9.0 / C# 13

**System.CommandLine** for the CLI

**AngleSharp** for parsing server response (success/fail/wait time)

**NUnit** and **YamlDotNet** for test cases

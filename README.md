## Advent Of Code Template

*Written in C# 11 (preview), .NET 7.0*

### Template Features
- Uses Reflection to easily discover classes
- Supporting Data Structures:
    - Vector2Int and Vector3Int
	- Bounds and Bounds3D
	- Node and Grid for graph traveral or pathfinding
	- Circular Array
- Lots of Utility helpers:
	- Pathfinding (A* and Dijkstra)
	- Collection extensions
	- Math extensions (primes, factors, GCD, LCM, and more)
	- File reading helpers
- Perfect for test-driven development:
	- Uses strings so no need to convert between `int`, `long`, or use `dynamic`
	- Supports multi-line answers (\*only for Part 2). Tip: use StringBuilder
- Benchmark project to compare your solution against your friends

Note: For legal purposes, you should add the following line to your `.gitignore` because we don't own the rights to redistribute their data:
> /AdventOfCode/*/input.txt

For more info, see <a href="https://adventofcode.com/about">AdventOfCode's About Page</a>

<br>
:writing_hand: Open to feedback. Feel free to send a PR or open a ticket! :computer: 

:star: Leave a Star if you got any value from this :star:
<br>

Future TODO's:
- Add a script to automate fetching input data
- Expand the different types of pathfinding algorithms and make it more flexible

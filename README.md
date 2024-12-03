## Advent Of Code Template

*Uses .NET 9.0 (w/ latest major C# release)*

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

Note: For legal purposes (because we don't own the rights to redistribute their data), the `.gitignore` includes the following line to ensure your input doesn't get pushed:
> /AdventOfCode/*/input.txt

For more info, see <a href="https://adventofcode.com/about">AdventOfCode's About Page</a>

<br>
:writing_hand: Open to feedback. Feel free to send a PR or open a ticket! :computer: 

:star: Leave a Star if you got any value from this :star:
<br>
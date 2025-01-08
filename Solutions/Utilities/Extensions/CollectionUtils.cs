using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AoC.Utilities.Extensions;

public static partial class Utils
{
    /// <summary>Adds the value to an existing key-value pair or creates a new one if one does not exist.</summary>
    /// <returns>False if a new key-value pair was created (with added value). True if one was already in the dictionary</returns>
    public static bool AddToExistingOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue val)
        where TValue : INumber<TValue>
    {
        if (dict.TryAdd(key, val)) return false;
        dict[key] += val;
        return true;
    }

    /// <summary>Searches an ordered list for the first (i.e. earliest) occurrence of the predicate returning true.</summary>
    /// <param name="list">Source list to search. Assumes it's sorted for false returns first and true results later</param>
    /// <param name="predicate">Condition to check against</param>
    /// <param name="min">Start index, inclusive</param>
    /// <param name="max">Stop index, inclusive (defaults to <paramref name="list" />.Count - 1)</param>
    /// <returns>The index for the first element the predicate returned true for. Returns -1 if predicate never passes</returns>
    public static int BinarySearch<T>(this IList<T> list, Predicate<T> predicate, int min = 0, int max = -1)
    {
        max = max == -1 ? list.Count - 1 : max;
        var i = max / 2;
        while (min + 1 < max)
        {
            if (predicate(list[i])) max = i;
            else min = i;
            i = max - (max - min) / 2;
        }

        return predicate(list[min]) ? min
            : predicate(list[max]) ? max
            : -1;
    }

    /// <summary>
    ///     Searches between two values, inclusive, and returns first value which passes the condition. Throws if none found.
    /// </summary>
    public static int BinarySearch(this int min, int max, Predicate<int> check)
    {
        var index = max - (max - min) / 2;
        while (min + 1 < max)
        {
            if (check(index)) max = index;
            else min = index;
            index = max - (max - min) / 2;
        }

        return check(min) ? min : check(max) ? max : throw new Exception("Not found");
    }

    /// <summary>
    ///     Chunk the source based on a <paramref name="takePredicate" /> and an optional <paramref name="skipPredicate" />.
    /// </summary>
    public static IList<T[]> ChunkBy<T>(this IEnumerable<T> source, Predicate<T> takePredicate,
        Predicate<T>? skipPredicate = null)
    {
        var chunks = new List<T[]>();
        var enumerable = source as IList<T> ?? source.ToArray();
        skipPredicate ??= el => !takePredicate(el);

        for (var i = 0; i < enumerable.Count;)
        {
            var chunk = enumerable
                .Skip(i)
                .TakeWhile(takePredicate.Invoke)
                .ToArray();

            if (chunk.Length > 0) chunks.Add(chunk);

            i += chunk.Length;
            i += enumerable
                .Skip(i)
                .TakeWhile(skipPredicate.Invoke)
                .Count();
        }

        return chunks;
    }

    /// <summary>
    ///     For getting vertical data in 2D arrays. This will throw an exception if you don't have the right amount in the
    ///     jagged array.
    /// </summary>
    public static T[][] GetColumnData<T>(this T[][] values, int startColumn, int numberOfColumns)
    {
        return Enumerable.Range(startColumn, numberOfColumns)
            .Select(i => values.Select(x => x[i]).ToArray())
            .ToArray();
    }

    /// <summary>This will return 1 column of data from a 2D jagged array into a single array.</summary>
    public static T[] GetColumnData<T>(this T[][] values, int column) => values.Select(x => x[column]).ToArray();

    /// <summary>
    ///     Adds a key/value pair to the <see cref="IDictionary{TKey,TValue}" /> if the key does not already exist.
    ///     Return the pre-existing or newly generated value for the key.
    /// </summary>
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
        Func<TKey, TValue> valueFactory) =>
        dict.TryGetValue(key, out var resultingValue) ? resultingValue : dict[key] = valueFactory(key);

    public static string JoinAsString<T>(this IEnumerable<T> source, char delimiter = ',') =>
        string.Join(delimiter, source);

    public static string JoinAsString<T>(this IEnumerable<T> source, string delimiter) =>
        string.Join(delimiter, source);

    /// <summary>Returns the median elements: the middle element after sorting the list.</summary>
    public static T Median<T>(this IList<T> list) => list.Order().ElementAt(list.Count / 2);

    /// <summary>Returns the median elements: the middle element after sorting the list.</summary>
    public static T Median<T>(this T[] array) => array.Order().ElementAt(array.Length / 2);

    /// <summary>Returns the middle-most value, favoring the end for collections of even quantities.</summary>
    public static T Middle<T>(this IEnumerable<T> source)
    {
        var enumerable = source as T[] ?? source.ToArray();
        return enumerable.ElementAt(enumerable.Length / 2);
    }

    /// <summary>Returns the middle-most value, favoring the end for collections of even quantities.</summary>
    public static T Middle<T>(this IList<T> list) => list.ElementAt(list.Count / 2);

    /// <summary>Returns the middle-most value, favoring the end for collections of even quantities.</summary>
    public static T Middle<T>(this T[] array) => array.ElementAt(array.Length / 2);

    /// <summary>Returns the mode of the collection (i.e. the element with the highest occurring frequency).</summary>
    public static T Mode<T>(this IEnumerable<T> source) where T : notnull =>
        source.GroupBy(e => e).MaxBy(g => g.Count())!.Key;

    /// <summary>Swaps two elements in a collection.</summary>
    public static void Swap<T>(this IList<T> list, int index1, int index2)
    {
        if (index1 == index2) return;
        (list[index2], list[index1]) = (list[index1], list[index2]);
    }

    /// <summary>Swaps two elements in a collection.</summary>
    public static void Swap<T>(this T[] array, int index1, int index2)
    {
        if (index1 == index2) return;
        (array[index2], array[index1]) = (array[index1], array[index2]);
    }

    /// <summary>Similar to Swap, but if the two indices aren't next to each other, everything in-between will shift over.</summary>
    public static void SwapShift<T>(this IList<T> list, int from, int to)
    {
        if (from == to) return;
        var temp = list[from];
        list.RemoveAt(from);
        list.Insert(to, temp);
    }
}
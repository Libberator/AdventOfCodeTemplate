using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AoC;

public static partial class Utils
{
    /// <summary>
    /// Adds the value to an existing key-value pair or creates a new one if one does not exist. Returns true if one was already in the dictionary
    /// </summary>
    public static bool AddToExistingOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue val) where TValue : INumber<TValue>
    {
        if (dict.ContainsKey(key))
        {
            dict[key] += val;
            return true;
        }
        dict.Add(key, val);
        return false;
    }

    /// <summary>
    /// Tries to grab a value from a dictionary if it exists, otherwise returns the provided default value.
    /// </summary>
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defVal) =>
        dict.TryGetValue(key, out var value) ? value : defVal;

    // Note: If the collection is already sorted, then Middle is the same as Median
    /// <summary>Returns the middle-most value, favoring the end for collections of even quantities.</summary>
    public static T Middle<T>(this IList<T> list) => list.ElementAt(list.Count / 2);
    /// <summary>Returns the middle-most value, favoring the end for collections of even quantities.</summary>
    public static T Middle<T>(this T[] array) => array[array.Length / 2];

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
        T temp = list[from];
        list.RemoveAt(from);
        list.Insert(to, temp);
    }

    public static T MaxBy<T>(this IEnumerable<T> source, Func<T, IComparable> score) =>
        source.Aggregate((x, y) => score(x).CompareTo(score(y)) > 0 ? x : y);

    public static T MinBy<T>(this IEnumerable<T> source, Func<T, IComparable> score) =>
        source.Aggregate((x, y) => score(x).CompareTo(score(y)) < 0 ? x : y);

    /// <summary>For getting vertical data in 2D arrays. This will throw an exception if you don't have the right amount in the jagged array.</summary>
    /// <exception cref="IndexOutOfRangeException"/>
    public static T[][] GetColumnData<T>(this T[][] values, int startColumn, int numberOfColumns)
    {
        return Enumerable.Range(startColumn, numberOfColumns)
            .Select(i => values.Select(x => x[i]).ToArray())
            .ToArray();
    }

    /// <summary>This will return 1 column of data from a 2D jagged array into a single array.</summary>
    public static T[] GetColumnData<T>(this T[][] values, int column) => values.Select(x => x[column]).ToArray();

    public static string JoinAsString<T>(this IEnumerable<T> source, string delimiter = ", ") =>
        string.Join(delimiter, source);
}
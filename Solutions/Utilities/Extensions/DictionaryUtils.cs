using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AoC.Utilities.Extensions;

public static partial class Utils
{
    /// <summary>Adds the number value to an existing entry's value or creates a new one if one does not exist.</summary>
    /// <returns>True if a key already existed. False if the key was not found</returns>
    public static bool AddToExistingOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        where TKey : notnull where TValue : INumber<TValue>
    {
        ref var val = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out var exists)!;
        val = exists ? val + value : value;
        return exists;
    }

    /// <summary>Adds a key-value pair to the <see cref="Dictionary{TKey,TValue}" /> if the key does not already exist.</summary>
    /// <returns>The value if it already existed, or the provided <paramref name="value" /> if it was not found</returns>
    public static TValue? GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue? value)
        where TKey : notnull
    {
        ref var val = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out var exists);
        return exists ? val : val = value;
    }

    /// <summary>Attempts to update a value of a key-value pair if the key exists.</summary>
    /// <returns>False if the key was not found. True if we updated the value</returns>
    public static bool TryUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        where TKey : notnull
    {
        ref var val = ref CollectionsMarshal.GetValueRefOrNullRef(dict, key);
        if (Unsafe.IsNullRef(ref val)) return false;
        val = value;
        return true;
    }
}
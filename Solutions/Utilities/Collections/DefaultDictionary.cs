using System;
using System.Collections;
using System.Collections.Generic;

namespace AoC.Utilities.Collections;

public sealed class DefaultDictionary<TKey, TValue>(Func<TKey, TValue> defaultSelector)
    : IDictionary<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _dictionary = new();

    public DefaultDictionary(TValue defaultValue) : this(_ => defaultValue)
    {
    }

    public DefaultDictionary(TValue defaultValue, IEnumerable<(TKey Key, TValue Value)> items) : this(defaultValue)
    {
        foreach (var item in items) Add(item.Key, item.Value);
    }

    public DefaultDictionary(TValue defaultValue, IEnumerable<KeyValuePair<TKey, TValue>> items) : this(defaultValue)
    {
        foreach (var item in items) Add(item.Key, item.Value);
    }

    public bool IsReadOnly => false;
    public int Count => _dictionary.Count;
    public ICollection<TKey> Keys => _dictionary.Keys;
    public ICollection<TValue> Values => _dictionary.Values;

    public TValue this[TKey key]
    {
        get => IndexGetInternal(key);
        set => IndexSetInternal(key, value);
    }

    public void Add(TKey key, TValue value) => _dictionary.Add(key, value);

    public void Add(KeyValuePair<TKey, TValue> item) => _dictionary[item.Key] = item.Value;

    public bool Remove(TKey key) => _dictionary.Remove(key);

    public bool Remove(KeyValuePair<TKey, TValue> item) => _dictionary.Remove(item.Key);

    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

    public bool Contains(KeyValuePair<TKey, TValue> item) =>
        _dictionary.TryGetValue(item.Key, out var value) && Equals(value, item.Value);

    public bool TryGetValue(TKey key, out TValue value)
    {
        value = this[key];
        return true;
    }

    public void Clear() => _dictionary.Clear();

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_dictionary).GetEnumerator();

    private void IndexSetInternal(TKey key, TValue value) => _dictionary[key] = value;

    private TValue IndexGetInternal(TKey key)
    {
        if (_dictionary.TryGetValue(key, out var value)) return value;

        _dictionary[key] = defaultSelector.Invoke(key);
        return _dictionary[key];
    }
}
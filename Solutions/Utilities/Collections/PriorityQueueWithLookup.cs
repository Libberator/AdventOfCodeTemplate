using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace AoC.Utilities.Collections;

// Note: this is inefficient and should be combined into one data structure instead of duplicating memory.
/// <summary>
///     A Priority Queue with a HashSet for a Contains lookup. Does not allow for duplicate Keys in the Priority Queue,
///     even if the priorities differ.
/// </summary>
public class PriorityQueueWithLookup<TElement, TPriority> where TPriority : INumber<TPriority>
{
    private readonly HashSet<TElement> _lookup = [];
    private readonly PriorityQueue<TElement, TPriority> _priorityQueue = new();

    public int Count => _priorityQueue.Count;

    public bool Any() => _priorityQueue.Count > 0;

    public void Clear()
    {
        _lookup.Clear();
        _priorityQueue.Clear();
    }

    public bool Contains(TElement element) => _lookup.Contains(element);

    public TElement Dequeue()
    {
        var element = _priorityQueue.Dequeue();
        _lookup.Remove(element);
        return element;
    }

    /// <summary>
    ///     Add an element to the HashSet and Priority Queue with the priority.
    /// </summary>
    /// <returns>True if we successfully add the element. False if it fails (no duplicates allowed)</returns>
    public bool Enqueue(TElement element, TPriority priority)
    {
        if (!_lookup.Add(element))
            return false;
        _priorityQueue.Enqueue(element, priority);
        return true;
    }

    public TElement Peek() => _priorityQueue.Peek();

    public bool Remove(TElement element) =>
        _priorityQueue.Remove(element, out _, out _) && _lookup.Remove(element);

    public bool Remove(TElement element, [MaybeNullWhen(false)] out TElement removedItem,
        [MaybeNullWhen(false)] out TPriority removedPriority,
        IEqualityComparer<TElement>? comparer = null) =>
        _priorityQueue.Remove(element, out removedItem, out removedPriority, comparer) && _lookup.Remove(element);

    public bool TryDequeue([MaybeNullWhen(false)] out TElement element,
        [MaybeNullWhen(false)] out TPriority priority) =>
        _priorityQueue.TryDequeue(out element, out priority) && _lookup.Remove(element);

    public bool TryPeek([MaybeNullWhen(false)] out TElement element, [MaybeNullWhen(false)] out TPriority priority) =>
        _priorityQueue.TryPeek(out element, out priority);
}
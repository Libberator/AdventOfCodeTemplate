using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AoC.Utilities.Extensions;

namespace AoC.Utilities.Collections;

/// <summary>A standard array except elements are accessed in a circular fashion.</summary>
public class CircularBuffer<T> : IEnumerable<T>
{
    //The internal array used to store the elements.
    private readonly T[] _array;

    /// <summary>The total number of elements the array can hold.</summary>
    public readonly int Capacity;

    //The index in the array of the currently selected element.
    private int _index;

    /// <summary>Initialize the array with the given capacity.</summary>
    public CircularBuffer(int size)
    {
        Capacity = size;
        _array = new T[size];
    }

    /// <summary>
    ///     Initialize the array with the given capacity and fill the array with the given elements.
    ///     If more elements are given than the capacity allows, the extra elements will be discarded and not saved in the
    ///     array.
    ///     If less elements are given than the capacity, the remaining elements are initialized with a default value.
    /// </summary>
    public CircularBuffer(int size, IEnumerable<T> source) : this(size)
    {
        var index = 0;
        foreach (var element in source.Take(size))
            _array[index++] = element;
    }

    /// <summary>
    ///     Initialize the array with a given set of elements. The capacity of the array is determined by the number of
    ///     elements given.
    /// </summary>
    public CircularBuffer(IEnumerable<T> source) : this(
        source.TryGetNonEnumeratedCount(out var count) ? count : source.Count(), source)
    {
    }

    /// <summary>Create a copy of another circular array. The new array defaults to the first element.</summary>
    public CircularBuffer(CircularBuffer<T> copy)
    {
        Capacity = copy.Capacity;
        _array = new T[copy.Capacity];
        Array.Copy(copy._array, _array, copy.Capacity);
    }

    /// <summary>The current index of the array. This value will always be in the range [0, Capacity).</summary>
    public int Index
    {
        get => _index;
        set => _index = value.Mod(Capacity);
    }

    /// <summary>
    ///     The current selected element in the array.
    ///     This is based on the Index and can be changed using 'Move(int offset)', 'Next(int count)', or 'Previous(int
    ///     count)'.
    /// </summary>
    public T Current
    {
        get => _array[_index];
        set => _array[_index] = value;
    }

    /// <summary>
    ///     Access an element based on its index. Negative and positive indexes are allowed and will simply wrap around the
    ///     array as needed, creating a "circular" effect.
    ///     For example, '-1' will access the last element in the array.
    /// </summary>
    public T this[int index]
    {
        get => _array[index.Mod(Capacity)];
        set => _array[index.Mod(Capacity)] = value;
    }

    public IEnumerator<T> GetEnumerator() => Forward(false).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Resets the index to the first element.</summary>
    public void Reset() => Index = 0;

    /// <summary>
    ///     Move the current index by the given offset (default 1).
    ///     Negative offsets are allowed and will simply move in the opposite direction as positive offsets.
    /// </summary>
    public T Move(int offset = 1)
    {
        Index += offset;
        return _array[_index];
    }

    /// <summary>
    ///     Move the current index by the given count (default 1) in the positive direction.
    ///     Only moves in the positive direction. Negative counts are not allowed.
    /// </summary>
    public T Next(int count = 1)
    {
        if (count < 0)
            throw new ArgumentException(
                $"Count can't be negative. Consider using \'Move(int count)\' instead. Argument provided: {count}.");
        Index += count;
        return _array[_index];
    }

    /// <summary>
    ///     Move the current index by the given count (default 1) in the negative direction.
    ///     Only moves in the negative direction. Negative counts are not allowed.
    /// </summary>
    public T Previous(int count = 1)
    {
        if (count < 0)
            throw new ArgumentException(
                $"Count can't be negative. Consider using \'Move(int count)\' instead. Argument provided: {count}.");
        Index -= count;
        return _array[_index];
    }

    /// <summary>
    ///     Peeks at an element with the given offset (default 1).
    ///     Negative offsets are allowed, and will simply offset in the opposite direction as a positive offset.
    /// </summary>
    public T Peek(int offset = 1) => _array[(_index + offset).Mod(Capacity)];

    /// <summary>
    ///     Returns elements of the array moving in the positive direction.
    ///     By default, will infinitely return items (as the array is "circular"). To only iterate over each element
    ///     once, set "infinite" to false. The iteration begins at the current item.
    /// </summary>
    public IEnumerable<T> Forward(bool infinite = true)
    {
        var index = _index;
        do
        {
            yield return _array[index];
            index = (index + 1) % Capacity;
        } while (index != _index || infinite);
    }

    /// <summary>
    ///     Returns elements of the array moving in the negative direction.
    ///     By default, will infinitely return items (as the array is "circular"). To only iterate over each element
    ///     once, set "infinite" to false. The iteration begins at the current item.
    /// </summary>
    public IEnumerable<T> Reverse(bool infinite = true)
    {
        var index = _index;
        do
        {
            yield return _array[index];
            index = (index - 1).Mod(Capacity);
        } while (index != _index || infinite);
    }

    /// <summary>
    ///     Fills the array with the given elements starting at the current index and moving in the positive direction.
    ///     If the number of elements is greater than the capacity of the array, some of the given elements will be overriden
    ///     in the array as the index wraps around.
    /// </summary>
    public void Fill(IEnumerable<T> elements)
    {
        var index = _index;
        foreach (var element in elements)
        {
            _array[index] = element;
            index = (index + 1) % Capacity;
        }
    }

    /// <summary>
    ///     Fills the array with the given elements starting at the current index and moving in the positive direction.
    ///     If count is less than the given number of elements, only the given elements will be filled.
    ///     If count is greater than the capacity of the array, some of the given elements will be overriden in the array as
    ///     the index wraps around.
    /// </summary>
    public void Fill(IEnumerable<T> elements, int count)
    {
        var index = _index;
        foreach (var element in elements.Take(count))
        {
            _array[index] = element;
            index = (index + 1) % Capacity;
        }
    }
}
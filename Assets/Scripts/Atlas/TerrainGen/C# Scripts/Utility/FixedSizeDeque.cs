using System;
using System.Collections.Generic;

public class FixedSizeDeque<T>
{
    private LinkedList<T> _deque;
    private HashSet<T> _set;
    private int _size;

    public FixedSizeDeque(int size)
    {
        if (size <= 0)
            throw new ArgumentException("Size must be greater than 0.", nameof(size));

        _deque = new LinkedList<T>();
        _set = new HashSet<T>();
        _size = size;
    }

    // Add to the front
    public void AddFront(T item)
    {
        _deque.AddFirst(item);
        _set.Add(item);
        EnsureSize();
    }

    // Remove and return the front item
    public T PopFront()
    {
        if (_deque.Count == 0)
            throw new InvalidOperationException("Deque is empty!");

        T value = _deque.First.Value;
        _deque.RemoveFirst();
        _set.Remove(value);
        return value;
    }

    // Peek at the front item without removing
    public T PeekFront()
    {
        if (_deque.Count == 0)
            throw new InvalidOperationException("Deque is empty!");

        return _deque.First.Value;
    }

    // Peek at the back item without removing
    public T PeekBack()
    {
        if (_deque.Count == 0)
            throw new InvalidOperationException("Deque is empty!");

        return _deque.Last.Value;
    }

    // Check if the deque contains the item
    public bool Contains(T item)
    {
        return _set.Contains(item); // O(1) lookup
    }

    // Get the number of items in the deque
    public int Count => _deque.Count;

    // Check if the deque is empty
    public bool IsEmpty => _deque.Count == 0;

    // Clear the deque
    public void Clear()
    {
        _deque.Clear();
    }

    // Ensures the deque does not exceed the specified size
    private void EnsureSize()
    {
        while (_deque.Count > _size)
        {
            _set.Remove(_deque.Last.Value);
            _deque.RemoveLast();
        }
    }
}

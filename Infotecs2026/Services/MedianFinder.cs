using System.Numerics;

namespace Infotecs2026.Services;

public class MedianFinder<T> where T : INumber<T>
{
    private PriorityQueue<T, T> _maxHeap;
    private PriorityQueue<T, T> _minHeap;

    public MedianFinder()
    {
        _maxHeap = new PriorityQueue<T, T>(Comparer<T>.Create((a, b) => b.CompareTo(a)));
        _minHeap = new PriorityQueue<T, T>();
    }

    public void Add(T num)
    {
        if (_maxHeap.Count == 0 || num <= _maxHeap.Peek())
            _maxHeap.Enqueue(num, num);
        else
            _minHeap.Enqueue(num, num);

        Rebalance();
    }

    private void Rebalance()
    {
        // |max-heap| - |min-heap| > 1
        if (_maxHeap.Count > _minHeap.Count + 1)
        {
            T movedValue = _maxHeap.Dequeue();
            _minHeap.Enqueue(movedValue, movedValue);
        }
        else if (_minHeap.Count < _maxHeap.Count - 1)
        {
            T movedValue = _minHeap.Dequeue();
            _maxHeap.Enqueue(movedValue, movedValue);
        }
    }

    public double FindMedian() => _maxHeap.Count > _minHeap.Count ?
        Convert.ToDouble(_maxHeap.Peek()) : 
        Convert.ToDouble(_maxHeap.Peek() + _minHeap.Peek()) / 2.0;
}

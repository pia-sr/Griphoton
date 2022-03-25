using System;

public interface IHeapItem<T> : IComparable<T>
{
    int IndexHeap
    {
        get;
        set;
    }
}
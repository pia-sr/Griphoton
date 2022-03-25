using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// code based on 
public class Heap<T> where T : IHeapItem<T>
{

    private T[] heap;
    private int heapItemIndex;

    public Heap(int heapSize)
    {
        heap = new T[heapSize];
    }

    private int childLeft(int index)
    {
        return index * 2 + 1;
    }

    private int childRight(int index)
    {
        return index * 2 + 2;
    }

    public void insert(T item)
    {
        item.IndexHeap = heapItemIndex;
        heap[heapItemIndex] = item;
        heapifyUp(item);
        heapItemIndex++;
    }

    public T deleteFirst()
    {
        T first = heap[0];
        heapItemIndex--;
        heap[0] = heap[heapItemIndex];
        heap[0].IndexHeap = 0;
        heapifyDown(heap[0]);
        return first;
    }

    public void updateItem(T item)
    {
        heapifyUp(item);
    }

    public int count
    {
        get
        {
            return heapItemIndex;
        }
    }

    public bool contains(T item)
    {
        return Equals(heap[item.IndexHeap], item);
    }

    void heapifyDown(T item)
    {
        while (true)
        {
            int swapIndex = 0;

            if (childLeft(item.IndexHeap) < heapItemIndex)
            {
                swapIndex = childLeft(item.IndexHeap);

                if (childRight(item.IndexHeap) < heapItemIndex)
                {
                    if (heap[childLeft(item.IndexHeap)].CompareTo(heap[childRight(item.IndexHeap)]) < 0)
                    {
                        swapIndex = childRight(item.IndexHeap);
                    }
                }

                if (item.CompareTo(heap[swapIndex]) < 0)
                {
                    swap(item, heap[swapIndex]);
                }
                else
                {
                    return;
                }

            }
            else
            {
                return;
            }

        }
    }

    void heapifyUp(T item)
    {
        int parentIndex = (item.IndexHeap - 1) / 2;

        while (true)
        {
            T parentItem = heap[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                swap(item, parentItem);
            }
            else
            {
                break;
            }

            parentIndex = (item.IndexHeap - 1) / 2;
        }
    }

    void swap(T itemA, T itemB)
    {
        heap[itemA.IndexHeap] = itemB;
        heap[itemB.IndexHeap] = itemA;
        int itemAIndex = itemA.IndexHeap;
        itemA.IndexHeap = itemB.IndexHeap;
        itemB.IndexHeap = itemAIndex;
    }

}

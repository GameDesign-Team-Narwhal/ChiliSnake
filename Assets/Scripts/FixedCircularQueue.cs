using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//circular queue with a fixed size
//supports random access
//the first element enqueued has index zero, and enqueueing another element will increment existing elements' indexes by 1
//Enqueue()ing more than size elements will overwrite the highest-index element
public class FixedCircularQueue<T>
{
    T[] elements;

    //current "zero" index
    uint currentStartIndex;
    uint _size = 0;

    //actual size of the queue (how many elements have been inserted)
    public uint size
    {
        get
        {
            return _size;
        }
    }

    //last valid index in the queue
    // aka size - 1
    //returns -1 if size is 0
    public int lastIndex
    {
        get
        {
            return ((int)_size) - 1;
        }
    }

    //the maximum number of elements that can be inserted befor eold elements get overwritten
    public uint maxSize
    {
        get
        {
            return (uint)elements.Length;
        }
    }

    public FixedCircularQueue(uint size)
    {
        elements = new T[size];

        currentStartIndex = size == 0 ? 0 : size - 1;
    }

    public void Enqueue(T element)
    {
        if (elements.Length == 0)
        {
            return;
        }

        elements[currentStartIndex] = element;

        if(_size < elements.Length)
        {
            ++_size;
        }

        //decrement start index
        if(currentStartIndex == 0)
        {
            currentStartIndex = (uint)elements.Length - 1; 
        }
        else
        {
            --currentStartIndex;
        }
    }

    //calculate the index in the internal array of the element which appears to be at apparentIndex
    //does validation
    private uint CalculateInternalIndex(uint apparentIndex)
    {
        if (apparentIndex >= _size)
        {
            throw new IndexOutOfRangeException("Index: " + apparentIndex + ", size: " + _size);
        }

        apparentIndex += currentStartIndex;
        if (apparentIndex >= elements.Length)
        {
            apparentIndex -= (uint)elements.Length - 1;
        }

        return apparentIndex;
    }

    public T this[uint index]
    {
        get
        {
            return elements[CalculateInternalIndex(index)];
        }
        set
        {
            elements[CalculateInternalIndex(index)] = value;
        }
    }

    //Changes the fixed size of the queue
    //Extra space is added at and surplus elements are removed from the end.
    public void Resize(uint newSize)
    {
        T[] newBackingArray = new T[newSize];

        //we'll use apparant indexes for this, it's too hard to use real indexes
        for(uint index = 0; index < Math.Min(_size, newSize); ++index)
        {
            newBackingArray[index] = this[index];
        }

        //now use the new array
        currentStartIndex = 0;
        elements = newBackingArray;
        _size = newSize;
        
    }

    public override string ToString()
    {
        StringBuilder retval = new StringBuilder();
        retval.Append('{');

        //we'll use apparant indexes for this, it's too hard to use real indexes
        for (uint index = 0; index < _size; ++index)
        {
            retval.Append('[');
            retval.Append(this[index].ToString());
            retval.Append(']');

            if(index < _size - 1)
            {
                retval.Append(", ");
            }
        }

        retval.Append('}');

        return retval.ToString();
    }
}



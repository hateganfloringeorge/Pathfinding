using System;

public class MinHeap
{
    private readonly Cell[] _elements;
    private int _size;

    public MinHeap(int size)
    {
        _elements = new Cell[size];
    }

    private int GetLeftChildIndex(int elementIndex) => 2 * elementIndex + 1;
    private int GetRightChildIndex(int elementIndex) => 2 * elementIndex + 2;
    private int GetParentIndex(int elementIndex) => (elementIndex - 1) / 2;

    private bool HasLeftChild(int elementIndex) => GetLeftChildIndex(elementIndex) < _size;
    private bool HasRightChild(int elementIndex) => GetRightChildIndex(elementIndex) < _size;
    private bool IsRoot(int elementIndex) => elementIndex == 0;

    private Cell GetLeftChild(int elementIndex) => _elements[GetLeftChildIndex(elementIndex)];
    private Cell GetRightChild(int elementIndex) => _elements[GetRightChildIndex(elementIndex)];
    private Cell GetParent(int elementIndex) => _elements[GetParentIndex(elementIndex)];

    private void Swap(int firstIndex, int secondIndex)
    {
        var temp = _elements[firstIndex];
        _elements[firstIndex] = _elements[secondIndex];
        _elements[secondIndex] = temp;
    }

    public bool IsEmpty()
    {
        return _size == 0;
    }

    public Cell Peek()
    {
        if (_size == 0)
            throw new IndexOutOfRangeException();

        return _elements[0];
    }

    public Cell Pop()
    {
        if (_size == 0)
            throw new IndexOutOfRangeException();

        var result = _elements[0];
        _elements[0] = _elements[_size - 1];
        _size--;

        ReCalculateDown();

        return result;
    }

    public void Add(Cell element)
    {
        if (_size == _elements.Length)
            throw new IndexOutOfRangeException();

        _elements[_size] = element;
        _size++;

        ReCalculateUp();
    }

    private void ReCalculateDown()
    {
        int index = 0;
        while (HasLeftChild(index))
        {
            var smallerIndex = GetLeftChildIndex(index);
            if (HasRightChild(index) && GetRightChild(index).fCost < GetLeftChild(index).fCost)
            {
                smallerIndex = GetRightChildIndex(index);
            }

            if (_elements[smallerIndex].fCost >= _elements[index].fCost)
            {
                break;
            }

            Swap(smallerIndex, index);
            index = smallerIndex;
        }
    }

    private void ReCalculateUp()
    {
        var index = _size - 1;
        while (!IsRoot(index) && _elements[index].fCost < GetParent(index).fCost)
        {
            var parentIndex = GetParentIndex(index);
            Swap(parentIndex, index);
            index = parentIndex;
        }
    }

    public bool Contains(Cell origin)
    {
        for (int i = 0; i < _size; i++)
        {
            if (origin == _elements[i])
                return true;
        }
        return false;
    }
}

using UnityEngine;
using System;

[Serializable]
public class Enum2DArray
{
    public int rows;
    public int columns;
    public FlipCode[] array;

    public Enum2DArray(int rows, int columns)
    {
        this.rows = rows;
        this.columns = columns;
        array = new FlipCode[rows * columns];
    }

    public FlipCode GetValue(int row, int column)
    {
        return array[row * columns + column];
    }

    public void SetValue(int row, int column, FlipCode value)
    {
        array[row * columns + column] = value;
    }
}
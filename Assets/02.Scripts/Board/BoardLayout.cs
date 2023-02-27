using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardLayout
{
    public int Row => _row;
    public int Column => _column;
    public float Size => _size;
    public float Spacing => _spacing;

    [SerializeField] private int _row;
    [SerializeField] private int _column;
    [SerializeField] private float _size;
    [SerializeField] private float _spacing;

    public BoardLayout(int row, int column, float size, float spacing)
    {
        _row = row;
        _column = column;
        _size = size;
        _spacing = spacing;
    }

    public override string ToString()
    {
        return $"Row : {_row} Column : {_column} Size : {_size} / Spacing : {_spacing}";
    }
}

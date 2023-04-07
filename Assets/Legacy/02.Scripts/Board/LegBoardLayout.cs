using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// board's layout data
/// </summary>
[System.Serializable]
public class LegBoardLayout
{
    public int Row => _row;
    public int Column => _column;    
    public float Size => _size;       
    public float Spacing => _spacing; 

    [SerializeField] private int _row;
    [SerializeField] private int _column;
    [SerializeField] private float _size;     // slot's size
    [SerializeField] private float _spacing;  // spacing between slots

    public LegBoardLayout(int row, int column, float size, float spacing)
    {
        _row = row;
        _column = column;
        _size = size;
        _spacing = spacing;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardLayout : MonoBehaviour
{
    public int Row => _row;
    public int Column => _column;
    public float Size => _size;
    public float Spacing => _spacing;

    [SerializeField] private int _row;
    [SerializeField] private int _column;
    [SerializeField] private float _size;
    [SerializeField] private float _spacing;

    public bool IsValid(int row, int column)
    {
        return 0 <= row && row < _row && 0 <= column && column < _column;
    }

    public Vector3 GetPosition(int row, int column)
    {
        var width = _size + _spacing;
        var height = _size + _spacing;
        var minX = _column / 2 * width * -1 + (_column % 2 == 0 ? width / 2 : 0);
        var minY = _row / 2 * height * -1 + (_row % 2 == 0 ? height / 2 : 0);

        return new Vector3(minX + column * width, minY + row * height);
    }

    public void GetRowColumn(Vector3 position, out int row, out int column)
    {
        var width = _size + _spacing;
        var height = _size + _spacing;
        var minX = _column / 2 * width * -1 + (_column % 2 == 0 ? width / 2 : 0);
        var minY = _row / 2 * height * -1 + (_row % 2 == 0 ? height / 2 : 0);

        row = Mathf.RoundToInt((position.y - minY) / height);
        column = Mathf.RoundToInt((position.x - minX) / width);
    }

    public int GetRow(Vector3 position)
    {
        GetRowColumn(position, out var row, out var column);

        return row;
    }

    public int GetColumn(Vector3 position)
    {
        GetRowColumn(position, out var row, out var column);

        return column;
    }

    public BoardLayout(int row, int column, float size, float spacing)
    {
        _row = row;
        _column = column;
        _size = size;
        _spacing = spacing;
    }
}

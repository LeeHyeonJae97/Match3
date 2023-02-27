using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardSlot
{
    public int Row => _row;
    public int Column => _column;
    public Vector2 Position => _position;
    public bool Refreshed { get; set; }
    public int HorizontallyRemovedCount { get; set; }
    public int VerticallyRemovedCount { get; set; }
    public BoardItem Item { get; set; }

    [SerializeField] private int _row;
    [SerializeField] private int _column;
    [SerializeField] private Vector2 _position;

    public BoardSlot(int row, int column, Vector2 position)
    {
        _row = row;
        _column = column;
        _position = position;
        Refreshed = false;
        HorizontallyRemovedCount = 0;
        VerticallyRemovedCount = 0;
        Item = null;
    }

    public BoardSlot(BoardSlot slot)
    {
        _row = slot.Row;
        _column = slot.Column;
        _position = slot.Position;
        Refreshed = false;
        HorizontallyRemovedCount = 0;
        VerticallyRemovedCount = 0;
        Item = null;
    }

    public override string ToString()
    {
        return $"({Column},{Row}) / {Item?.name}";
    }
}

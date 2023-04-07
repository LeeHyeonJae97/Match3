using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardSlot
{
    public int Row => _row;
    public int Column => _column;
    public Vector2 Position => _position;
    public bool Refreshed { get; set; }               // is moved toward new slot recently
    public int HorizontallyRemovedCount { get; set; } // how many items on same row are removed simultaneously
    public int VerticallyRemovedCount { get; set; }   // how many items on same column are removed simultaneously
    public int GroupIndex { get; set; }               // item group's index that removed together
    public bool Removed { get; set; }                 // if true, act like removed already
    public BoardItem Item { get; set; }               // item that currently in this slot

    public bool Empty => Item == null;
    

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
        GroupIndex = 0;
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
        GroupIndex = 0;
        Removed = false;
        Item = null;
    }

    // reset current state
    public void Reset()
    {
        Refreshed = false;
        HorizontallyRemovedCount = 0;
        VerticallyRemovedCount = 0;
        GroupIndex = 0;
    }

    /// <summary>
    /// called when screen swiped
    /// </summary>
    /// <param name="board"></param>
    /// <param name="direction"></param>    
    public void OnSwiped(LegBoard board, int direction)
    {
        if (Item == null) return;

        Item.OnSwiped(board, direction, this);
    }

    /// <summary>
    /// called when removed
    /// </summary>
    /// <param name="board"></param>
    /// <param name="destroyer"></param>
    public void OnRemoved(LegBoard board, BoardItemType destroyer)
    {
        if (Item == null) return;

        Item.OnRemoved(board, destroyer, this);
    }

    public override string ToString()
    {
        return $"({Column},{Row})";
    }
}

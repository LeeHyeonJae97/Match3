using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSlot
{
    public int Row { get; set; }
    public int Column { get; set; }
    public Vector2 Position { get; set; }
    public bool Locked { get; set; }
    public BoardItem Item { get; set; }

    public BoardSlot(int row, int column, Vector2 position)
    {
        Row = row;
        Column = column;
        Position = position;
        Locked = false;
        Item = null;
    }

    public BoardSlot(int row, int column, Vector2 position, bool locked)
    {
        Row = row;
        Column = column;
        Position = position;
        Locked = locked;
        Item = null;
    }
}

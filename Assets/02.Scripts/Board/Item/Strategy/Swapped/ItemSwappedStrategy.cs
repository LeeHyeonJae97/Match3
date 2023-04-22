using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemSwappedStrategy : ScriptableObject
{
    protected const string PATH = "ScriptableObject/Board/Item/Strategy/";

    public abstract void OnSwapped(Vector2Int direction, BoardBehaviour boardBehaviour, Board board, BoardLayout boardLayout, ItemBehaviour itemBehaviour);

    protected bool GetNeighborInDirection(Vector2Int direction, Board board, BoardLayout boardLayout, ItemBehaviour itemBehaviour, out ItemBehaviour neighbor)
    {
        boardLayout.GetRowColumn(itemBehaviour, out var row, out var column);

        var nrow = row + direction.y;
        var ncolumn = column + direction.x;

        neighbor = boardLayout.IsValid(nrow, ncolumn) ? board.GetItemBehaviour(nrow, ncolumn) : null;

        return neighbor != null;
    }
}

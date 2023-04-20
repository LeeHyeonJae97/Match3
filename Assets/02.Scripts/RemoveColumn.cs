using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = NAME, menuName = PATH + NAME)]
public class RemoveColumn : ItemRemovedStrategy
{
    protected const string NAME = nameof(RemoveColumn);

    public override void OnRemoved(List<ItemBehaviour> matched, Board board, BoardLayout boardLayout, ItemBehaviour itemBehaviour)
    {
        var col = boardLayout.GetColumn(itemBehaviour.transform.position);

        for (int row = 0; row < boardLayout.Row; row++)
        {
            matched.Add(board.GetItemBehaviour(row, col));
        }
    }
}

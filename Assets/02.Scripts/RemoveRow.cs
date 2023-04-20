using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = NAME, menuName = PATH + NAME)]
public class RemoveRow : ItemRemovedStrategy
{
    protected const string NAME = nameof(RemoveRow);

    public override void OnRemoved(List<ItemBehaviour> matched, Board board, BoardLayout boardLayout, ItemBehaviour itemBehaviour)
    {
        var row = boardLayout.GetRow(itemBehaviour.transform.position);

        for (int column = 0; column < boardLayout.Column; column++)
        {
            matched.Add(board.GetItemBehaviour(row, column));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = NAME, menuName = PATH + NAME)]
public class RemoveColor : ItemRemovedStrategy
{
    protected const string NAME = nameof(RemoveColor);

    public override void OnRemoved(List<ItemBehaviour> matched, ItemBehaviour remover, Board board, BoardLayout boardLayout, ItemBehaviour itemBehaviour)
    {
        if (matched.Contains(itemBehaviour)) return;

        matched.Add(itemBehaviour);

        for (int row = 0; row < boardLayout.Row; row++)
        {
            for (int column = 0; column < boardLayout.Column; column++)
            {
                var removed = board.GetItemBehaviour(row, column);

                if (removed != null && removed.IsSame(remover))
                {
                    removed.OnRemoved(matched, itemBehaviour);
                }
            }
        }
    }
}

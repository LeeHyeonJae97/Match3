using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = NAME, menuName = PATH + NAME)]
public class RemoveNeighbors : ItemRemovedStrategy
{
    protected const string NAME = nameof(RemoveNeighbors);

    public override void OnRemoved(List<ItemBehaviour> matched, ItemBehaviour remover, Board board, ItemBehaviour itemBehaviour)
    {
        if (matched.Contains(itemBehaviour)) return;

        matched.Add(itemBehaviour);

        board.Layout.GetRowColumn(itemBehaviour, out var row, out var column);

        for (int r = row - 1; r <= row + 1; r++)
        {
            for (int c = column - 1; c <= column + 1; c++)
            {
                var removed = board.GetItemBehaviour(r, c);

                if (removed != null)
                {
                    removed.OnRemoved(matched, itemBehaviour);
                }
            }
        }
    }
}

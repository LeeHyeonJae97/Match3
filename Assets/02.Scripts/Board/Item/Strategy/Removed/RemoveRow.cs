using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = NAME, menuName = PATH + NAME)]
public class RemoveRow : ItemRemovedStrategy
{
    protected const string NAME = nameof(RemoveRow);

    public override void OnRemoved(List<ItemBehaviour> matched, ItemBehaviour remover, Board board, ItemBehaviour itemBehaviour)
    {
        if (matched.Contains(itemBehaviour)) return;

        matched.Add(itemBehaviour);

        if (remover == null)
        {
            RemoveRow(board.Layout.GetRow(itemBehaviour));
        }
        else
        {
            board.Layout.GetRowColumn(remover, out var removerRow, out var removerColumn);
            board.Layout.GetRowColumn(itemBehaviour, out var row, out var column);

            if (removerRow == row)
            {
                RemoveColumn(column);
            }
            else
            {
                RemoveRow(row);
            }
        }

        // LOCAL FUNCTION
        void RemoveRow(int row)
        {
            for (int c = 0; c < board.Layout.Column; c++)
            {
                var removed = board.GetItemBehaviour(row, c);

                if (removed != null)
                {
                    removed.OnRemoved(matched, itemBehaviour);
                }
            }
        }

        // LOCAL FUNCTION
        void RemoveColumn(int column)
        {
            for (int r = 0; r < board.Layout.Row; r++)
            {
                var removed = board.GetItemBehaviour(r, column);

                if (removed != null)
                {
                    removed.OnRemoved(matched, itemBehaviour);
                }
            }
        }
    }
}

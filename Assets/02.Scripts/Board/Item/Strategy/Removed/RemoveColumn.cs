using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = NAME, menuName = PATH + NAME)]
public class RemoveColumn : ItemRemovedStrategy
{
    protected const string NAME = nameof(RemoveColumn);

    public override void OnRemoved(List<ItemBehaviour> matched, ItemBehaviour remover, Board board, ItemBehaviour itemBehaviour)
    {
        if (matched.Contains(itemBehaviour)) return;

        matched.Add(itemBehaviour);

        if (remover == null)
        {
            RemoveColumn(board.Layout.GetColumn(itemBehaviour));
        }
        else
        {
            board.Layout.GetRowColumn(remover, out var removerRow, out var removerColumn);
            board.Layout.GetRowColumn(itemBehaviour, out var row, out var column);

            if (removerColumn == column)
            {
                RemoveRow(row);
            }
            else
            {
                RemoveColumn(column);
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

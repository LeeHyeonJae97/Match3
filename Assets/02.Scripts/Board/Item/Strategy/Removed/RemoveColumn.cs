using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = NAME, menuName = PATH + NAME)]
public class RemoveColumn : ItemRemovedStrategy
{
    protected const string NAME = nameof(RemoveColumn);

    public override void OnRemoved(List<ItemBehaviour> matched, ItemBehaviour remover, Board board, BoardLayout boardLayout, ItemBehaviour itemBehaviour)
    {
        if (matched.Contains(itemBehaviour)) return;

        matched.Add(itemBehaviour);

        if (remover == null)
        {
            RemoveColumn(boardLayout.GetColumn(itemBehaviour));
        }
        else
        {
            boardLayout.GetRowColumn(remover, out var removerRow, out var removerColumn);
            boardLayout.GetRowColumn(itemBehaviour, out var row, out var column);

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
            for (int c = 0; c < boardLayout.Column; c++)
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
            for (int r = 0; r < boardLayout.Row; r++)
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

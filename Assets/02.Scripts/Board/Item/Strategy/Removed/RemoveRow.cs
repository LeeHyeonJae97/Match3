using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = NAME, menuName = PATH + NAME)]
public class RemoveRow : ItemRemovedStrategy
{
    protected const string NAME = nameof(RemoveRow);

    public override void OnRemoved(List<ItemBehaviour> matched, ItemBehaviour remover, Board board, BoardLayout boardLayout, ItemBehaviour itemBehaviour)
    {
        if (matched.Contains(itemBehaviour)) return;

        matched.Add(itemBehaviour);

        if (remover == null)
        {
            RemoveRow(boardLayout.GetRow(itemBehaviour));
        }
        else
        {
            boardLayout.GetRowColumn(remover, out var removerRow, out var removerColumn);
            boardLayout.GetRowColumn(itemBehaviour, out var row, out var column);

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

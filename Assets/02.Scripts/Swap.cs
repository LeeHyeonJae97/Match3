using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = NAME, menuName = PATH + NAME)]
public class Swap : ItemSwappedStrategy
{
    protected const string NAME = nameof(Swap);

    public override void OnSwapped(Vector2Int direction, ItemBehaviour item, BoardBehaviour boardBehaviour, Board board, BoardLayout layout)
    {
        if (GetSwappedItem(out var swapped))
        {
            item. StartCoroutine(CoSwap());
        }

        // LOCAL FUNCTION
        bool GetSwappedItem(out ItemBehaviour swapped)
        {
            layout.GetRowColumn(item.transform.position, out var row, out var column);

            var nrow = row + direction.y;
            var ncolumn = column + direction.x;

            swapped = layout.IsValid(nrow, ncolumn) ? board.GetItemBehaviour(nrow, ncolumn) : null;

            return swapped != null;
        }

        // LOCAL FUNCTION
        IEnumerator CoSwap()
        {
            Coroutine[] cors =
            {
               item. StartCoroutine(item.CoMove(swapped.transform.position)),
                item.StartCoroutine(swapped.CoMove(item.transform.position))
            };

            foreach (var cor in cors)
            {
                yield return cor;
            }

            board.GetSlot(item.transform.position).Refreshed = true;
            board.GetSlot(swapped.transform.position).Refreshed = true;

            if (!boardBehaviour.Matchable())
            {
                cors[0] = item.StartCoroutine(item.CoMove(swapped.transform.position));
                cors[1] = item.StartCoroutine(swapped.CoMove(item.transform.position));

                foreach (var cor in cors)
                {
                    yield return cor;
                }
            }
            else
            {
                boardBehaviour.Match();
            }
        }
    }
}

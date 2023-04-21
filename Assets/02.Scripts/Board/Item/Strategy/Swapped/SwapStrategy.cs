using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = NAME, menuName = PATH + NAME)]
public class SwapStrategy : ItemSwappedStrategy
{
    protected const string NAME = nameof(SwapStrategy);

    public override void OnSwapped(Vector2Int direction, ItemBehaviour item, BoardBehaviour boardBehaviour, Board board, BoardLayout boardLayout)
    {
        if (GetSwappedItem(out var swapped))
        {
            item.StartCoroutine(CoSwap());
        }

        // LOCAL FUNCTION
        bool GetSwappedItem(out ItemBehaviour swapped)
        {
            boardLayout.GetRowColumn(item, out var row, out var column);

            var nrow = row + direction.y;
            var ncolumn = column + direction.x;

            swapped = boardLayout.IsValid(nrow, ncolumn) ? board.GetItemBehaviour(nrow, ncolumn) : null;

            return swapped != null;
        }

        // LOCAL FUNCTION
        IEnumerator CoSwap()
        {
            var itemPos = item.transform.position;
            var swappedPos = swapped.transform.position;

            Coroutine[] cors =
            {
                item.StartCoroutine(item.CoMove(swappedPos)),
                item.StartCoroutine(swapped.CoMove(itemPos))
            };

            foreach (var cor in cors)
            {
                yield return cor;
            }

            if (!boardBehaviour.Matchable())
            {
                cors[0] = item.StartCoroutine(item.CoMove(itemPos));
                cors[1] = item.StartCoroutine(swapped.CoMove(swappedPos));

                foreach (var cor in cors)
                {
                    yield return cor;
                }
            }
            else
            {
                board.GetSlot(item).Refreshed = true;
                board.GetSlot(swapped).Refreshed = true;

                boardBehaviour.Match();
            }
        }
    }
}

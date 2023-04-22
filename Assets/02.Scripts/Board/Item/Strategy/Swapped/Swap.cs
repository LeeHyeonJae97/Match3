using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = NAME, menuName = PATH + NAME)]
public class Swap : ItemSwappedStrategy
{
    protected const string NAME = nameof(Swap);

    public override void OnSwapped(Vector2Int direction, BoardBehaviour boardBehaviour, Board board, BoardLayout boardLayout, ItemBehaviour itemBehaviour)
    {
        if (GetNeighborInDirection(direction, board, boardLayout, itemBehaviour, out var neighbor))
        {
            boardBehaviour.StartCoroutine(CoSwap());
        }

        // LOCAL FUNCTION
        IEnumerator CoSwap()
        {
            var itemPos = itemBehaviour.transform.position;
            var neighborPos = neighbor.transform.position;

            Coroutine[] cors =
            {
                boardBehaviour.StartCoroutine(itemBehaviour.CoMove(neighborPos)),
                boardBehaviour.StartCoroutine(neighbor.CoMove(itemPos))
            };

            foreach (var cor in cors)
            {
                yield return cor;
            }

            if (!boardBehaviour.Matchable())
            {
                cors[0] = boardBehaviour.StartCoroutine(itemBehaviour.CoMove(itemPos));
                cors[1] = boardBehaviour.StartCoroutine(neighbor.CoMove(neighborPos));

                foreach (var cor in cors)
                {
                    yield return cor;
                }
            }
            else
            {
                board.GetSlot(itemBehaviour).Refreshed = true;
                board.GetSlot(neighbor).Refreshed = true;

                boardBehaviour.Match();
            }
        }
    }
}

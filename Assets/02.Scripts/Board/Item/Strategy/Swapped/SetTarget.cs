using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = NAME, menuName = PATH + NAME)]
public class SetTarget : ItemSwappedStrategy
{
    protected const string NAME = nameof(SetTarget);

    public override void OnSwapped(Vector2Int direction, BoardBehaviour boardBehaviour, Board board, ItemBehaviour itemBehaviour)
    {
        if (GetNeighborInDirection(direction, board, itemBehaviour, out var neighbor))
        {
            itemBehaviour.StartCoroutine(CoMove());
        }

        // LOCAL FUNCTION
        IEnumerator CoMove()
        {
            var itemPos = itemBehaviour.transform.position;
            var neighborPos = neighbor.transform.position;

            yield return itemBehaviour.StartCoroutine(itemBehaviour.CoMove(neighborPos));

            itemBehaviour.transform.position = itemPos;

            boardBehaviour.Match(itemBehaviour, neighbor);
        }
    }
}

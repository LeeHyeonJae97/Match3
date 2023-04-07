using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ball", menuName = "ScriptableObject/BoardItem/Ball")]
public class BoardItemDataBall : BoardItemData
{
#if UNITY_EDITOR
    private void OnValidate()
    {
        _type = BoardItemType.Ball;
        _color = BoardItemColor.None;
    }
#endif

    public override void OnRemoved(LegBoard board, BoardItemType destroyer, BoardSlot slot)
    {
        if (board.Matched.Contains(slot)) return;

        // if destroyed by special item, set roll direction by destroyer item's type
        if (destroyer == BoardItemType.StripedHCandy)
        {
            OnSwiped(board, Random.Range(0, 2) == 0 ? 1 : 3, slot);
        }
        else if (destroyer == BoardItemType.StripedVCandy)
        {
            OnSwiped(board, Random.Range(0, 2) == 0 ? 0 : 2, slot);
        }
        else if (destroyer == BoardItemType.WrappedCandy)
        {
            OnSwiped(board, Random.Range(0, 4), slot);
        }
        else
        {
            board.Matched.Add(slot);
        }
    }

    public override void OnSwiped(LegBoard board, int direction, BoardSlot slot)
    {
        if (slot.Item.Move) return;

        var row = slot.Row;
        var column = slot.Column;
        var item = slot.Item;

        board.StartCoroutine(CoRoll());

        // LOCAL FUNCTION
        IEnumerator CoRoll()
        {
            // block input
            LegInputManager.Instance.enabled = false;

            Vector2Int offset = direction == 0 ? Vector2Int.right : direction == 1 ? Vector2Int.up : direction == 2 ? Vector2Int.left : direction == 3 ? Vector2Int.down : Vector2Int.zero;
            var nrow = row;
            var ncolumn = column;

            slot.Removed = true;

            while (true)
            {
                nrow += offset.y;
                ncolumn += offset.x;

                // if can't move anymore, remove this item
                if (!(0 <= nrow && nrow < board.Row && 0 <= ncolumn && ncolumn < board.Column))
                {
                    slot.Removed = false;
                    board.Remove(item.Data.Type, row, column, 0, 0, 0);
                    board.Return();
                    board.Reset();
                    break;
                }

                // move toward target slot
                item.TargetPosition = board.Slots[nrow, ncolumn].Position;
                item.Move = true;

                yield return new WaitUntil(() => !item.Move);

                // remove target slot's item
                board.Remove(item.Data.Type, nrow, ncolumn, 0, 0, 0);
                board.Return();
                board.Reset();
            }
            yield return board.StartCoroutine(board.CoRefill(false));
            yield return board.StartCoroutine(board.CoMatchAfterRefill());

            Stage.Instance.RemainTryCount--;

            // restore input
            LegInputManager.Instance.enabled = true;
        }
    }
}

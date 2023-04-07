using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RainbowCandy", menuName = "ScriptableObject/BoardItem/RainbowCandy")]
public class BoardItemDataRainbowCandy : BoardItemData
{
#if UNITY_EDITOR
    private void OnValidate()
    {
        _type = BoardItemType.RainbowCandy;
    }
#endif

    public override void OnRemoved(LegBoard board, BoardItemType destroyer, BoardSlot slot)
    {
        if (!board.Matched.Contains(slot))
        {
            board.Matched.Add(slot);
        }
        board.Clear(this);
    }

    public override void OnSwiped(LegBoard board, int direction, BoardSlot slot)
    {
        board.StartCoroutine(CoRemoveTargets());

        // LOCAL FUNCTION
        IEnumerator CoRemoveTargets()
        {
            LegInputManager.Instance.enabled = false;

            var row = slot.Row;
            var column = slot.Column;
            var nrow = row + (direction == 1 ? 1 : direction == 3 ? -1 : 0);
            var ncolumn = column + (direction == 0 ? 1 : direction == 2 ? -1 : 0);

            var swapable = 0 <= nrow && nrow < board.Row && 0 <= ncolumn && ncolumn < board.Column;

            if (swapable)
            {
                board.Remove((BoardItemType)(-1), row, column, 0, 0, 0);
                // remove all the same color's candy
                Remove(board.Slots[nrow, ncolumn].Item.Data);
                board.Return();
                board.Reset();
                yield return board.StartCoroutine(board.CoRefill(false));
                yield return board.StartCoroutine(board.CoMatchAfterRefill());

                Stage.Instance.RemainTryCount--;
            }

            LegInputManager.Instance.enabled = true;
        }

        // LOCAL FUNCTION
        void Remove(BoardItemData data)
        {
            for (int r = 0; r < board.Row; r++)
            {
                for (int c = 0; c < board.Column; c++)
                {
                    var slot = board.Slots[r, c];
                    
                    if (slot.Item.Data.IsSameCandy(data))
                    {
                        board.Remove((BoardItemType)(-1), r, c, 0, 0, 0);
                    }
                }
            }
        }
    }
}

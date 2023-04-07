using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Candy", menuName = "ScriptableObject/BoardItem/Candy")]
public class BoardItemDataCandy : BoardItemData
{
#if UNITY_EDITOR
    private void OnValidate()
    {
        _type = BoardItemType.Candy;
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
        board.StartCoroutine(CoSwap());

        // LOCAL FUNCTION
        IEnumerator CoSwap()
        {
            // block input
            LegInputManager.Instance.enabled = false;

            var row = slot.Row;
            var column = slot.Column;

            // if swappable, swap
            if (board.Swap(row, column, direction, true, false))
            {
                yield return new WaitUntil(board.IsBoardStill);

                board.Match();
                if (board.Matched.Count == 0)
                {
                    board.Swap(row, column, direction, true, false);
                    yield return new WaitUntil(board.IsBoardStill);
                }
                else
                {
                    while (board.Matched.Count > 0)
                    {
                        board.FillSpecial();
                        board.Return();
                        board.Reset();
                        yield return board.StartCoroutine(board.CoRefill(false));
                        board.Match();
                    }

                    Stage.Instance.RemainTryCount--;
                }
                board.Reset();
            }

            // restore input
            LegInputManager.Instance.enabled = true;
        }
    }
}

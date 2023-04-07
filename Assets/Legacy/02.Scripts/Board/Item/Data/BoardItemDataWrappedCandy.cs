using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WrappedCandy", menuName = "ScriptableObject/BoardItem/WrappedCandy")]
public class BoardItemDataWrappedCandy : BoardItemDataCandy
{
#if UNITY_EDITOR
    private void OnValidate()
    {
        _type = BoardItemType.WrappedCandy;
    }
#endif

    public override void OnRemoved(LegBoard board, BoardItemType destroyer, BoardSlot slot)
    {
        if (!board.Matched.Contains(slot))
        {
            board.Matched.Add(slot);
        }
        board.Clear(this);

        RemoveDiamond();

        // LOCAL FUNCTION
        void RemoveDiamond()
        {
            var row = slot.Row;
            var column = slot.Column;
            // the diamond's size
            int num = 2;

            for (int r = row - num; r <= row + num; r++)
            {
                int abs = num - Mathf.Abs(r - row);

                for (int c = column - abs; c <= column + abs; c++)
                {
                    if (0 <= r && r < board.Row && 0 <= c && c < board.Column)
                    {
                        board.Remove(_type, r, c, 0, 0, 0);
                    }
                }
            }
        }
    }
}

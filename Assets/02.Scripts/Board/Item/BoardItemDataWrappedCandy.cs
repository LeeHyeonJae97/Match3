using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WrappedCandy", menuName = "ScriptableObject/BoardItem/WrappedCandy")]
public class BoardItemDataWrappedCandy : BoardItemData
{
    public override void OnDestroyed(Board board, BoardItemType destroyer, BoardSlot slot)
    {
        if (!board.Matched.Contains(slot)) board.Matched.Add(slot);
        board.Clear(Type);
        board.RemoveCross(slot);
    }

    public override void OnSwiped(Board board)
    {
        board.Swap();
    }
}

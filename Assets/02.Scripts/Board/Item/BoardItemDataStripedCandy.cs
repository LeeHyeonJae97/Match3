using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StripedCandy", menuName = "ScriptableObject/BoardItem/StripedCandy")]
public class BoardItemDataStripedCandy : BoardItemData
{
    public override void OnDestroyed(Board board, BoardItemType destroyer, BoardSlot slot)
    {
        if (!board.Matched.Contains(slot)) board.Matched.Add(slot);
        board.Clear(Type);
        board.RemoveRow(slot);
    }

    public override void OnSwiped(Board board)
    {
        board.Swap();
    }
}

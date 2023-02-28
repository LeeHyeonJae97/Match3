using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RainbowCandy", menuName = "ScriptableObject/BoardItem/RainbowCandy")]
public class BoardItemDataRainbowCandy : BoardItemData
{
    public override void OnDestroyed(Board board, BoardItemType destroyer, BoardSlot slot)
    {
        if (!board.Matched.Contains(slot)) board.Matched.Add(slot);
        board.Clear(Type);
    }

    public override void OnSwiped(Board board)
    {
        board.RemoveTargets();
    }
}

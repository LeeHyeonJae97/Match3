using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ball", menuName = "ScriptableObject/BoardItem/Ball")]
public class BoardItemDataBall : BoardItemData
{
    public override void OnDestroyed(Board board, BoardItemType destroyer, BoardSlot slot)
    {
        if (!board.Matched.Contains(slot)) board.Matched.Add(slot);
    }

    public override void OnSwiped(Board board)
    {
        board.Roll();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hall", menuName = "ScriptableObject/BoardItem/Hall")]
public class BoardItemDataHall : BoardItemData
{
    public override void OnDestroyed(Board board, BoardItemType destroyer, BoardSlot slot)
    {
        if (destroyer == BoardItemType.Ball)
        {
            if(!board.Matched.Contains(slot)) board.Matched.Add(slot);
            board.Clear(Type);
        }
    }

    public override void OnSwiped(Board board)
    {

    }
}

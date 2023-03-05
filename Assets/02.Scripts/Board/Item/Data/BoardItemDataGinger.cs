using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ginger", menuName = "ScriptableObject/BoardItem/Ginger")]
public class BoardItemDataGinger : BoardItemData
{
    public override void OnRemoved(Board board, BoardItemType destroyer, BoardSlot slot)
    {
        throw new System.NotImplementedException();
    }

    public override void OnSwiped(Board board, int direction, BoardSlot slot)
    {
        throw new System.NotImplementedException();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cookie", menuName = "ScriptableObject/BoardItem/Cookie")]
public class BoardItemDataCookie : BoardItemData
{
    public override void OnRemoved(LegBoard board, BoardItemType destroyer, BoardSlot slot)
    {
        throw new System.NotImplementedException();
    }

    public override void OnSwiped(LegBoard board, int direction, BoardSlot slot)
    {
        throw new System.NotImplementedException();
    }
}

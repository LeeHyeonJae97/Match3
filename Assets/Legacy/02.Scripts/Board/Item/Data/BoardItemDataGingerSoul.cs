using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GingerSoul", menuName = "ScriptableObject/BoardItem/GingerSoul")]
public class BoardItemDataGingerSoul : BoardItemData
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

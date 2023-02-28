using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ice", menuName = "ScriptableObject/BoardItem/Ice")]
public class BoardItemDataIce : BoardItemData
{
    public override void OnDestroyed(Board board, BoardItemType destroyer, BoardSlot slot)
    {
        throw new System.NotImplementedException();
    }

    public override void OnSwiped(Board board)
    {
        throw new System.NotImplementedException();
    }
}

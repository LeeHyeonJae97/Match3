using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CookieSoul", menuName = "ScriptableObject/BoardItem/CookieSoul")]
public class BoardItemDataCookieSoul : BoardItemData
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

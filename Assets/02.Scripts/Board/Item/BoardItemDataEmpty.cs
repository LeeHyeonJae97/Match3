using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Empty", menuName = "ScriptableObject/BoardItem/Empty")]
public class BoardItemDataEmpty : BoardItemData
{
    public override void OnDestroyed(Board board, BoardItemType destroyer, BoardSlot slot)
    {

    }

    public override void OnSwiped(Board board)
    {

    }
}

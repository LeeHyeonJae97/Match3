using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Empty", menuName = "ScriptableObject/BoardItem/Empty")]
public class BoardItemDataEmpty : BoardItemData
{
#if UNITY_EDITOR
    private void OnValidate()
    {
        _type = BoardItemType.Empty;
        _color = BoardItemColor.None;
    }
#endif

    public override void OnRemoved(Board board, BoardItemType destroyer, BoardSlot slot)
    {

    }

    public override void OnSwiped(Board board, int direction, BoardSlot slot)
    {

    }
}

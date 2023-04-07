using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hall", menuName = "ScriptableObject/BoardItem/Hall")]
public class BoardItemDataHall : BoardItemData
{
#if UNITY_EDITOR
    private void OnValidate()
    {
        _type = BoardItemType.Hall;
        _color = BoardItemColor.None;
    }
#endif

    public override void OnRemoved(LegBoard board, BoardItemType destroyer, BoardSlot slot)
    {
        // only check stage clear condition when removed by ball
        if (destroyer == BoardItemType.Ball)
        {
            board.Clear(this);
        }
    }

    public override void OnSwiped(LegBoard board, int direction, BoardSlot slot)
    {

    }
}

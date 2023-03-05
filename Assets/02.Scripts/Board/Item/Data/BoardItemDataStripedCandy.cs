using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StripedCandy", menuName = "ScriptableObject/BoardItem/StripedCandy")]
public class BoardItemDataStripedCandy : BoardItemDataCandy
{
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_type != BoardItemType.StripedHCandy && _type != BoardItemType.StripedVCandy)
        {
            _type = BoardItemType.StripedHCandy;
        }
    }
#endif

    public override void OnRemoved(Board board, BoardItemType destroyer, BoardSlot slot)
    {
        if (!board.Matched.Contains(slot))
        {
            board.Matched.Add(slot);
        }
        board.Clear(this);

        // if destroyed by special item, set removing direction by destroyer item's type
        if (destroyer == BoardItemType.StripedVCandy)
        {
            RemoveRow();
        }
        else if (destroyer == BoardItemType.StripedHCandy)
        {
            RemoveColumn();
        }
        else if (_type == BoardItemType.StripedVCandy)
        {
            RemoveColumn();
        }
        else if (_type == BoardItemType.StripedHCandy)
        {
            RemoveRow();
        }

        // LOCAL FUNCTION
        void RemoveRow()
        {
            var row = slot.Row;
            // do not need group index when removed by special item
            var groupIndex = -1;

            board.RemoveHorizontally(slot.Item.Data.Type, row, 0, board.Column - 1, 0, 0, ref groupIndex);
        }

        // LOCAL FUNCTION
        void RemoveColumn()
        {
            var column = slot.Column;
            // do not need group index when removed by special item
            var groupIndex = -1;

            board.RemoveVertically(slot.Item.Data.Type, column, 0, board.Row - 1, 0, 0, ref groupIndex);
        }
    }
}

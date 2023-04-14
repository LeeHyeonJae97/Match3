using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Board", menuName = "ScriptableObject/Board/Board")]
public class LegBoardData : ScriptableObject
{
    public int Row { get; private set; }
    public int Column { get; private set; }
    public BoardSlot[,] Slots => _slots;

    [SerializeField] private BoardItem _itemPrefab;
    private BoardSlot[,] _slots;

    public BoardSlot this[int row, int column]
    {
        get { return _slots[row, column]; }
    }

    public void Initialize(StageData data)
    {
        Row = data.BoardLayout.Row;
        Column = data.BoardLayout.Column;

        _slots = new BoardSlot[Row * 2, Column];

        var holder = new GameObject("Board");

        for (int r = 0; r < Row * 2; r++)
        {
            for (int c = 0; c < Column; c++)
            {
                // copy data's slot
                _slots[r, c] = new BoardSlot(data.Slots[r * Column + c]);

                if (r < Row)
                {
                    // instantiate and initialize item
                    var item = Instantiate(_itemPrefab);

                    item.transform.localScale = Vector2.one * data.BoardLayout.Size;
                    item.transform.SetParent(holder.transform);
                    item.Position = Slots[r, c].Position;
                    item.Data = data.ItemData[r * Column + c];

                    Slots[r, c].Item = item;
                }
            }
        }
    }

    public bool IsStill(int column)
    {   
        for (int r = 0; r < Row; r++)
        {
            bool isStill = !_slots[r, column].Empty;

            if (!isStill) return false;
        }

        return true;
    }
}

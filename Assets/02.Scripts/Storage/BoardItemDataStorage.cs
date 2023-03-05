using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardItemDataStorage : Storage<BoardItemData>
{
    private static Dictionary<(BoardItemType, BoardItemColor), BoardItemData> Items
    {
        get
        {
            if (_items == null)
            {
                _items = new Dictionary<(BoardItemType, BoardItemColor), BoardItemData>();

                var items = Resources.LoadAll<BoardItemData>("BoardItemData");

                for (int i = 0; i < items.Length; i++)
                {
                    _items[(items[i].Type, items[i].Color)] = items[i];
                }
            }

            return _items;
        }
    }

    private static Dictionary<(BoardItemType, BoardItemColor), BoardItemData> _items;

    public static BoardItemData Get(BoardItemType type, BoardItemColor color)
    {
        return Items.TryGetValue((type, color), out var data) ? data : null;
    }
}

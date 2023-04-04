using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class NewItemStorage : MonoBehaviour
{
    [SerializeField] private NewBoardLayout _layout;
    [SerializeField] private List<NewItem> _items;

    public void Add(NewItem item)
    {
        if (_items == null) _items = new List<NewItem>();

        _items.Add(item);
    }

    public NewItem GetItem(int row, int column)
    {
        if (!_layout.IsValid(row, column)) return null;

        return _items.Find((item) => item.transform.position == _layout.GetPosition(row, column));
    }

    private void OnDrawGizmos()
    {
        DrawRowColumnOfItems();

        // LOCAL FUNCTION
        void DrawRowColumnOfItems()
        {
            if (_items == null) return;

            foreach (var item in _items)
            {
                if (item != null)
                {
                    _layout.GetRowColumn(item.transform.position, out var row, out var column);
                    Handles.Label(item.transform.position, $"({column},{row})");
                }
            }
        }
    }
}

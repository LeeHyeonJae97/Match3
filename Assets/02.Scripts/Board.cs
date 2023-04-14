using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private BoardData _data;
    [SerializeField] private BoardLayout _layout;
    private List<Item> _items;
    private Slot[,] _slots;

    private void Awake()
    {
        _slots = new Slot[_layout.Row, _layout.Column];

        for (int row = 0; row < _layout.Row; row++)
        {
            for (int column = 0; column < _layout.Column; column++)
            {
                _slots[row, column] = new Slot();
            }
        }
    }

    public void Add(Item item)
    {
        if (_items == null) _items = new List<Item>();

        _items.Add(item);
    }

    public Item GetItem(Vector3 position)
    {
        return _items.Find((item) => item.transform.position == position);
    }

    public Item GetItem(int row, int column)
    {
        return GetItem(_layout.GetPosition(row, column));
    }

    public Slot GetSlot(Vector3 position)
    {
        _layout.GetRowColumn(position, out var row, out var column);

        return GetSlot(row, column);
    }

    public Slot GetSlot(int row, int column)
    {
        return _layout.IsValid(row, column) ? _slots[row, column] : null;
    }

    public void ResetSlots()
    {
        foreach (var slot in _slots)
        {
            slot.Reset();
        }
    }

    public void Load()
    {
        var colors = _data.Load();

        for (int i = 0; i < _items.Count; i++)
        {
            _items[i].SetColor(colors[i]);
        }
    }

    public void Save()
    {
        _data.Save(_items);
    }

    public void Shuffle()
    {
        foreach (var item in _items)
        {
            item.SetColor();
        }
    }

    private void OnDrawGizmos()
    {
        DrawItems();
        DrawSlots();

        // LOCAL FUNCTION
        void DrawItems()
        {
            if (_items == null) return;

            foreach (var item in _items)
            {
                if (item != null)
                {
                    _layout.GetRowColumn(item.transform.position, out var row, out var column);

                    var position = item.transform.position - new Vector3(item.transform.localScale.x / 2, 0, 0);

                    UnityEditor.Handles.Label(position, $"({column},{row})");
                }
            }
        }

        // LOCAL FUNCTION
        void DrawSlots()
        {
            if (_slots == null) return;

            for (int row = 0; row < _layout.Row; row++)
            {
                for (int column = 0; column < _layout.Column; column++)
                {
                    var slot = _slots[row, column];
                    var rs = slot.RowScore;
                    var cs = slot.ColumnScore;
                    var mg = slot.MatchGroup;
                    var refreshed = slot.Refreshed;

                    var position = _layout.GetPosition(row, column);

                    position.x -= _items[0].transform.localScale.x / 4;

                    UnityEditor.Handles.Label(position , $"({rs},{cs}) / {mg} / {refreshed}");
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "Board", menuName = "ScriptableObject/Board/Board")]
public class Board : ScriptableObject
{
    [SerializeField] private BoardLayout _layout;
    [SerializeField] private List<Item> _items;
    private List<ItemBehaviour> _itemBehaviours;
    private Slot[,] _slots;
    private int[] _data;

    public void Initialize(List<ItemBehaviour> itemBehaviours)
    {
        Assert.IsNotNull(itemBehaviours);
        Assert.AreEqual(_layout.Row * _layout.Column, itemBehaviours.Count);

        _itemBehaviours = itemBehaviours;

        _slots = new Slot[_layout.Row, _layout.Column];

        for (int row = 0; row < _layout.Row; row++)
        {
            for (int column = 0; column < _layout.Column; column++)
            {
                _slots[row, column] = new Slot();
            }
        }
    }

    public ItemBehaviour GetItemBehaviour(Vector3 position)
    {
        return _itemBehaviours.Find((item) => item.transform.position == position);
    }

    public ItemBehaviour GetItemBehaviour(int row, int column)
    {
        return GetItemBehaviour(_layout.GetPosition(row, column));
    }

    public Item GetItem(int id)
    {
        return _items.Find((item) => item.Id == id);
    }

    public Item GetItem(ItemType type)
    {
        var item = _items.FindAll((item) => item.Type == type);

        return item[Random.Range(0, item.Count)];
    }

    public Slot GetSlot(ItemBehaviour itemBehaviour)
    {
        _layout.GetRowColumn(itemBehaviour, out var row, out var column);

        return _layout.IsValid(row, column) ? _slots[row, column] : null;
    }

    public bool TryGetSlot(ItemBehaviour itemBehaviour, out Slot slot)
    {
        _layout.GetRowColumn(itemBehaviour, out var row, out var column);

        bool valid = _layout.IsValid(row, column);

        slot = valid ? _slots[row, column] : null;

        return valid;
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
        Assert.AreEqual(_layout.Row * _layout.Column, _data.Length);
        Assert.AreEqual(_layout.Row * _layout.Column, _itemBehaviours.Count);

        for (int i = 0; i < _data.Length; i++)
        {
            _itemBehaviours[i].Initialize(GetItem(_data[i]));
        }
    }

    public void Save()
    {
        Assert.AreEqual(_layout.Row * _layout.Column, _itemBehaviours.Count);

        _data = new int[_itemBehaviours.Count];

        for (int i = 0; i < _itemBehaviours.Count; i++)
        {
            _data[i] = _itemBehaviours[i].Item.Id;
        }
    }

    public void Shuffle()
    {

    }

    public void OnDrawGizmos()
    {
        DrawItems();
        DrawSlots();

        // LOCAL FUNCTION
        void DrawItems()
        {
            if (!Application.isPlaying || _itemBehaviours == null) return;

            foreach (var item in _itemBehaviours)
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
            if (!Application.isPlaying || _slots == null || _itemBehaviours == null) return;

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

                    position.x -= _itemBehaviours[0].transform.localScale.x / 4;

                    UnityEditor.Handles.Label(position , $"({rs},{cs}) / {mg} / {refreshed}");
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "Board", menuName = "ScriptableObject/Board/Board")]
public class Board : ScriptableObject
{
    public BoardLayout Layout => _layout;

    [SerializeField] private BoardLayout _layout;
    [SerializeField] private List<Item> _items;
    [SerializeField] private int[] _data;
    private List<ItemBehaviour> _itemBehaviours;
    private Slot[,] _slots;

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

    public ItemBehaviour GetItemBehaviourApproximately(Vector3 position)
    {
        return _itemBehaviours.Find((item) => item.transform.position.SqrDst(position) < 0.01f);
    }

    public ItemBehaviour GetItemBehaviourApproximately(int row, int column)
    {
        return GetItemBehaviourApproximately(_layout.GetPosition(row, column));
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

    public Item GetItem(ItemType type, ItemColor color)
    {
        return _items.Find((item) => item.Type == type && item.Color == color);
    }

    public Slot GetSlot(ItemBehaviour itemBehaviour)
    {
        _layout.GetRowColumn(itemBehaviour, out var row, out var column);

        return GetSlot(row, column);
    }

    public Slot GetSlot(int row, int column)
    {
        return _layout.IsValid(row, column) ? _slots[row, column] : null;
    }

    public bool TryGetSlot(ItemBehaviour itemBehaviour, out Slot slot)
    {
        _layout.GetRowColumn(itemBehaviour, out var row, out var column);

        bool valid = _layout.IsValid(row, column);

        slot = valid ? _slots[row, column] : null;

        return valid;
    }

    public void Reset()
    {
        foreach (var slot in _slots)
        {
            slot.Reset();
        }
    }

    [System.Obsolete]
    public void Load()
    {
        Assert.AreEqual(_layout.Row * _layout.Column, _data.Length);
        Assert.AreEqual(_layout.Row * _layout.Column, _itemBehaviours.Count);

        for (int i = 0; i < _data.Length; i++)
        {
            _itemBehaviours[i].Initialize(GetItem(_data[i]));
        }
    }

    [System.Obsolete]
    public void Save()
    {
        Assert.AreEqual(_layout.Row * _layout.Column, _itemBehaviours.Count);

        _data = new int[_itemBehaviours.Count];

        for (int i = 0; i < _itemBehaviours.Count; i++)
        {
            _data[i] = _itemBehaviours[i].Item.Id;
        }
    }
}

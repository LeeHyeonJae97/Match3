using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "Board", menuName = "ScriptableObject/Board/Board")]
public class Board : ScriptableObject
{
    [SerializeField] private int[] _data;
    [SerializeField] private BoardLayout _layout;
    [SerializeField] private List<Item> _items;
    private List<ItemBehaviour> _itemsBehaviours;
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

    public void Add(ItemBehaviour item)
    {
        if (_itemsBehaviours == null) _itemsBehaviours = new List<ItemBehaviour>();

        _itemsBehaviours.Add(item);
    }

    public ItemBehaviour GetItemBehaviour(Vector3 position)
    {
        return _itemsBehaviours.Find((item) => item.transform.position == position);
    }

    public ItemBehaviour GetItemBehaviour(int row, int column)
    {
        return GetItemBehaviour(_layout.GetPosition(row, column));
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

    public Item GetItem(int id)
    {
        return _items.Find((item) => item.Id == id);
    }

    public Item GetItem()
    {
        return _items[Random.Range(0, _items.Count)];
    }

    public void Load()
    {
        var ids = _data;

        for (int i = 0; i < _itemsBehaviours.Count; i++)
        {
            var item = _items.Find((item) => item.Id == ids[i]);

            Assert.AreEqual(null, item);

            _itemsBehaviours[i].Initialize(item);
        }

        [System.Obsolete]
        void LegLoad()
        {
            var colors = _data;

            for (int i = 0; i < _itemsBehaviours.Count; i++)
            {
                _itemsBehaviours[i].SetColor(colors[i]);
            }
        }
    }

    public void Save()
    {
        if (_data == null || _data.Length == 0) _data = new int[_itemsBehaviours.Count];

        for (int i = 0; i < _itemsBehaviours.Count; i++)
        {
            _data[i] = _itemsBehaviours[i].Data.Id;
        }
    }

    public void Shuffle()
    {
        foreach (var item in _itemsBehaviours)
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
            if (_itemsBehaviours == null) return;

            foreach (var item in _itemsBehaviours)
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

                    position.x -= _itemsBehaviours[0].transform.localScale.x / 4;

                    UnityEditor.Handles.Label(position , $"({rs},{cs}) / {mg} / {refreshed}");
                }
            }
        }
    }
}

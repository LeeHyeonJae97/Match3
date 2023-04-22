using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Candy, HSCandy, VSCandy, WCandy, Rainbow }
public enum ItemColor { Blue, Green, Red, Yellow, White, Black, Gray, None = -1 }

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/Board/Item/Item")]
public class Item : ScriptableObject
{
    public int Id => (int)_type * 10 + (int)_color;
    public ItemType Type => _type;
    public ItemColor Color => _color;
    public Sprite Sprite => _sprite;
    public ItemSwappedStrategy SwappedStrategy => _swappedStrategy;
    public ItemRemovedStrategy RemovedStrategy => _removedStrategy;

    [SerializeField] private ItemType _type;
    [SerializeField] private ItemColor _color;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private ItemSwappedStrategy _swappedStrategy;
    [SerializeField] private ItemRemovedStrategy _removedStrategy;

    public bool IsSame(Item item)
    {
        return _color == item.Color;
    }
}

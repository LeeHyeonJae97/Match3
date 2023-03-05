using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardItemType
{
    Empty,
    Candy,
    StripedHCandy,
    StripedVCandy,
    WrappedCandy,
    RainbowCandy, 
    Ball, 
    Hall, 
    Ice, 
    Chocolate, 
    Cookie, 
    CookieSoul, 
    Ginger, 
    GingerSoul,     
}
public enum BoardItemColor { None, Blue, Green, Red, Yellow }

public abstract class BoardItemData : ScriptableObject
{
    public BoardItemType Type => _type;
    public BoardItemColor Color => _color;
    public bool Selectable => _selectable;
    public bool Static => _static;
    public bool Destroyable => _destroyable;
    public bool Stackable => _stackable;
    public bool Penetratable => _penetratable;
    public int Layer => _layer;
    public Sprite Sprite => _sprite;

    [SerializeField] protected BoardItemType _type;
    [SerializeField] protected BoardItemColor _color;
    [SerializeField] protected bool _selectable;
    [SerializeField] protected bool _static;
    [SerializeField] protected bool _destroyable;
    [SerializeField] protected bool _stackable;
    [SerializeField] protected bool _penetratable;
    [SerializeField] protected int _layer;
    [SerializeField] protected Sprite _sprite;

    public abstract void OnRemoved(Board board, BoardItemType destroyer, BoardSlot slot);
    public abstract void OnSwiped(Board board, int direction, BoardSlot slot);

    public bool IsCandy()
    {
        return _type == BoardItemType.Candy || _type == BoardItemType.StripedHCandy || _type == BoardItemType.StripedVCandy || _type == BoardItemType.WrappedCandy;
    }

    public bool IsSameCandy(BoardItemData data)
    {
        return data == null ? false : IsCandy() && data.IsCandy() && _color == data.Color;
    }

    public bool IsSameType(BoardItemData data)
    {
        return data == null ? false : _type == data.Type && _color == data.Color;
    }

    public override string ToString()
    {
        return $"{Type}";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardItemType
{
    CandyBlue, CandyGreen, CandyRed, CandyYellow, // 0 ~ 3
    StripedCandyBlue, StripedCandyGreen, StripedCandyRed, StripedCandyYellow, // 4 ~ 7
    WrappedCandyBlue, WrappedCandyGreen, WrappedCandyRed, WrappedCandyYellow, // 8 ~ 11
    RainbowCandy, // 12
    Ball, // 13
    Hall, // 14
    Ice, // 15
    Chocolate, // 16
    Cookie, // 17
    CookieSoul, // 18
    Ginger, // 19
    GingerSoul, // 20
    Empty, // 21
}

public abstract class BoardItemData : ScriptableObject
{
    public BoardItemType Type => _type;
    public bool Selectable => _selectable;
    public bool Static => _static;
    public bool Destroyable => _destroyable;
    public bool Stackable => _stackable;
    public bool Penetratable => _penetratable;
    public int Layer => _layer;
    public Sprite Sprite => _sprite;

    [SerializeField] private BoardItemType _type;
    [SerializeField] private bool _selectable;
    [SerializeField] private bool _static;
    [SerializeField] private bool _destroyable;
    [SerializeField] private bool _stackable;
    [SerializeField] private bool _penetratable;
    [SerializeField] private int _layer;
    [SerializeField] private Sprite _sprite;

    public abstract void OnDestroyed(Board board, BoardItemType destroyer, BoardSlot slot);
    public abstract void OnSwiped(Board board);

    public override string ToString()
    {
        return $"{Type}";
    }
}

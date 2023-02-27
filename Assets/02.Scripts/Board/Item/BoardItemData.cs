using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardItemType
{
    Empty, 
    CandyBlue, CandyGreen, CandyRed, CandyYellow,
    RainbowCandy,
    StripedCandyBlue, StripedCandyGreen, StripedCandyRed, StripedCandyYellow,
    WrappedCandyBlue, WrappedCandyGreen, WrappedCandyRed, WrappedCandyYellow,
    Ball,
    Hall,
    Ice,
    Chocolate,
    Cookie,
    CookieSoul,
    Ginger,
    GingerSoul
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

    public abstract void OnDestroyed();
    public abstract void OnSwiped();

    public override string ToString()
    {
        return $"{Type}";
    }
}

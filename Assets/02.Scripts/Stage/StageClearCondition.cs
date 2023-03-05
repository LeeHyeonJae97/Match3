using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageClearCondition
{
    public BoardItemType Type => _type;
    public BoardItemColor Color => _color;
    public int Count => _count;

    [SerializeField] private BoardItemType _type;
    [SerializeField] private BoardItemColor _color;
    [SerializeField] private int _count;

    public StageClearCondition(BoardItemType type, BoardItemColor color, int count)
    {
        _type = type;
        _color = color;
        _count = count;
    }
}

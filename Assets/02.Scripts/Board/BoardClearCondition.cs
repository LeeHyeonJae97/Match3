using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardClearCondition
{
    public BoardItemType Type => _type;
    public int Count => _count;

    [SerializeField] private BoardItemType _type;
    [SerializeField] private int _count;

    public BoardClearCondition(BoardItemType type, int count)
    {
        _type = type;
        _count = count;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slot
{
    public int RowScore { get; set; }
    public int ColumnScore { get; set; }    
    //public Vector2Int Score
    //{
    //    get { return new Vector2Int(RowScore, ColumnScore); }

    //    set
    //    {
    //        RowScore = value.x;
    //        ColumnScore = value.y;
    //    }
    //}
    public bool Refreshed { get; set; }

    public Slot()
    {
        Reset();
    }

    public void Reset()
    {
        RowScore = 0;
        ColumnScore = 0;
        //Score = Vector2Int.zero;
        Refreshed = false;
    }

    public override string ToString()
    {
        return $"({RowScore},{ColumnScore}) / {Refreshed}";
    }
}

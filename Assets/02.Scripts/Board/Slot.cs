using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slot
{
    public int RowScore { get; set; }
    public int ColumnScore { get; set; }    
    public int MatchGroup { get; set; }
    public bool Refreshed { get; set; }

    public Slot()
    {
        Reset();
    }

    public void Reset()
    {
        RowScore = 0;
        ColumnScore = 0;
        MatchGroup = 0;
        Refreshed = false;
    }
}

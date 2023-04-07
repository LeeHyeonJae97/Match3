using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "ScriptableObject/Stage/Data")]
public class StageData : ScriptableObject
{
    [field: SerializeField] public int Level { get; set; }
    [field: SerializeField, TextArea] public string Description { get; set; }
    [field: SerializeField] public int TryCount { get; set; }
    [field: SerializeField] public StageClearCondition ClearCondition { get; set; }
    [field: SerializeField] public BoardSlot[] Slots { get; set; }
    [field: SerializeField] public BoardItemData[] ItemData { get; set; }
    [field: SerializeField] public LegBoardLayout BoardLayout { get; set; }
}

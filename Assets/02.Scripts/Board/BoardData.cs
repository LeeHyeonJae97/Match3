using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Board", menuName = "ScriptableObject/Board/Data")]
public class BoardData : ScriptableObject
{
    [field: SerializeField] public BoardSlot[] Slots { get; set; }
    [field: SerializeField] public BoardItemData[] ItemData { get; set; }
    [field: SerializeField] public BoardLayout Layout { get; set; }
}

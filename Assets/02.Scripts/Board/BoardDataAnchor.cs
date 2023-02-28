using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoardDataAnchor", menuName = "ScriptableObject/Board/BoardDataAnchor")]
public class BoardDataAnchor : ScriptableObject
{
    public BoardData Data { get; set; }
}

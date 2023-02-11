using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardItemType { Red, Green, Blue, White, Yellow, Black }

[CreateAssetMenu(fileName = "BoardItem", menuName = "ScriptableObject/BoardItem")]
public class BoardItemInfo : ScriptableObject
{
    [field: SerializeField] public BoardItemType Type { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public Color Color { get; private set; } = Color.white;
}

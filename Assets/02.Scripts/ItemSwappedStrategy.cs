using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemSwappedStrategy : ScriptableObject
{
    protected const string PATH = "ScriptableObject/Board/Item/Strategy/";

    public abstract void OnSwapped(Vector2Int direction, ItemBehaviour item, BoardBehaviour boardBehaviour, Board board, BoardLayout boardLayout);
}

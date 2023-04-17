using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemRemovedStrategy : ScriptableObject
{
    protected const string PATH = "ScriptableObject/Board/Item/Strategy/";

    public abstract void OnRemoved(List<ItemBehaviour> matched, ItemBehaviour itemBehaviour);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = NAME, menuName = PATH + NAME)]
public class Remove : ItemRemovedStrategy
{
    protected const string NAME = nameof(Remove);

    public override void OnRemoved(List<ItemBehaviour> matched, ItemBehaviour itemBehaviour)
    {
        matched.Add(itemBehaviour);
    }
}

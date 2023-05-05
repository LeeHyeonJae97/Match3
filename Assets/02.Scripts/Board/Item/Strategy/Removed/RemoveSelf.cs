using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = NAME, menuName = PATH + NAME)]
public class RemoveSelf : ItemRemovedStrategy
{
    protected const string NAME = nameof(RemoveSelf);

    public override void OnRemoved(List<ItemBehaviour> matched, ItemBehaviour remover, Board board, ItemBehaviour itemBehaviour)
    {
        if (matched.Contains(itemBehaviour)) return;

        matched.Add(itemBehaviour);
    }
}

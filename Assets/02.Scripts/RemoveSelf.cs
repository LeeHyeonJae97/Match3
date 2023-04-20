using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = NAME, menuName = PATH + NAME)]
public class RemoveSelf : ItemRemovedStrategy
{
    protected const string NAME = nameof(RemoveSelf);

    public override void OnRemoved(List<ItemBehaviour> matched, Board board, BoardLayout boardLayout, ItemBehaviour itemBehaviour)
    {
        matched.Add(itemBehaviour);
    }
}

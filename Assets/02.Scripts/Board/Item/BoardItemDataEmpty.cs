using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Empty", menuName = "ScriptableObject/BoardItem/Empty")]
public class BoardItemDataEmpty : BoardItemData
{
    public override void OnDestroyed()
    {
        throw new System.NotImplementedException();
    }

    public override void OnSwiped()
    {
        throw new System.NotImplementedException();
    }
}

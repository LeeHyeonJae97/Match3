using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chocolate", menuName = "ScriptableObject/BoardItem/Chocolate")]
public class BoardItemDataChocolate : BoardItemData
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

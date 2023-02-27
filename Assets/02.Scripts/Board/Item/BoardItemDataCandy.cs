using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Candy", menuName = "ScriptableObject/BoardItem/Candy")]
public class BoardItemDataCandy : BoardItemData
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

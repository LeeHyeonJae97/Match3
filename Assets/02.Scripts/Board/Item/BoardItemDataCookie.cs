using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cookie", menuName = "ScriptableObject/BoardItem/Cookie")]
public class BoardItemDataCookie : BoardItemData
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

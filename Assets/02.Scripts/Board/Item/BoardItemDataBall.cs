using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ball", menuName = "ScriptableObject/BoardItem/Ball")]
public class BoardItemDataBall : BoardItemData
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

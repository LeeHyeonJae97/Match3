using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "StageEventChannel", menuName = "ScriptableObject/EventChannel/Stage")]
public class StageEventChannel : EventChannel
{
    public event UnityAction<int> OnCurrentClearCountUpdated;
    public event UnityAction<int> OnCurrentTryCountUpdated;
    public event UnityAction<BoardItemData> OnBoardItemRemoved;

    public void UpdateCurrentClearCount(int count)
    {
        OnCurrentClearCountUpdated?.Invoke(count);
    }

    public void UpdateCurrentTryCount(int count)
    {
        OnCurrentTryCountUpdated?.Invoke(count);
    }

    public void RemoveBoardItem(BoardItemData data)
    {
        OnBoardItemRemoved?.Invoke(data);
    }
}

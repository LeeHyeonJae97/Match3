using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameEventChannel", menuName = "ScriptableObject/EventChannel/Game")]
public class GameEventChannel : EventChannel
{
    public event UnityAction OnPaused;
    public event UnityAction OnResumed;
    public event UnityAction OnCleared;
    public event UnityAction OnFailed;

    public void Pause()
    {
        OnPaused?.Invoke();
    }

    public void Resume()
    {
        OnResumed?.Invoke();
    }

    public void Clear()
    {
        OnCleared?.Invoke();
    }

    public void Fail()
    {
        OnFailed?.Invoke();
    }
}

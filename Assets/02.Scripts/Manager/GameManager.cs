using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float deltaTime => Time.deltaTime * timeScale;
    public float timeScale { get; set; } = 1f;

    [SerializeField] private GameEventChannel _gameEventChannel;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _gameEventChannel.OnCleared += OnCleared;
        _gameEventChannel.OnFailed += OnFailed;
        _gameEventChannel.OnPaused += OnPaused;
        _gameEventChannel.OnResumed += OnResumed;
    }

    private void OnDestroy()
    {
        _gameEventChannel.OnCleared -= OnCleared;
        _gameEventChannel.OnFailed -= OnFailed;
        _gameEventChannel.OnPaused -= OnPaused;
        _gameEventChannel.OnResumed -= OnResumed;
    }

    private void OnCleared()
    {
        timeScale = 0f;
    }

    private void OnFailed()
    {
        timeScale = 0f;
    }

    private void OnPaused()
    {
        timeScale = 0f;
    }

    private void OnResumed()
    {
        timeScale = 1f;
    }
}

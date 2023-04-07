using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClearCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private Button _quitButton;
    private GameEventChannel _gameEventChannel;

    private void Awake()
    {
        _gameEventChannel = EventChannelStorage.Get<GameEventChannel>();

        _gameEventChannel.OnCleared += OnCleared;
        _quitButton.onClick.AddListener(OnClickQuitButton);

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _gameEventChannel.OnCleared -= OnCleared;
    }

    private void OnCleared()
    {
        _levelText.text = $"Level {Stage.Instance.Data.Level}";

        gameObject.SetActive(true);
    }

    private void OnClickQuitButton()
    {
        SceneManager.LoadScene("Lobby");
    }
}

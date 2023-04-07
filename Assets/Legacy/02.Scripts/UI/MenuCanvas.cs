using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuCanvas : MonoBehaviour
{
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameEventChannel _gameEventChannel;

    private void Awake()
    {
        _menuButton.onClick.AddListener(OnClickMenuButton);
        _quitButton.onClick.AddListener(OnClickQuitButton);
        _closeButton.onClick.AddListener(OnClickCloseButton);

        _menuPanel.SetActive(false);
    }

    private void OnClickMenuButton()
    {
        _gameEventChannel.Pause();

        _menuPanel.SetActive(true);
    }

    private void OnClickQuitButton()
    {
        SceneManager.LoadScene("Lobby");
    }

    private void OnClickCloseButton()
    {
        _gameEventChannel.Resume();

        _menuPanel.SetActive(false);
    }
}

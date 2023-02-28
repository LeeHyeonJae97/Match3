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

    private void Awake()
    {
        _menuButton.onClick.AddListener(OnClickMenuButton);
        _quitButton.onClick.AddListener(OnClickQuitButton);
        _closeButton.onClick.AddListener(OnClickCloseButton);

        _menuPanel.SetActive(false);
    }

    private void OnClickMenuButton()
    {
        _menuPanel.SetActive(true);
    }

    private void OnClickQuitButton()
    {
        SceneManager.LoadScene("Stage");
    }

    private void OnClickCloseButton()
    {
        _menuPanel.SetActive(false);
    }
}

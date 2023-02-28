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
    [SerializeField] private Board _board;

    private void Awake()
    {
        _board.onCleared += OnCleared;
        _quitButton.onClick.AddListener(OnClickQuitButton);

        gameObject.SetActive(false);
    }

    private void OnCleared()
    {
        _levelText.text = $"{_board.Data.Level}";

        gameObject.SetActive(true);
    }

    private void OnClickQuitButton()
    {
        SceneManager.LoadScene("Stage");
    }
}

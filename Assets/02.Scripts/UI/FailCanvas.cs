using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FailCanvas : MonoBehaviour
{
    [SerializeField] private Image _clearedConditionTypeImage;
    [SerializeField] private TextMeshProUGUI _remainClearedCountText;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Board _board;

    private void Awake()
    {
        _board.onFailed += OnFailed;
        _quitButton.onClick.AddListener(OnClickQuitButton);

        gameObject.SetActive(false);
    }

    private void OnFailed()
    {
        _clearedConditionTypeImage.sprite = _board.ClearedCondition.Sprite;
        _remainClearedCountText.text = $"{_board.RemainClearedCount}";

        gameObject.SetActive(true);
    }

    private void OnClickQuitButton()
    {
        SceneManager.LoadScene("Stage");
    }
}

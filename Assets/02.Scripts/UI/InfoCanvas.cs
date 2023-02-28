using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _currentTryCount;
    [SerializeField] private TextMeshProUGUI _currentClearedCount;
    [SerializeField] private Image _clearedConditionTypeImage;
    [SerializeField] private Board _board;

    private void OnEnable()
    {
        // initialize ui
        _levelText.text = $"{_board.Data.Level}";
        _clearedConditionTypeImage.sprite = _board.ClearedCondition.Sprite;

        // add event
        _board.onCurrentClearedCountUpdated += OnCurrentClearedCountUpdated;
        _board.onCurrentTryCountUpdated += OnCurrentTryCountUpdated;
    }

    private void OnDisable()
    {
        _board.onCurrentClearedCountUpdated -= OnCurrentClearedCountUpdated;
        _board.onCurrentTryCountUpdated -= OnCurrentTryCountUpdated;
    }

    private void OnCurrentTryCountUpdated(int value)
    {
        _currentTryCount.text = $"{value}";
    }

    private void OnCurrentClearedCountUpdated(int value)
    {
        _currentClearedCount.text = $"{value}";
    }
}

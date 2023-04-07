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
    [SerializeField] private Image _clearConditionTypeImage;
    private StageEventChannel _stageEventChannel;

    private void Awake()
    {
        _stageEventChannel = EventChannelStorage.Get<StageEventChannel>();

        _levelText.text = $"Level {Stage.Instance.Data.Level}";
        var clearCondition = Stage.Instance.Data.ClearCondition;
        _clearConditionTypeImage.sprite = BoardItemDataStorage.Get(clearCondition.Type, clearCondition.Color).Sprite;
    }

    private void OnEnable()
    {
        _stageEventChannel.OnCurrentClearCountUpdated += OnCurrentClearedCountUpdated;
        _stageEventChannel.OnCurrentTryCountUpdated += OnCurrentTryCountUpdated;
    }

    private void OnDisable()
    {
        _stageEventChannel.OnCurrentClearCountUpdated -= OnCurrentClearedCountUpdated;
        _stageEventChannel.OnCurrentTryCountUpdated -= OnCurrentTryCountUpdated;
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

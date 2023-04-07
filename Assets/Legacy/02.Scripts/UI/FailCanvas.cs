using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FailCanvas : MonoBehaviour
{
    [SerializeField] private Image _clearConditionTypeImage;
    [SerializeField] private TextMeshProUGUI _remainClearedCountText;
    [SerializeField] private Button _quitButton;
    private GameEventChannel _gameEventChannel;

    private void Awake()
    {
        _gameEventChannel = EventChannelStorage.Get<GameEventChannel>();

        _gameEventChannel.OnFailed += OnFailed;
        _quitButton.onClick.AddListener(OnClickQuitButton);

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _gameEventChannel.OnFailed -= OnFailed;
    }

    private void OnFailed()
    {
        var clearCondition = Stage.Instance.Data.ClearCondition;
        _clearConditionTypeImage.sprite = BoardItemDataStorage.Get(clearCondition.Type, clearCondition.Color).Sprite;
        _remainClearedCountText.text = $"{Stage.Instance.RemainClearCount}";

        gameObject.SetActive(true);
    }

    private void OnClickQuitButton()
    {
        SceneManager.LoadScene("Lobby");
    }
}

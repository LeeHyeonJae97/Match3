using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Button _slotButton;
    [SerializeField] private StageDataAnchor _stageDataAnchor;
    private StageData _stageData;

    private void Awake()
    {
        _slotButton.onClick.AddListener(OnClickSlotButton);
    }

    public void Init(StageData stageData)
    {
        if (stageData == null)
        {
            Destroy(gameObject);
            return;
        }

        _stageData = stageData;

        _levelText.text = $"Level {_stageData.Level}";
        _descriptionText.text = $"{_stageData.Description}";
    }

    private void OnClickSlotButton()
    {
        _stageDataAnchor.Value = _stageData;
        SceneManager.LoadScene("Play");
    }
}

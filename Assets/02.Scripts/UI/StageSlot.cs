using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private Button _slotButton;
    [SerializeField] private BoardDataAnchor _boardDataAnchor;
    private BoardData _data;

    private void Awake()
    {
        _slotButton.onClick.AddListener(OnClickSlotButton);
    }

    public void Init(BoardData data)
    {
        if (data == null)
        {
            Destroy(gameObject);
            return;
        }

        _data = data;

        _levelText.text = $"{_data.Level}";
    }

    private void OnClickSlotButton()
    {
        _boardDataAnchor.Data = _data;
        SceneManager.LoadScene("Play");
    }
}

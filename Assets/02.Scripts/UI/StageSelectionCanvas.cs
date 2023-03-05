using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectionCanvas : MonoBehaviour
{
    [SerializeField] private StageSlot _stageSlotPrefab;
    [SerializeField] private Transform _stageSlotHolder;

    private void Awake()
    {
        var data = StageDataStorage.Get();

        for (int i = 0; i < data.Length; i++)
        {
            Instantiate(_stageSlotPrefab, _stageSlotHolder).Init(data[i]);
        }
    }
}

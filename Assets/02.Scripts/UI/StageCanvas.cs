using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageCanvas : MonoBehaviour
{
    [SerializeField] private StageSlot _stageSlotPrefab;
    [SerializeField] private Transform _stageSlotHolder;
    [SerializeField] private BoardData[] _data;

    private void Awake()
    {
        for (int i = 0; i < _data.Length; i++)
        {
            Instantiate(_stageSlotPrefab, _stageSlotHolder).Init(_data[i]);
        }
    }
}

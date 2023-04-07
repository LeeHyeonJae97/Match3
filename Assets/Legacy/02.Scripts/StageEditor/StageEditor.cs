using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class StageEditor : MonoBehaviour
{
#if UNITY_EDITOR
    private readonly KeyCode[] TypeSelectionKeyCodes = {KeyCode.Alpha1, KeyCode.Alpha2 , KeyCode.Alpha3 , KeyCode.Alpha4 , KeyCode.Alpha5
        ,KeyCode.Alpha6,KeyCode.Alpha7,KeyCode.Alpha8,KeyCode.Alpha9,KeyCode.F1,KeyCode.F2,KeyCode.F3,KeyCode.F4,KeyCode.F5,KeyCode.F6,
        KeyCode.F7,KeyCode.F8,KeyCode.F9, KeyCode.F10, KeyCode.F11, KeyCode.F12 };

    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private GameObject _itemDataImagePrefab;
    [SerializeField] private BoardItemData[] _itemDataCandidates;
    [SerializeField] private TMP_InputField _levelInputField;
    [SerializeField] private TMP_InputField _tryCountInputField;
    [SerializeField] private TMP_Dropdown _clearConditionTypeDropdown;
    [SerializeField] private TMP_Dropdown _clearConditionColorDropdown;
    [SerializeField] private TMP_InputField _clearConditionCountInputField;
    [SerializeField] private TMP_InputField _rowInputField;
    [SerializeField] private TMP_InputField _columnInputField;
    [SerializeField] private TMP_InputField _sizeInputField;
    [SerializeField] private TMP_InputField _spacingInputField;
    [SerializeField] private Button _generateButton;
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _loadButton;
    [SerializeField] private Button _fillButton;
    [SerializeField] private Transform _itemHolder;
    [SerializeField] private Transform _itemDataImageHolder;
    [SerializeField] private Transform _cursor;
    [SerializeField] private StageData _stage;
    private int _currentRow;
    private int _currentColumn;

    private void Awake()
    {
        _generateButton.onClick.AddListener(OnClickGenerateButton);
        _saveButton.onClick.AddListener(OnClickSaveButton);
        _loadButton.onClick.AddListener(OnClickLoadButton);
        _fillButton.onClick.AddListener(OnClickFillButton);

        var values = new List<string>();
        foreach (var type in Enum.GetValues(typeof(BoardItemType)))
        {
            values.Add(type.ToString());
        }
        _clearConditionTypeDropdown.AddOptions(values);

        values.Clear();
        foreach (var color in Enum.GetValues(typeof(BoardItemColor)))
        {
            values.Add(color.ToString());
        }
        _clearConditionColorDropdown.AddOptions(values);

        for (int i = 0; i < _itemDataCandidates.Length; i++)
        {
            var image = Instantiate(_itemDataImagePrefab, _itemDataImageHolder);
            image.GetComponent<Image>().sprite = _itemDataCandidates[i].Sprite;
            image.GetComponentInChildren<TextMeshProUGUI>().text = $"{(i + 1 > 9 ? $"F{i - 8}" : i + 1)}";
        }
    }

    private void Start()
    {
        OnClickGenerateButton();
    }

    private void Update()
    {
        MoveCursor();
        UpdateCurrentSlot();

        void MoveCursor()
        {
            var h = Input.GetKeyDown(KeyCode.RightArrow) ? 1 : Input.GetKeyDown(KeyCode.LeftArrow) ? -1 : 0;
            var v = Input.GetKeyDown(KeyCode.DownArrow) ? -1 : Input.GetKeyDown(KeyCode.UpArrow) ? 1 : 0;
            var row = _stage.BoardLayout.Row;
            var column = _stage.BoardLayout.Column;
            var size = _stage.BoardLayout.Size;

            _currentRow = Mathf.Clamp(_currentRow + v, 0, Mathf.Max(row - 1, 0));
            _currentColumn = Mathf.Clamp(_currentColumn + h, 0, Mathf.Max(column - 1, 0));

            _cursor.localScale = Vector2.one * size * 1.2f;
            _cursor.position = _itemHolder.transform.GetChild(_currentRow * column + _currentColumn).position;
        }

        void UpdateCurrentSlot()
        {
            for (int i = 0; i < _itemDataCandidates.Length; i++)
            {
                if (Input.GetKeyDown(TypeSelectionKeyCodes[i]))
                {
                    int index = _currentRow * _stage.BoardLayout.Column + _currentColumn;

                    var item = _itemHolder.transform.GetChild(index).gameObject;
                    var itemData = _itemDataCandidates[i];

                    item.GetComponent<SpriteRenderer>().sprite = itemData.Sprite;

                    _stage.ItemData[index] = itemData;
                }
            }
        }
    }

    private void OnClickGenerateButton()
    {
        if (!int.TryParse(_levelInputField.text, out var level)) level = 0;
        if (!int.TryParse(_tryCountInputField.text, out var tryCount)) tryCount = 0;
        if (!Enum.TryParse<BoardItemType>(_clearConditionTypeDropdown.options[_clearConditionTypeDropdown.value].text, out var clearConditionType)) clearConditionType = BoardItemType.Candy;
        if (!Enum.TryParse<BoardItemColor>(_clearConditionColorDropdown.options[_clearConditionColorDropdown.value].text, out var clearConditionColor)) clearConditionColor = BoardItemColor.Blue;
        if (!int.TryParse(_clearConditionCountInputField.text, out var clearConditionCount)) clearConditionCount = 0;
        if (!int.TryParse(_rowInputField.text, out var row)) row = 0;
        if (!int.TryParse(_columnInputField.text, out var column)) column = 0;
        if (!float.TryParse(_sizeInputField.text, out var size)) size = 0f;
        if (!float.TryParse(_spacingInputField.text, out var spacing)) spacing = 0f;

        if (_stage == null || _stage.Level != level)
        {
            _stage = ScriptableObject.CreateInstance<StageData>();
        }
        _stage.Level = level;
        _stage.TryCount = tryCount;
        _stage.ClearCondition = new StageClearCondition(clearConditionType, clearConditionColor, clearConditionCount);
        _stage.Slots = new BoardSlot[row * 2 * column];
        _stage.ItemData = new BoardItemData[row * column];
        _stage.BoardLayout = new LegBoardLayout(row, column, size, spacing);

        foreach (Transform child in _itemHolder)
        {
            Destroy(child.gameObject);
        }

        var width = size + spacing;
        var height = size + spacing;
        var minX = column / 2 * width * -1 + (column % 2 == 0 ? width / 2 : 0);
        var minY = row / 2 * height * -1 + (row % 2 == 0 ? height / 2 : 0);

        for (int r = 0; r < row * 2; r++)
        {
            for (int c = 0; c < column; c++)
            {
                int index = r * column + c;

                _stage.Slots[index] = new BoardSlot(r, c, new Vector2(minX + c * width, minY + r * height));
            }
        }

        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < column; c++)
            {
                int index = r * column + c;

                var item = Instantiate(_itemPrefab, _itemHolder);

                item.transform.position = _stage.Slots[index].Position;
                item.transform.localScale = Vector2.one * size;
            }
        }
    }

    private void OnClickSaveButton()
    {
        if (!int.TryParse(_levelInputField.text, out var level)) return;

        if (!int.TryParse(_tryCountInputField.text, out var tryCount)) tryCount = 0;
        if (!Enum.TryParse<BoardItemType>(_clearConditionTypeDropdown.options[_clearConditionTypeDropdown.value].text, out var clearConditionType)) clearConditionType = BoardItemType.Candy;
        if (!Enum.TryParse<BoardItemColor>(_clearConditionColorDropdown.options[_clearConditionColorDropdown.value].text, out var clearConditionColor)) clearConditionColor = BoardItemColor.Blue;
        if (!int.TryParse(_clearConditionCountInputField.text, out var clearConditionCount)) clearConditionCount = 0;

        _stage.Level = level;
        _stage.TryCount = tryCount;
        _stage.ClearCondition = new StageClearCondition(clearConditionType, clearConditionColor, clearConditionCount);

        var path = $"Assets/Resources/StageData/Stage {level}.asset";
        var npath = AssetDatabase.GenerateUniqueAssetPath(path);

        if (npath == path)
        {
            AssetDatabase.CreateAsset(_stage, path);
        }
        AssetDatabase.SaveAssets();
    }

    private void OnClickLoadButton()
    {
        if (!int.TryParse(_levelInputField.text, out var level)) return;

        _stage = AssetDatabase.LoadAssetAtPath<StageData>($"Assets/Resources/StageData/Stage {level}.asset");

        if (_stage == null)
        {
            Debug.LogError($"There's no {level} stage's data");
            return;
        }

        EditorUtility.SetDirty(_stage);

        var tryCount = _stage.TryCount;
        var clearConditionType = _stage.ClearCondition.Type;
        var clearConditionColor = _stage.ClearCondition.Color;
        var clearConditionCount = _stage.ClearCondition.Count;
        var row = _stage.BoardLayout.Row;
        var column = _stage.BoardLayout.Column;
        var size = _stage.BoardLayout.Size;
        var spacing = _stage.BoardLayout.Spacing;

        _levelInputField.text = $"{level}";
        _tryCountInputField.text = $"{tryCount}";
        _clearConditionTypeDropdown.value = (int)clearConditionType;
        _clearConditionColorDropdown.value = (int)clearConditionColor;
        _clearConditionCountInputField.text = $"{clearConditionCount}";
        _rowInputField.text = $"{row}";
        _columnInputField.text = $"{column}";
        _sizeInputField.text = $"{size}";
        _spacingInputField.text = $"{spacing}";

        foreach (Transform child in _itemHolder)
        {
            Destroy(child.gameObject);
        }

        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < column; c++)
            {
                int index = r * column + c;

                var item = Instantiate(_itemPrefab, _itemHolder);

                item.transform.position = _stage.Slots[index].Position;
                item.transform.localScale = Vector2.one * size;
                item.GetComponent<SpriteRenderer>().sprite = _stage.ItemData[r * column + c].Sprite;
            }
        }
    }

    private void OnClickFillButton()
    {
        for (int row = 0; row < _stage.BoardLayout.Row; row++)
        {
            for (int column = 0; column < _stage.BoardLayout.Column; column++)
            {
                int index = row * _stage.BoardLayout.Column + column;

                var item = _itemHolder.transform.GetChild(index).gameObject;
                var itemData = _itemDataCandidates[UnityEngine.Random.Range(0, 4)];

                item.GetComponent<SpriteRenderer>().sprite = itemData.Sprite;

                _stage.ItemData[index] = itemData;
            }
        }
    }
#endif
}

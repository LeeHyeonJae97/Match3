using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class BoardEditor : MonoBehaviour
{
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private GameObject _itemDataImagePrefab;
    [SerializeField] private BoardItemData[] _itemDataCandidates;
    [SerializeField] private TMP_InputField _rowInputField;
    [SerializeField] private TMP_InputField _columnInputField;
    [SerializeField] private TMP_InputField _sizeInputField;
    [SerializeField] private TMP_InputField _spacingInputField;
    [SerializeField] private Button _generateButton;
    [SerializeField] private Button _saveButton;
    [SerializeField] private Transform _itemHolder;
    [SerializeField] private Transform _itemDataImageHolder;
    [SerializeField] private Transform _cursor;
    private BoardData _board;
    private int _currentRow;
    private int _currentColumn;

    private void Awake()
    {
        _generateButton.onClick.AddListener(OnClickGenerateButton);
        _saveButton.onClick.AddListener(OnClickSaveButton);

        for (int i = 0; i < _itemDataCandidates.Length; i++)
        {
            var image = Instantiate(_itemDataImagePrefab, _itemDataImageHolder);
            image.GetComponent<Image>().sprite = _itemDataCandidates[i].Sprite;
        }
    }

    private void Update()
    {
        if (_itemHolder.childCount == 0) return;

        MoveCursor();
        UpdateCurrentSlot();

        void MoveCursor()
        {
            int h = Input.GetKeyDown(KeyCode.RightArrow) ? 1 : Input.GetKeyDown(KeyCode.LeftArrow) ? -1 : 0;
            int v = Input.GetKeyDown(KeyCode.DownArrow) ? -1 : Input.GetKeyDown(KeyCode.UpArrow) ? 1 : 0;
            int row = int.Parse(_rowInputField.text);
            int column = int.Parse(_columnInputField.text);
            float size = float.Parse(_sizeInputField.text);

            _currentRow = Mathf.Clamp(_currentRow + v, 0, row - 1);
            _currentColumn = Mathf.Clamp(_currentColumn + h, 0, column - 1);

            _cursor.localScale = Vector2.one * size * 1.2f;
            _cursor.position = _itemHolder.transform.GetChild(_currentRow * column + _currentColumn).position;
        }

        void UpdateCurrentSlot()
        {
            int column = int.Parse(_columnInputField.text);

            for (int i = (int)KeyCode.Alpha1; i < (int)KeyCode.Alpha1 + _itemDataCandidates.Length; i++)
            {
                if (Input.GetKeyDown((KeyCode)i))
                {
                    int index = _currentRow * column + _currentColumn;

                    var item = _itemHolder.transform.GetChild(index).gameObject;
                    var itemData = _itemDataCandidates[i - (int)KeyCode.Alpha1];

                    item.GetComponent<SpriteRenderer>().sprite = itemData.Sprite;

                    _board.ItemData[index] = itemData;
                }
            }
        }
    }

    private void OnClickGenerateButton()
    {
        int row = int.Parse(_rowInputField.text);
        int column = int.Parse(_columnInputField.text);
        float size = float.Parse(_sizeInputField.text);
        float spacing = float.Parse(_spacingInputField.text);

        if (_board == null)
        {
            _board = ScriptableObject.CreateInstance<BoardData>();
        }
        _board.Slots = new BoardSlot[row * 2 * column];
        _board.ItemData = new BoardItemData[row * column];
        _board.Layout = new BoardLayout(row, column, size, spacing);

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

                _board.Slots[index] = new BoardSlot(r, c, new Vector2(minX + c * width, minY + r * height));
            }
        }

        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < column; c++)
            {
                int index = r * column + c;

                var item = Instantiate(_itemPrefab, _itemHolder);

                item.transform.position = _board.Slots[index].Position;
                item.transform.localScale = Vector2.one * size;
            }
        }
    }

    private void OnClickSaveButton()
    {
        var path = AssetDatabase.GenerateUniqueAssetPath($"Assets/04.ScriptableObjects/Board/Board.asset");
        AssetDatabase.CreateAsset(_board, path);
        AssetDatabase.SaveAssets();
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBoard : MonoBehaviour
{
    [SerializeField] private NewItem _itemPrefab;
    [SerializeField] private NewItemStorage _itemStorage;
    [SerializeField] private NewBoardLayout _layout;
    [SerializeField] private NewInputManager _inputManager;
    private NewItem _selected;

    private void Start()
    {
        Initialize();
    }

    private void OnEnable()
    {
        _inputManager.OnTouched += OnTouched;
        _inputManager.OnScrolled += OnScrolled;
        _inputManager.OnReleased += OnReleased;
    }

    private void OnDisable()
    {
        _inputManager.OnTouched -= OnTouched;
        _inputManager.OnScrolled -= OnScrolled;
        _inputManager.OnReleased -= OnReleased;
    }

    private void Initialize()
    {
        if (_itemPrefab == null || _layout == null) return;

        var row = _layout.Row;
        var column = _layout.Column;
        var size = _layout.Size;
        var spacing = _layout.Spacing;

        var width = size + spacing;
        var height = size + spacing;
        var minX = column / 2 * width * -1 + (column % 2 == 0 ? width / 2 : 0);
        var minY = row / 2 * height * -1 + (row % 2 == 0 ? height / 2 : 0);

        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < column; c++)
            {
                var item = Instantiate(_itemPrefab, transform);

                item.transform.position = new Vector2(minX + c * width, minY + r * height);
                item.transform.localScale = Vector2.one * size;

                // DEPRECATED
                item.ItemStorage = _itemStorage;
                item.Layout = _layout;

                _itemStorage.Add(item);
            }
        }
    }


    private void OnTouched(Vector2 position)
    {
        _layout.GetRowColumn(position, out var row, out var column);

        _selected = _itemStorage.GetItem(row, column);
    }

    private void OnScrolled(Vector2 delta)
    {
        if (_selected == null || delta.sqrMagnitude < 1000) return;

        var direction = Mathf.Abs(delta.x) > Mathf.Abs(delta.y) ? new Vector2Int(delta.x > 0 ? 1 : -1, 0) : new Vector2Int(0, delta.y > 0 ? 1 : -1);

        _selected.Swap(direction);
        _selected = null;
    }

    private void OnReleased()
    {
        _selected = null;
    }

    private void OnDrawGizmos()
    {
        HighlightSelectedItem();

        // LOCAL FUNCTION
        void HighlightSelectedItem()
        {
            if (_selected == null) return;

            Gizmos.DrawWireRect(_selected.transform.position, _selected.transform.localScale * 1.2f, Color.red);
        }
    }
}


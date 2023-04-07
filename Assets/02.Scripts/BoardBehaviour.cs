using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardBehaviour : MonoBehaviour
{
    [SerializeField] private Item _itemPrefab;
    [SerializeField] private Board _board;
    [SerializeField] private BoardLayout _layout;
    [SerializeField] private InputManager _inputManager;
    private Item _selected;
    private List<Item> _matched;
    private float _matchCalledTime;

    private void Start()
    {
        Initialize();

        // LOCAL FUNCTION
        void Initialize()
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
                    item.Initialize(this, _board, _layout);

                    _board.Add(item);
                }
            }

            _matched = new List<Item>();
        }
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

    public bool Match()
    {
        if (_matchCalledTime == Time.time) return false;

        _matchCalledTime = Time.time;

        bool matched = Match(_matched);

        if (matched)
        {
            var counts = new int[_layout.Column];

            Refill(counts, _matched);
            Drop(counts);
        }
        Reset();

        return matched;
    }

    private bool Match(List<Item> matched)
    {
        MatchRow();
        MatchColumn();

        return matched.Count > 0;

        // LOCAL FUNCTION
        void MatchRow()
        {
            for (int r = 0; r < _layout.Row; r++)
            {
                var color = NewItemColor.None;
                var score = 1;

                for (int c = 0; c < _layout.Column; c++)
                {
                    var item = _board.GetItem(r, c);

                    if (color != NewItemColor.None && item != null && item.Color == color)
                    {
                        score++;

                        if (c == _layout.Column - 1 && score >= 3)
                        {
                            for (int i = 0; i < score; i++)
                            {
                                var slot = _board.GetSlot(r, c - i);
                                slot.RowScore = Mathf.Max(slot.RowScore, score);

                                matched.Add(_board.GetItem(r, c - i));
                            }
                        }
                    }
                    else
                    {
                        if (score >= 3)
                        {
                            for (int i = 1; i <= score; i++)
                            {
                                var slot = _board.GetSlot(r, c - i);
                                slot.RowScore = Mathf.Max(slot.RowScore, score);

                                matched.Add(_board.GetItem(r, c - i));
                            }
                        }

                        color = item == null ? NewItemColor.None : item.Color;
                        score = 1;
                    }

                    //Debug.Log($"RowMatchable : {c} {row} {color} {item?.Color} {count}");
                }
            }
        }

        // LOCAL FUNCTION
        void MatchColumn()
        {
            for (int c = 0; c < _layout.Column; c++)
            {
                var color = NewItemColor.None;
                var score = 1;

                for (int r = 0; r < _layout.Row; r++)
                {
                    var item = _board.GetItem(r, c);

                    if (color != NewItemColor.None && item != null && item.Color == color)
                    {
                        score++;

                        if (r == _layout.Row - 1 && score >= 3)
                        {
                            for (int i = 0; i < score; i++)
                            {
                                var slot = _board.GetSlot(r - i, c);
                                slot.RowScore = Mathf.Max(slot.RowScore, score);

                                matched.Add(_board.GetItem(r - i, c));
                            }
                        }
                    }
                    else
                    {
                        if (score >= 3)
                        {
                            for (int i = 1; i <= score; i++)
                            {
                                var slot = _board.GetSlot(r - i, c);
                                slot.RowScore = Mathf.Max(slot.RowScore, score);

                                matched.Add(_board.GetItem(r - i, c));
                            }
                        }

                        color = item == null ? NewItemColor.None : item.Color;
                        score = 1;
                    }

                    //Debug.Log($"ColumnMatchable : {column} {r} {color} {item?.Color} {count}");
                }
            }

        }
    }

    private void Refill(int[] counts, List<Item> matched)
    {
        foreach (var item in matched)
        {
            _layout.GetRowColumn(item.transform.position, out var row, out var column);

            var slot = _board.GetSlot(row, column);

            if (slot.Refreshed && ((slot.RowScore >= 3 && slot.ColumnScore >= 3) || slot.RowScore > 3 || slot.ColumnScore > 3))
            {
                item.SetColorSpecial();
            }
            else
            {
                counts[column]++;
                item.transform.position = _layout.GetPosition(_layout.Row - 1 + counts[column], column);
                item.SetColor();
            }
        }
    }

    private void Drop(int[] counts)
    {
        // NOTICE :
        // Need to change Algorithm
        for (int c = 0; c < counts.Length; c++)
        {
            if (counts[c] > 0)
            {
                int count = 0;

                for (int r = 0; r < _layout.Row + counts[c]; r++)
                {
                    var item = _board.GetItem(r, c);

                    if (item == null)
                    {
                        count++;
                    }
                    else if (count > 0)
                    {
                        StartCoroutine(item.CoDrop(count));
                    }
                }
            }
        }
    }

    private void Reset()
    {
        _matched.Clear();
        _board.ResetSlots();
    }

    private void OnTouched(Vector2 position)
    {
        _layout.GetRowColumn(position, out var row, out var column);

        _selected = _board.GetItem(row, column);
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

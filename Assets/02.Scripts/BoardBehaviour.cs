using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            do
            {
                for (int i = 0; i < _matched.Count; i++)
                {
                    _matched[i].SetColor();
                }

                _matched.Clear();

                Match(_matched);
            }
            while (_matched.Count > 0);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _board.Save();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _board.Load();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            _board.Shuffle();
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

    public bool Matchable()
    {
        return MatchRow() || MatchColumn();

        // LOCAL FUNCTION
        bool MatchRow()
        {
            for (int r = 0; r < _layout.Row; r++)
            {
                var color = ItemColor.None;
                var score = 1;

                for (int c = 0; c < _layout.Column; c++)
                {
                    var item = _board.GetItem(r, c);

                    if (color != ItemColor.None && item != null && item.Color == color)
                    {
                        score++;

                        if (c == _layout.Column - 1 && score >= 3)
                        {
                            for (int i = 0; i < score; i++)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (score >= 3)
                        {
                            for (int i = 1; i <= score; i++)
                            {
                                return true;
                            }
                        }

                        color = item == null ? ItemColor.None : item.Color;
                        score = 1;
                    }
                }
            }

            return false;
        }

        // LOCAL FUNCTION
        bool MatchColumn()
        {
            for (int c = 0; c < _layout.Column; c++)
            {
                var color = ItemColor.None;
                var score = 1;

                for (int r = 0; r < _layout.Row; r++)
                {
                    var item = _board.GetItem(r, c);

                    if (color != ItemColor.None && item != null && item.Color == color)
                    {
                        score++;

                        if (r == _layout.Row - 1 && score >= 3)
                        {
                            for (int i = 0; i < score; i++)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (score >= 3)
                        {
                            for (int i = 1; i <= score; i++)
                            {
                                return true;
                            }
                        }

                        color = item == null ? ItemColor.None : item.Color;
                        score = 1;
                    }
                }
            }

            return false;
        }
    }

    public void Match()
    {
        if (!Validate()) return;

        StartCoroutine(CoMatch());

        // LOCAL FUNCTION
        bool Validate()
        {
            if (_matchCalledTime == Time.time) return false;

            _matchCalledTime = Time.time;

            return true;
        }

        // LOCAL FUNCTION
        IEnumerator CoMatch()
        {
            yield return new WaitForEndOfFrame();

            Match(_matched);

            if (_matched.Count > 0)
            {
                yield return StartCoroutine(CoRefill(_matched));
                Drop();
            }
            Reset();
        }
    }

    private void Match(List<Item> matched)
    {
        var matchGroup = 0;

        MatchRow();
        MatchColumn();

        // LOCAL FUNCTION
        void MatchRow()
        {
            for (int r = 0; r < _layout.Row; r++)
            {
                var color = ItemColor.None;
                var score = 1;

                for (int c = 0; c < _layout.Column; c++)
                {
                    var item = _board.GetItem(r, c);

                    if (color != ItemColor.None && item != null && item.Color == color)
                    {
                        score++;

                        if (c == _layout.Column - 1 && score >= 3)
                        {
                            ++matchGroup;

                            for (int i = 0; i < score; i++)
                            {
                                var slot = _board.GetSlot(r, c - i);

                                slot.RowScore = Mathf.Max(slot.RowScore, score);
                                slot.MatchGroup = matchGroup;

                                matched.Add(_board.GetItem(r, c - i));
                            }
                        }
                    }
                    else
                    {
                        if (score >= 3)
                        {
                            ++matchGroup;

                            for (int i = 1; i <= score; i++)
                            {
                                var slot = _board.GetSlot(r, c - i);

                                slot.RowScore = Mathf.Max(slot.RowScore, score);
                                slot.MatchGroup = matchGroup;

                                matched.Add(_board.GetItem(r, c - i));
                            }
                        }

                        color = item == null ? ItemColor.None : item.Color;
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
                var color = ItemColor.None;
                var score = 1;

                for (int r = 0; r < _layout.Row; r++)
                {
                    var item = _board.GetItem(r, c);

                    // to prevent dropping items are matched
                    if (item == null) return;

                    if (color != ItemColor.None && item != null && item.Color == color)
                    {
                        score++;

                        if (r == _layout.Row - 1 && score >= 3)
                        {
                            var mg = 0;

                            for (int i = 0; i < score; i++)
                            {
                                var tmp = _board.GetSlot(r - i, c).MatchGroup;

                                if (tmp > 0)
                                {
                                    mg = tmp;
                                }
                            }
                            if (mg == 0) mg = ++matchGroup;

                            for (int i = 0; i < score; i++)
                            {
                                var slot = _board.GetSlot(r - i, c);

                                slot.ColumnScore = Mathf.Max(slot.ColumnScore, score);
                                slot.MatchGroup = mg;

                                matched.Add(_board.GetItem(r - i, c));
                            }
                        }
                    }
                    else
                    {
                        if (score >= 3)
                        {
                            var mg = 0;

                            for (int i = 0; i < score; i++)
                            {
                                var tmp = _board.GetSlot(r - i, c).MatchGroup;

                                if (tmp > 0)
                                {
                                    mg = tmp;
                                }
                            }
                            if (mg == 0) mg = ++matchGroup;

                            for (int i = 1; i <= score; i++)
                            {
                                var slot = _board.GetSlot(r - i, c);

                                slot.ColumnScore = Mathf.Max(slot.ColumnScore, score);
                                slot.MatchGroup = mg;

                                matched.Add(_board.GetItem(r - i, c));
                            }
                        }

                        color = item == null ? ItemColor.None : item.Color;
                        score = 1;
                    }

                    //Debug.Log($"ColumnMatchable : {column} {r} {color} {item?.Color} {count}");
                }
            }
        }
    }

    IEnumerator CoRefill(List<Item> matched)
    {
        matched = matched.Distinct().ToList();

        matched.Sort((l, r) =>
        {
            _layout.GetRowColumn(l.transform.position, out var lr, out var lc);
            _layout.GetRowColumn(r.transform.position, out var rr, out var rc);

            var ls = _board.GetSlot(lr, lc);
            var rs = _board.GetSlot(rr, rc);

            var comp = ls.MatchGroup.CompareTo(rs.MatchGroup);

            if (comp == 0)
            {
                comp = ls.Refreshed ? -1 : 1;
            }

            return comp;
        });

        var pivot = default(Item);
        var matchGroup = 0;

        var cors = new List<Coroutine>();

        var counts = new int[_layout.Column];

        foreach (var item in matched)
        {
            _layout.GetRowColumn(item.transform.position, out var row, out var column);

            var slot = _board.GetSlot(row, column);

            if (slot.MatchGroup == matchGroup || (slot.RowScore >= 3 && slot.ColumnScore >= 3) || slot.RowScore > 3 || slot.ColumnScore > 3)
            {
                if (slot.Refreshed && slot.MatchGroup != matchGroup)
                {
                    pivot = item;
                    matchGroup = slot.MatchGroup;

                    item.SetColor();
                }
                else
                {
                    counts[column]++;

                    var pivotPosition = pivot.transform.position;
                    var refillPosition = _layout.GetPosition(_layout.Row - 1 + counts[column], column);

                    var cor = StartCoroutine(CoMove(item, pivotPosition, refillPosition));

                    cors.Add(cor);
                }
            }
            else
            {
                counts[column]++;

                item.transform.position = _layout.GetPosition(_layout.Row - 1 + counts[column], column); ;
                item.SetColor();
            }
        }

        foreach (var cor in cors)
        {
            yield return cor;
        }

        // LOCAL FUNCTION
        IEnumerator CoMove(Item item, Vector3 pivot, Vector3 refill)
        {
            yield return StartCoroutine(item.CoMove(pivot, 5));

            item.transform.position = refill;
            item.SetColor();
        }
    }

    private void Drop()
    {
        // NOTICE :
        // Need to change Algorithm
        for (int c = 0; c < _layout.Column; c++)
        {
            int count = 0;

            for (int r = 0; r < _layout.Row + count; r++)
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

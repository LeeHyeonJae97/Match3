using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private static readonly Vector2Int[] DIRECTIONS = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

    private int Row => _data.Layout.Row;
    private int Column => _data.Layout.Column;

    [SerializeField] private BoardItem _itemPrefab;
    [SerializeField] private BoardItemData[] _candyData;
    [SerializeField] private BoardData _data;
    private BoardSlot[,] _slots;
    private Queue<BoardItem> _items = new Queue<BoardItem>();
    private HashSet<(int, int, int)> _matchables = new HashSet<(int, int, int)>();
    private List<BoardSlot> _matched = new List<BoardSlot>();
    private BoardSlot _selected;
    private Vector3 _mouse;

    private void Start()
    {
        Generate();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mouse = Input.mousePosition;
            mouse.z -= Camera.main.transform.position.z;
            var pos = Camera.main.ScreenToWorldPoint(mouse);

            for (int row = 0; row < Row; row++)
            {
                for (int column = 0; column < Column; column++)
                {
                    if (_slots[row, column].Item.Bounds.Contains(pos))
                    {
                        _selected = _slots[row, column];
                        _mouse = Input.mousePosition;

                        return;
                    }
                }
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (_selected != null && (Input.mousePosition - _mouse).sqrMagnitude > 1000)
            {
                var row = _selected.Row;
                var column = _selected.Column;
                var direction = ((Vector2)(Input.mousePosition - _mouse)).Snap4();

                Swap(row, column, direction, true, true);
                Match(_matched);
                Fill(_matched);
                Return(_matched);
                Reset();
                Refill(true);

                Match(_matched);
                Fill(_matched);

                //do
                //{
                //    Match(_matched);
                //    Fill(_matched);
                //    Return(_matched);
                //    Reset();
                //    Refill(true);
                //}
                //while (_matched.Count > 0);

                _selected = null;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _selected = null;
        }
    }

    //private void Update()
    //{
    //    if (!IsSlotStill()) return;

    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        var mouse = Input.mousePosition;
    //        mouse.z -= Camera.main.transform.position.z;
    //        var pos = Camera.main.ScreenToWorldPoint(mouse);

    //        for (int row = 0; row < Row; row++)
    //        {
    //            for (int column = 0; column < Column; column++)
    //            {
    //                if (_slots[row, column].Item.Bounds.Contains(pos))
    //                {
    //                    _selected = _slots[row, column];
    //                    _mouse = Input.mousePosition;

    //                    return;
    //                }
    //            }
    //        }
    //    }
    //    else if (Input.GetMouseButton(0))
    //    {
    //        if (_selected != null && (Input.mousePosition - _mouse).sqrMagnitude > 1000)
    //        {
    //            var row = _selected.Row;
    //            var column = _selected.Column;
    //            var direction = ((Vector2)(Input.mousePosition - _mouse)).Snap4();

    //            StartCoroutine(CoSwap(row, column, direction));
    //            _selected = null;
    //        }
    //    }
    //    else if (Input.GetMouseButtonUp(0))
    //    {
    //        _selected = null;
    //    }
    //}

    private void OnDrawGizmos()
    {
        if (_slots != null)
        {
            foreach (var slot in _slots)
            {
                if (slot != null && slot.Item != null && slot.Refreshed)
                {
                    Gizmos.DrawWireRect(slot.Position, slot.Item.transform.localScale * 1.2f, Color.red);
                }
            }
        }

        if (_matchables != null)
        {
            foreach (var matchable in _matchables)
            {
                var slot = _slots[matchable.Item1, matchable.Item2];

                var direction = DIRECTIONS[matchable.Item3];
                var nextSlot = _slots[matchable.Item1 + direction.y, matchable.Item2 + direction.x];

                Gizmos.DrawLine(slot.Position, nextSlot.Position, Color.magenta);
            }
        }

        if (_matched != null)
        {
            foreach (var slot in _matched)
            {
                Gizmos.DrawWireRect(slot.Position, slot.Item.transform.localScale * 1.2f, Color.cyan);
            }
        }
    }

    //IEnumerator CoSwap(int row, int column, int direction)
    //{
    //    var wait = new WaitUntil(IsSlotStill);

    //    if (!Swap(row, column, direction, true, false)) yield break;

    //    yield return wait;

    //    // if there's no matched item, re-swap
    //    if (!Match(_matched))
    //    {
    //        Swap(row, column, direction, true, false);
    //        yield return wait;
    //        yield break;
    //    }

    //    Remove(_matched);
    //    Refill(false);
    //    yield return wait;

    //    do
    //    {
    //        this.Match(_matched);
    //        Remove(_matched);
    //        Refill(false);
    //        yield return wait;
    //    }
    //    while (_matched.Count > 0);

    //    //while (!Matchable(_matchables))
    //    //{
    //    //    Generate();
    //    //    do
    //    //    {
    //    //        this.Match(_matched);
    //    //        Remove(_matched);
    //    //        Refill(true);
    //    //    }
    //    //    while (_matched.Count > 0);
    //    //}

    //    bool Match(List<BoardSlot> matched)
    //    {
    //        matched.Clear();

    //        switch (direction)
    //        {
    //            // right
    //            case 0:
    //                MatchHorizontally(row, matched);
    //                MatchVertically(column, matched);
    //                MatchVertically(column + 1, matched);
    //                break;

    //            // up
    //            case 1:
    //                MatchHorizontally(row, matched);
    //                MatchHorizontally(row + 1, matched);
    //                MatchVertically(column, matched);
    //                break;

    //            // left
    //            case 2:
    //                MatchHorizontally(row, matched);
    //                MatchVertically(column, matched);
    //                MatchVertically(column - 1, matched);
    //                break;

    //            // down
    //            case 3:
    //                MatchHorizontally(row, matched);
    //                MatchHorizontally(row - 1, matched);
    //                MatchVertically(column, matched);
    //                break;
    //        }

    //        return matched.Count > 0;
    //    }
    //}

    private void Generate()
    {
        if (_slots != null) return;

        _slots = new BoardSlot[Row * 2, Column];

        for (int r = 0; r < Row; r++)
        {
            for (int c = 0; c < Column; c++)
            {
                // copy data's slot
                _slots[r, c] = new BoardSlot(_data.Slots[r * Column + c]);

                // instantiate and initialize item
                var item = Instantiate(_itemPrefab);

                item.transform.localScale = Vector2.one * _data.Layout.Size;
                item.transform.SetParent(transform);
                item.Position = _slots[r, c].Position;
                item.Data = _data.ItemData[r * Column + c];

                _slots[r, c].Item = item;
            }
        }
    }

    //private void Match(List<BoardSlot> matched)
    //{
    //    if (matched == null) return;

    //    matched.Clear();

    //    for (int row = 0; row < Row; row++)
    //    {
    //        MatchHorizontally(row, matched);
    //    }

    //    for (int column = 0; column < Column; column++)
    //    {
    //        MatchVertically(column, matched);
    //    }
    //}

    private void Match(List<BoardSlot> matched)
    {
        Match(0, 0, Row, Column, matched);
    }

    private void Match(int startRow, int startColumn, int endRow, int endColumn, List<BoardSlot> matched)
    {
        for (int row = startRow; row < endRow; row++)
        {
            MatchHorizontally(row, startColumn, endColumn, _matched);
        }

        for (int column = startColumn; column < endColumn; column++)
        {
            MatchVertically(column, startRow, endRow, _matched);
        }
    }

    private void MatchHorizontally(int row, int startColumn, int endColumn, List<BoardSlot> matched)
    {
        if (matched == null) return;

        var matchCount = 1;
        var current = (BoardItemType)(-1);

        BoardSlot pivot = null;

        for (int column = startColumn; column < endColumn; column++)
        {
            if (_slots[row, column].Refreshed && (pivot == null || pivot.Item.Data.Type != current || _slots[row, column].HorizontallyRemovedCount > pivot.HorizontallyRemovedCount || _slots[row, column].VerticallyRemovedCount > pivot.VerticallyRemovedCount))
            {
                pivot = _slots[row, column];
            }

            if (_slots[row, column].Item.Data.Type == current)
            {
                matchCount++;

                if (row < Row - 1 && matchCount >= 2)
                {
                    var slot1 = _slots[row + 1, column - 1];
                    var slot2 = _slots[row + 1, column];

                    if (current == slot1.Item.Data.Type && current == slot2.Item.Data.Type)
                    {
                        if (slot1.Refreshed)
                        {
                            pivot = slot1;
                        }
                        else if (slot2.Refreshed)
                        {
                            pivot = slot2;
                        }

                        Remove(row, column - 1);
                        Remove(row, column);
                        Remove(row + 1, column - 1);
                        Remove(row + 1, column);

                        pivot.HorizontallyRemovedCount = Mathf.Max(pivot.HorizontallyRemovedCount, matchCount);
                        pivot.VerticallyRemovedCount = Mathf.Max(pivot.VerticallyRemovedCount, 2);
                    }
                }

                if (column == endColumn - 1 && matchCount >= 3)
                {
                    RemoveHorizontally(row, column - matchCount + 1, column);

                    pivot.HorizontallyRemovedCount = Mathf.Max(pivot.HorizontallyRemovedCount, matchCount);
                }
            }
            else
            {
                if (matchCount >= 3)
                {
                    RemoveHorizontally(row, column - matchCount, column - 1);

                    pivot.HorizontallyRemovedCount = Mathf.Max(pivot.HorizontallyRemovedCount, matchCount);
                }

                matchCount = 1;
                current = _slots[row, column].Item.Data.Type;
            }
        }
    }

    private void MatchVertically(int column, int startRow, int endRow, List<BoardSlot> matched)
    {
        if (matched == null) return;

        var matchCount = 1;
        var current = (BoardItemType)(-1);

        BoardSlot pivot = null;

        for (int row = startRow; row < endRow; row++)
        {
            if (_slots[row, column].Refreshed && (pivot == null || pivot.Item.Data.Type != current || _slots[row, column].HorizontallyRemovedCount > pivot.HorizontallyRemovedCount || _slots[row, column].VerticallyRemovedCount > pivot.VerticallyRemovedCount))
            {
                pivot = _slots[row, column];
            }

            if (_slots[row, column].Item.Data.Type == current)
            {
                matchCount++;

                if (row == endRow - 1 && matchCount >= 3)
                {
                    RemoveVertically(column, row - matchCount + 1, row);

                    pivot.VerticallyRemovedCount = Mathf.Max(pivot.VerticallyRemovedCount, matchCount);
                }
            }
            else
            {
                if (matchCount >= 3)
                {
                    RemoveVertically(column, row - matchCount, row - 1);

                    pivot.VerticallyRemovedCount = Mathf.Max(pivot.VerticallyRemovedCount, matchCount);
                }

                matchCount = 1;
                current = _slots[row, column].Item.Data.Type;
            }
        }
    }
    
    private void RemoveRow(int row)
    {
        RemoveHorizontally(row, 0, Column);
    }

    private void RemoveColumn(int column)
    {
        RemoveVertically(column, 0, Row);
    }

    private void RemoveHorizontally(int row, int startColumn, int endColumn)
    {
        for (int column = startColumn; column <= endColumn; column++)
        {
            Remove(row, column);
        }
    }

    private void RemoveVertically(int column, int startRow, int endRow)
    {
        for (int row = startRow; row <= endRow; row++)
        {
            Remove(row, column);
        }
    }

    private void Remove(int row, int column)
    {
        _matched.Add(_slots[row, column]);
    }

    private void Fill(List<BoardSlot> matched)
    {
        if (matched == null) return;

        Debug.Log("Fill");

        for (int i = 0; i < matched.Count; i++)
        {
            var h = matched[i].HorizontallyRemovedCount;
            var v = matched[i].VerticallyRemovedCount;

            if (h == 5 || v == 5)
            {
                Debug.Log($"{matched[i]} / {h} / {v} / Rainbow Candy");
            }
            else if (h == 4 || v == 4)
            {
                Debug.Log($"{matched[i]} / {h} / {v} / Striped Candy");
            }
            else if (h == 3 && v == 3)
            {
                Debug.Log($"{matched[i]} / {h} / {v} / Wrapped Candy");
            }
            else if (h >= 2 && v >= 2)
            {
                Debug.Log($"{matched[i]} / {h} / {v} / Ball Candy");
            }
            else if( h== 3 || v == 3)
            {
                Debug.Log($"{matched[i]} / {h} / {v} / Candy");
            }
        }
    }

    private void Return(List<BoardSlot> matched)
    {
        if (matched == null) return;

        for (int i = 0; i < matched.Count; i++)
        {
            var slot = matched[i];

            if (slot.Item != null)
            {
                var item = slot.Item;
                item.Data = null;
                slot.Item = null;
                _items.Enqueue(item);
            }
        }
    }

    private void Reset()
    {
        _matched.Clear();
        _matchables.Clear();

        for (int r = 0; r < Row; r++)
        {
            for (int c = 0; c < Column; c++)
            {
                var slot = _slots[r, c];

                slot.Refreshed = false;
                slot.HorizontallyRemovedCount = 0;
                slot.VerticallyRemovedCount = 0;
            }
        }
    }

    //private void Remove(List<BoardSlot> matched)
    //{
    //    if (matched == null) return;

    //    foreach (var slot in matched)
    //    {
    //        if (slot.Item == null) return;

    //        var item = slot.Item;
    //        item.Data = null;
    //        _items.Enqueue(item);

    //        slot.Item = null;
    //    }
    //}

    private void Refill(bool directly)
    {
        for (int column = 0; column < Column; column++)
        {
            int blank = 0;

            Drop(column, ref blank);
            Refill(column, blank);
        }

        // drop current remain items
        void Drop(int column, ref int blank)
        {
            for (int row = 0; row < Row; row++)
            {
                var slot = _slots[row, column];

                if (slot.Item == null)
                {
                    blank++;
                }
                else if (blank > 0)
                {
                    var item = slot.Item;
                    var targetSlot = _slots[row - blank, column];

                    if (directly)
                    {
                        item.Position = targetSlot.Position;
                    }
                    else
                    {
                        item.TargetPosition = targetSlot.Position;
                        item.Move = true;
                    }

                    targetSlot.Item = slot.Item;
                    targetSlot.Refreshed = true;
                }
            }
        }

        // refill items as needed
        void Refill(int column, int blank)
        {
            for (int i = 0; i < blank; i++)
            {
                var item = _items.Dequeue();
                var slot = _slots[Row - blank + i, column];
                var tmpSlot = _slots[Row + i, column];

                if (directly)
                {
                    item.Position = slot.Position;
                }
                else
                {
                    item.Position = tmpSlot.Position;
                    item.TargetPosition = slot.Position;
                    item.Move = true;
                }
                item.Data = _candyData[Random.Range(0, _candyData.Length)];

                slot.Item = item;
                slot.Refreshed = true;
            }
        }
    }

    private bool Matchable(HashSet<(int, int, int)> matchables)
    {
        matchables.Clear();

        for (int row = 0; row < Row; row++)
        {
            for (int column = 0; column < Column; column++)
            {
                IsMatchableWhenSwapRight(row, column);
                IsMatchableWhenSwapUp(row, column);
            }
        }

        return matchables.Count > 0;

        // check matchable when swap rightward
        void IsMatchableWhenSwapRight(int row, int column)
        {
            if (Swap(row, column, 0, false, true))
            {
                if (IsMatchableHorizontally(row) || IsMatchableVertically(column) || IsMatchableVertically(column + 1))
                {
                    matchables.Add((row, column, 0));
                    matchables.Add((row, column + 1, 2));
                }

                Swap(row, column, 0, false, true);
            }
        }

        // check matchable when swap upward
        void IsMatchableWhenSwapUp(int row, int column)
        {
            if (Swap(row, column, 1, false, true))
            {
                if (IsMatchableHorizontally(row) || IsMatchableHorizontally(row + 1) || IsMatchableVertically(column))
                {
                    matchables.Add((row, column, 1));
                    matchables.Add((row + 1, column, 3));
                }

                Swap(row, column, 1, false, true);
            }
        }

        // check matchable specific row horizontally
        bool IsMatchableHorizontally(int row)
        {
            var matchCount = 1;
            var current = (BoardItemType)(-1);

            for (int column = 0; column < Column; column++)
            {
                if (_slots[row, column].Item.Data.Type == current)
                {
                    matchCount++;

                    if (column == Column - 1 && matchCount >= 3) return true;
                }
                else
                {
                    if (matchCount >= 3) return true;

                    matchCount = 1;
                    current = _slots[row, column].Item.Data.Type;
                }
            }

            return false;
        }

        // check matchable specific column vertically
        bool IsMatchableVertically(int column)
        {
            var matchCount = 1;
            var current = (BoardItemType)(-1);

            for (int row = 0; row < Row; row++)
            {
                if (_slots[row, column].Item.Data.Type == current)
                {
                    matchCount++;

                    if (row == Row - 1 && matchCount >= 3) return true;
                }
                else
                {
                    if (matchCount >= 3) return true;

                    matchCount = 1;
                    current = _slots[row, column].Item.Data.Type;
                }
            }

            return false;
        }
    }

    private bool Swap(int row, int column, int direction, bool position, bool directly)
    {
        // direction : right (0), up (1), left (2), down (3)

        var nrow = row + (direction == 1 ? 1 : direction == 3 ? -1 : 0);
        var ncolumn = column + (direction == 0 ? 1 : direction == 2 ? -1 : 0);

        var swapable = 0 <= nrow && nrow < Row && 0 <= ncolumn && ncolumn < Column;

        if (swapable)
        {
            Swap(row, column, nrow, ncolumn, position, directly);
        }

        return swapable;

        // LOCAL FUNCTION
        void Swap(int row, int column, int nrow, int ncolumn, bool position, bool directly)
        {
            var tmp = _slots[row, column].Item;
            _slots[row, column].Item = _slots[nrow, ncolumn].Item;
            _slots[nrow, ncolumn].Item = tmp;

            if (position)
            {
                var slot = _slots[row, column];
                if (directly)
                {
                    slot.Item.Position = slot.Position;
                }
                else
                {
                    slot.Item.TargetPosition = slot.Position;
                    slot.Item.Move = true;
                }
                slot.Refreshed = true;

                slot = _slots[nrow, ncolumn];
                if (directly)
                {
                    slot.Item.Position = slot.Position;
                }
                else
                {
                    slot.Item.TargetPosition = slot.Position;
                    slot.Item.Move = true;
                }
                slot.Refreshed = true;
            }
        }
    }   

    //private void Score(int count)
    //{
    //    Debug.Log($"{count * 10}");
    //}

    //private bool IsSlotStill()
    //{
    //    for (int row = 0; row <Row; row++)
    //    {
    //        for (int column = 0; column < Column; column++)
    //        {
    //            if (_slots[row, column].Item.Move) return false;
    //        }
    //    }
    //    return true;
    //}
}

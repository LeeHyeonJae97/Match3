using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Board : MonoBehaviour
{
    private static readonly Vector2Int[] DIRECTIONS = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

    public BoardItemData ClearedCondition => _itemData[(int)Data.ClearCondition.Type];
    public BoardData Data => _dataAnchor.Data;
    private int Row => Data.Layout.Row;
    private int Column => Data.Layout.Column;
    private int RemainTryCount
    {
        get { return _remainTryCount; }

        set
        {
            _remainTryCount = value;
            onCurrentTryCountUpdated?.Invoke(_remainTryCount);
        }
    }
    public int RemainClearedCount
    {
        get { return _remainClearedCount; }

        set
        {
            _remainClearedCount = value;
            onCurrentClearedCountUpdated?.Invoke(_remainClearedCount);
        }
    }
    public List<BoardSlot> Matched { get; set; } = new List<BoardSlot>();

    [SerializeField] private BoardItem _itemPrefab;
    [SerializeField] private BoardItemData[] _itemData;
    [SerializeField] private BoardDataAnchor _dataAnchor;
    private BoardSlot[,] _slots;
    private Queue<BoardItem> _items = new Queue<BoardItem>();
    private HashSet<(int, int, int)> _matchables = new HashSet<(int, int, int)>();    
    private BoardSlot _selected;
    private Vector3 _mouse;
    private bool _locked;
    private int _remainTryCount;
    private int _remainClearedCount;
    private WaitForSeconds _delay = new WaitForSeconds(0.3f);

    public event UnityAction<int> onCurrentClearedCountUpdated;
    public event UnityAction<int> onCurrentTryCountUpdated;    
    public event UnityAction onCleared;
    public event UnityAction onFailed;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // sort item data by type (enum)
        var list = new List<BoardItemData>(_itemData);
        list.Sort((l, r) => l.Type.CompareTo(r.Type));
        _itemData = list.ToArray();
    }
#endif

    private void Awake()
    {
        onCleared += OnCleared;
        onFailed += OnFailed;
    }

    private void Start()
    {
        RemainTryCount = Data.TryCount;
        RemainClearedCount = Data.ClearCondition.Count;

        Generate();
    }

    private void Update()
    {
        if (_locked) return;

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
                _selected.OnSwiped(this);
                _selected = null;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _selected = null;
        }
    }

    public void Swap()
    {
        StartCoroutine(CoSwap());

        IEnumerator CoSwap()
        {
            _locked = true;

            var row = _selected.Row;
            var column = _selected.Column;
            var direction = ((Vector2)(Input.mousePosition - _mouse)).Snap4();

            if (Swap(row, column, direction, true, true))
            {
                RemainTryCount--;

                Match(Matched);
                if (Matched.Count == 0)
                {
                    yield return _delay;
                    Swap(row, column, direction, true, true);
                }
                else
                {
                    while (Matched.Count > 0)
                    {
                        Fill(Matched);
                        Return(Matched);
                        yield return _delay;
                        Reset();
                        Refill(true);
                        yield return _delay;
                        Match(Matched);
                    }
                }
                Reset();
            }

            _locked = false;

            if (RemainTryCount <= 0)
            {
                onFailed?.Invoke();
            }
        }
    }

    public void RemoveTargets()
    {
        StartCoroutine(CoRemoveTargets());

        IEnumerator CoRemoveTargets()
        {
            _locked = true;

            RemainTryCount--;

            var row = _selected.Row;
            var column = _selected.Column;
            var direction = ((Vector2)(Input.mousePosition - _mouse)).Snap4();
            var nrow = row + (direction == 1 ? 1 : direction == 3 ? -1 : 0);
            var ncolumn = column + (direction == 0 ? 1 : direction == 2 ? -1 : 0);

            var swapable = 0 <= nrow && nrow < Row && 0 <= ncolumn && ncolumn < Column;

            if (swapable)
            {
                this.Remove((BoardItemType)(-1), row, column, 0, 0, 0);
                Remove(_slots[nrow, ncolumn].Item.Data.Type);
                Return(Matched);
                Reset();
                yield return _delay;
                Refill(true);

                yield return _delay;

                MatchAfterRefill();
            }

            _locked = false;

            if (RemainTryCount <= 0)
            {
                onFailed?.Invoke();
            }
        }

        void Remove(BoardItemType type)
        {
            for (int r = 0; r < Row; r++)
            {
                for (int c = 0; c < Column; c++)
                {
                    var slot = _slots[r, c];

                    if (slot.Item.Data.Type == type)
                    {
                        this.Remove((BoardItemType)(-1), r, c, 0, 0, 0);
                    }
                }
            }
        }
    }

    public void Roll()
    {
        StartCoroutine(CoRoll());

        IEnumerator CoRoll()
        {
            _locked = true;

            RemainTryCount--;

            var row = _selected.Row;
            var column = _selected.Column;
            var direction = ((Vector2)(Input.mousePosition - _mouse)).Snap4();
            var groupIndex = -1;

            switch (direction)
            {
                // right
                case 0:
                    RemoveHorizontally(_selected.Item.Data.Type, row, column, Column - 1, 0, 0, ref groupIndex);
                    break;

                // up
                case 1:
                    RemoveVertically(_selected.Item.Data.Type, column, row, Row - 1, 0, 0, ref groupIndex);
                    break;

                // left
                case 2:
                    RemoveHorizontally(_selected.Item.Data.Type, row, 0, column, 0, 0, ref groupIndex);
                    break;

                // down
                case 3:
                    RemoveVertically(_selected.Item.Data.Type, column, 0, row, 0, 0, ref groupIndex);
                    break;
            }
            Return(Matched);
            Reset();
            yield return _delay;
            Refill(true);

            yield return _delay;

            MatchAfterRefill();

            _locked = false;

            if (RemainTryCount <= 0)
            {
                onFailed?.Invoke();
            }
        }
    }

    public void RemoveRow(BoardSlot slot)
    {
        var row = slot.Row;
        var groupIndex = -1;

        RemoveHorizontally(slot.Item.Data.Type, row, 0, Column - 1, 0, 0, ref groupIndex);
    }

    public void RemoveColumn(BoardSlot slot)
    {
        var column = slot.Column;
        var groupIndex = -1;

        RemoveVertically(slot.Item.Data.Type, column, 0, Row - 1, 0, 0, ref groupIndex);
    }

    public void RemoveCross(BoardSlot slot)
    {
        var row = slot.Row;
        var column = slot.Column;
        int num = 2;

        for (int r = row - num; r <= row + num; r++)
        {
            int abs = num - Mathf.Abs(r - row);

            for (int c = column - abs; c <= column + abs; c++)
            {
                if (0 <= r && r < Row && 0 <= c && c < Column)
                {
                    Remove(slot.Item.Data.Type, r, c, 0, 0, 0);
                }
            }
        }
    }

    private void MatchAfterRefill()
    {
        StartCoroutine(CoMatchAfterRefill());

        IEnumerator CoMatchAfterRefill()
        {
            _locked = true;

            Match(Matched);
            while (Matched.Count > 0)
            {
                Fill(Matched);
                Return(Matched);
                yield return _delay;
                Reset();
                Refill(true);
                yield return _delay;
                Match(Matched);
            }
            Reset();

            _locked = false;
        }
    }

    private void Generate()
    {
        if (_slots != null) return;

        _slots = new BoardSlot[Row * 2, Column];

        for (int r = 0; r < Row; r++)
        {
            for (int c = 0; c < Column; c++)
            {
                // copy data's slot
                _slots[r, c] = new BoardSlot(Data.Slots[r * Column + c]);

                // instantiate and initialize item
                var item = Instantiate(_itemPrefab);

                item.transform.localScale = Vector2.one * Data.Layout.Size;
                item.transform.SetParent(transform);
                item.Position = _slots[r, c].Position;
                item.Data = Data.ItemData[r * Column + c];

                _slots[r, c].Item = item;
            }
        }
    }

    private void Match(List<BoardSlot> matched)
    {
        int groupIndex = 0;

        Match(0, 0, Row, Column, matched, ref groupIndex);
    }

    private void Match(int startRow, int startColumn, int endRow, int endColumn, List<BoardSlot> matched, ref int groupIndex)
    {
        for (int row = startRow; row < endRow; row++)
        {
            MatchHorizontally(row, startColumn, endColumn, matched, ref groupIndex);
        }

        for (int column = startColumn; column < endColumn; column++)
        {
            MatchVertically(column, startRow, endRow, matched, ref groupIndex);
        }
    }

    private void MatchHorizontally(int row, int startColumn, int endColumn, List<BoardSlot> matched, ref int groupIndex)
    {
        if (matched == null) return;

        var matchCount = 1;
        var current = -1;

        for (int column = startColumn; column < endColumn; column++)
        {
            int type = (int)_slots[row, column].Item.Data.Type;

            if (type / 4 <= 2 && current / 4 <= 2 && type % 4 == current % 4)
            {
                matchCount++;

                if (row < Row - 1 && matchCount >= 2)
                {
                    var slot1 = _slots[row + 1, column - 1];
                    var slot2 = _slots[row + 1, column];

                    if (current == (int)slot1.Item.Data.Type % 4 && current == (int)slot2.Item.Data.Type % 4)
                    {
                        Remove((BoardItemType)(-1), row, column - 1, row + 1, column, matchCount, 2, ref groupIndex);
                    }
                }

                if (column == endColumn - 1 && matchCount >= 3)
                {
                    RemoveHorizontally((BoardItemType)(-1), row, column - matchCount + 1, column, matchCount, 0, ref groupIndex);
                }
            }
            else
            {
                if (matchCount >= 3)
                {
                    RemoveHorizontally((BoardItemType)(-1), row, column - matchCount, column - 1, matchCount, 0, ref groupIndex);
                }

                matchCount = 1;
                current = type;
            }
        }
    }

    private void MatchVertically(int column, int startRow, int endRow, List<BoardSlot> matched, ref int groupIndex)
    {
        if (matched == null) return;

        var matchCount = 1;
        var current = -1;

        for (int row = startRow; row < endRow; row++)
        {
            int type = (int)_slots[row, column].Item.Data.Type;

            if (type / 4 <= 2 && current / 4 <= 2 && type % 4 == current % 4)
            {
                matchCount++;

                if (row == endRow - 1 && matchCount >= 3)
                {
                    RemoveVertically((BoardItemType)(-1), column, row - matchCount + 1, row, 0, matchCount, ref groupIndex);
                }
            }
            else
            {
                if (matchCount >= 3)
                {
                    RemoveVertically((BoardItemType)(-1), column, row - matchCount, row - 1, 0, matchCount, ref groupIndex);
                }

                matchCount = 1;
                current = type;
            }
        }
    }

    private void RemoveHorizontally(BoardItemType destroyer, int row, int startColumn, int endColumn, int horizontallyRemovedCount, int verticallyRemovedCount, ref int nextGroupIndex)
    {
        Remove(destroyer, row, startColumn, row, endColumn, horizontallyRemovedCount, verticallyRemovedCount, ref nextGroupIndex);
    }

    private void RemoveVertically(BoardItemType destroyer, int column, int startRow, int endRow, int horizontallyRemovedCount, int verticallyRemovedCount, ref int nextGroupIndex)
    {
        Remove(destroyer, startRow, column, endRow, column, horizontallyRemovedCount, verticallyRemovedCount, ref nextGroupIndex);
    }

    private void Remove(BoardItemType destroyer, int startRow, int startColumn, int endRow, int endColumn, int horizontallyRemovedCount, int verticallyRemovedCount, ref int nextGroupIndex)
    {
        int groupIndex = 0;

        for (int row = startRow; row <= endRow; row++)
        {
            for (int column = startColumn; column <= endColumn; column++)
            {
                if (_slots[row, column].GroupIndex > 0)
                {
                    groupIndex = _slots[row, column].GroupIndex;
                }
            }
        }

        if (groupIndex == 0) groupIndex = ++nextGroupIndex;

        for (int row = startRow; row <= endRow; row++)
        {
            for (int column = startColumn; column <= endColumn; column++)
            {
                Remove(destroyer, row, column, horizontallyRemovedCount, verticallyRemovedCount, groupIndex);
            }
        }
    }

    private void Remove(BoardItemType destroyer, int row, int column, int horizontallyRemovedCount, int verticallyRemovedCount, int groupIndex)
    {
        var slot = _slots[row, column];

        slot.HorizontallyRemovedCount = Mathf.Max(slot.HorizontallyRemovedCount, horizontallyRemovedCount);
        slot.VerticallyRemovedCount = Mathf.Max(slot.VerticallyRemovedCount, verticallyRemovedCount);
        slot.GroupIndex = Mathf.Max(slot.GroupIndex, groupIndex);

        if (!Matched.Contains(slot))
        {
            slot.OnDestroyed(this, destroyer);
        }
    }

    private void Fill(List<BoardSlot> matched)
    {
        if (matched == null) return;

        //Debug.Log("Fill");

        int max = 0;
        int groupIndex = 1;
        BoardSlot pivot = null;

        matched.Sort((l, r) => l.GroupIndex.CompareTo(r.GroupIndex));

        for (int i = 0; i < matched.Count; i++)
        {
            if (matched[i].GroupIndex == 0) continue;

            //Debug.Log($"{matched[i]}/{matched[i].HorizontallyRemovedCount}/{matched[i].VerticallyRemovedCount}");

            if (groupIndex != matched[i].GroupIndex)
            {
                Fill();

                max = 0;
                groupIndex = matched[i].GroupIndex;
            }

            if (matched[i].Refreshed)
            {
                int hv = matched[i].HorizontallyRemovedCount + matched[i].VerticallyRemovedCount;

                if (hv > max)
                {
                    max = hv;
                    pivot = matched[i];
                }
            }
        }

        Fill();

        void Fill()
        {
            if (pivot == null)
            {
                Debug.Log("ERROR!!!!!!!!!!");
                return;
            }

            var h = pivot.HorizontallyRemovedCount;
            var v = pivot.VerticallyRemovedCount;

            if (h == 5 || v == 5)
            {
                pivot.Item.Data = _itemData[(int)BoardItemType.RainbowCandy];
                matched.Remove(pivot);
            }
            else if (h >= 3 && v >= 3)
            {
                pivot.Item.Data = _itemData[4 * 2 + (int)pivot.Item.Data.Type % 4];
                matched.Remove(pivot);
            }
            else if (h == 4 || v == 4)
            {
                pivot.Item.Data = _itemData[4 * 1 + (int)pivot.Item.Data.Type % 4];
                matched.Remove(pivot);
            }
            else if (h >= 2 && v >= 2)
            {
                pivot.Item.Data = _itemData[(int)BoardItemType.Ball];
                matched.Remove(pivot);
            }
        }
    }

    private void Return(List<BoardSlot> matched)
    {
        if (matched == null) return;

        for (int i = 0; i < matched.Count; i++)
        {
            var slot = matched[i];
            
            if (slot.Item != null && slot.Item.Data.Type != BoardItemType.Hall)
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
        Matched.Clear();
        _matchables.Clear();

        for (int r = 0; r < Row; r++)
        {
            for (int c = 0; c < Column; c++)
            {
                _slots[r, c].Reset();
            }
        }
    }

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
                item.Data = _itemData[Random.Range((int)BoardItemType.CandyBlue, (int)BoardItemType.CandyYellow + 1)];

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

    public void Clear(BoardItemType type)
    {
        if (type == Data.ClearCondition.Type || ((int)type / 4 <= 2 && (int)Data.ClearCondition.Type / 4 <= 2 && (int)type % 4 == (int)Data.ClearCondition.Type % 4))
        {
            if (RemainClearedCount > 0)
            {
                RemainClearedCount--;

                if (RemainClearedCount == 0)
                {
                    onCleared?.Invoke();
                }
            }
        }
    }

    private void OnCleared()
    {
        enabled = false;
    }

    private void OnFailed()
    {
        enabled = false;
    }
}

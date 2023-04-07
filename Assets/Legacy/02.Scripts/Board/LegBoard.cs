using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegBoard : MonoBehaviour
{
    public int Row => _boardData.Row;
    public int Column => _boardData.Column;
    public BoardSlot[,] Slots => _boardData.Slots;
    public List<BoardSlot> Matched { get; set; } = new List<BoardSlot>(); // matched slots

    [SerializeField] private BoardData _boardData;
    private Queue<BoardItem> _items = new Queue<BoardItem>();             // item pool
    private BoardSlot _selected;                                          // selected slot by touch input
    private StageEventChannel _stageEventChannel;
    //private HashSet<(int, int, int)> _matchables = new HashSet<(int, int, int)>();

    private void Awake()
    {
        _stageEventChannel = EventChannelStorage.Get<StageEventChannel>();
    }

    private void OnEnable()
    {
        LegInputManager.Instance.OnTouched += OnTouched;
        LegInputManager.Instance.OnScrolled += OnScrolled;
        LegInputManager.Instance.OnReleased += OnReleased;
    }

    private void OnDisable()
    {
        LegInputManager.Instance.OnTouched -= OnTouched;
        LegInputManager.Instance.OnScrolled -= OnScrolled;
        LegInputManager.Instance.OnReleased -= OnReleased;
    }

    /// <summary>
    /// initialize board : set position, scale, instantiate items for slots and set using data
    /// </summary>
    /// <param name="data"></param>
    public void Initialize(StageData data)
    {
        transform.position = Vector3.zero;
        transform.localScale = new Vector3(data.BoardLayout.Size * data.BoardLayout.Column + data.BoardLayout.Spacing * (data.BoardLayout.Column - 1), data.BoardLayout.Size * data.BoardLayout.Row + data.BoardLayout.Spacing * (data.BoardLayout.Row - 1));

        _boardData.Initialize(data);
    }

    /// <summary>
    /// find all matchable slots
    /// </summary>
    public void Match()
    {
        Match(0, 0, Row, Column);
    }

    /// <summary>
    /// find all matchable slots
    /// </summary>
    /// <param name="startRow"></param>
    /// <param name="startColumn"></param>
    /// <param name="endRow"></param>
    /// <param name="endColumn"></param>
    public void Match(int startRow, int startColumn, int endRow, int endColumn)
    {
        // use groupIndex to know whether items should be removed together or not
        int groupIndex = 0;

        for (int row = startRow; row < endRow; row++)
        {
            MatchHorizontally(row, startColumn, endColumn, Matched, ref groupIndex);
        }

        for (int column = startColumn; column < endColumn; column++)
        {
            MatchVertically(column, startRow, endRow, Matched, ref groupIndex);
        }

        // LOCAL FUNCTION
        void MatchHorizontally(int row, int startColumn, int endColumn, List<BoardSlot> matched, ref int groupIndex)
        {
            if (matched == null) return;

            var matchCount = 1;
            var current = default(BoardItemData);

            for (int column = startColumn; column < endColumn; column++)
            {
                var data = Slots[row, column].Item == null ? null : Slots[row, column].Item.Data;

                if (data != null && current != null && current.IsSameCandy(data))
                {
                    matchCount++;

                    if (row < Row - 1 && matchCount >= 2)
                    {
                        var data1 = Slots[row + 1, column - 1].Item == null ? null : Slots[row + 1, column - 1].Item.Data;
                        var data2 = Slots[row + 1, column].Item == null ? null : Slots[row + 1, column].Item.Data;

                        if (data1 != null && data2 != null && current.IsSameCandy(data1) && current.IsSameCandy(data2))
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
                    current = data;
                }
            }
        }

        // LOCAL FUNCTION
        void MatchVertically(int column, int startRow, int endRow, List<BoardSlot> matched, ref int groupIndex)
        {
            if (matched == null) return;

            var matchCount = 1;
            var current = default(BoardItemData);

            for (int row = startRow; row < endRow; row++)
            {
                var data = Slots[row, column].Item == null ? null : Slots[row, column].Item.Data;

                if (data != null && current != null && current.IsSameCandy(data))
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
                    current = data;
                }
            }
        }
    }

    /// <summary>
    /// after match by swipe or other specail items and refill, keep match and refill until there's no more items that are matchable
    /// </summary>
    /// <returns></returns>
    public IEnumerator CoMatchAfterRefill()
    {
        Match();
        while (Matched.Count > 0)
        {
            FillSpecial();
            Return();
            Reset();
            yield return StartCoroutine(CoRefill(false));
            Match();
        }
        Reset();
    }

    /// <summary>
    /// remove specific row's from specific column to specific column items
    /// </summary>
    /// <param name="destroyer"></param>
    /// <param name="row"></param>
    /// <param name="startColumn"></param>
    /// <param name="endColumn"></param>
    /// <param name="horizontallyRemovedCount"></param>
    /// <param name="verticallyRemovedCount"></param>
    /// <param name="nextGroupIndex"></param>
    public void RemoveHorizontally(BoardItemType destroyer, int row, int startColumn, int endColumn, int horizontallyRemovedCount, int verticallyRemovedCount, ref int nextGroupIndex)
    {
        Remove(destroyer, row, startColumn, row, endColumn, horizontallyRemovedCount, verticallyRemovedCount, ref nextGroupIndex);
    }

    /// <summary>
    /// remove specific column's from specific row to specific row items
    /// </summary>
    /// <param name="destroyer"></param>
    /// <param name="column"></param>
    /// <param name="startRow"></param>
    /// <param name="endRow"></param>
    /// <param name="horizontallyRemovedCount"></param>
    /// <param name="verticallyRemovedCount"></param>
    /// <param name="nextGroupIndex"></param>
    public void RemoveVertically(BoardItemType destroyer, int column, int startRow, int endRow, int horizontallyRemovedCount, int verticallyRemovedCount, ref int nextGroupIndex)
    {
        Remove(destroyer, startRow, column, endRow, column, horizontallyRemovedCount, verticallyRemovedCount, ref nextGroupIndex);
    }

    /// <summary>
    /// remove from specific row to specific row and from specific column to specific column items
    /// </summary>
    /// <param name="destroyer"></param>
    /// <param name="startRow"></param>
    /// <param name="startColumn"></param>
    /// <param name="endRow"></param>
    /// <param name="endColumn"></param>
    /// <param name="horizontallyRemovedCount"></param>
    /// <param name="verticallyRemovedCount"></param>
    /// <param name="nextGroupIndex"></param>
    private void Remove(BoardItemType destroyer, int startRow, int startColumn, int endRow, int endColumn, int horizontallyRemovedCount, int verticallyRemovedCount, ref int nextGroupIndex)
    {
        int groupIndex = 0;

        for (int row = startRow; row <= endRow; row++)
        {
            for (int column = startColumn; column <= endColumn; column++)
            {
                if (Slots[row, column].GroupIndex > 0)
                {
                    groupIndex = Slots[row, column].GroupIndex;
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

    /// <summary>
    /// remove specific position's item
    /// </summary>
    /// <param name="destroyer"></param>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <param name="horizontallyRemovedCount"></param>
    /// <param name="verticallyRemovedCount"></param>
    /// <param name="groupIndex"></param>
    public void Remove(BoardItemType destroyer, int row, int column, int horizontallyRemovedCount, int verticallyRemovedCount, int groupIndex)
    {
        var slot = Slots[row, column];

        if (slot.Removed) return;

        slot.HorizontallyRemovedCount = Mathf.Max(slot.HorizontallyRemovedCount, horizontallyRemovedCount);
        slot.VerticallyRemovedCount = Mathf.Max(slot.VerticallyRemovedCount, verticallyRemovedCount);
        slot.GroupIndex = Mathf.Max(slot.GroupIndex, groupIndex);

        if (!Matched.Contains(slot))
        {
            slot.OnRemoved(this, destroyer);
        }
    }

    /// <summary>
    /// check if need special items and fill it needed position's slot
    /// </summary>
    public void FillSpecial()
    {
        if (Matched == null) return;

        int max = 0;
        int groupIndex = 1;
        BoardSlot pivot = null;

        Matched.Sort((l, r) => l.GroupIndex.CompareTo(r.GroupIndex));

        for (int i = 0; i < Matched.Count; i++)
        {
            if (Matched[i].GroupIndex == 0) continue;

            if (groupIndex != Matched[i].GroupIndex)
            {
                Fill();

                max = 0;
                groupIndex = Matched[i].GroupIndex;
            }

            if (Matched[i].Refreshed)
            {
                int hv = Matched[i].HorizontallyRemovedCount + Matched[i].VerticallyRemovedCount;

                if (hv > max)
                {
                    max = hv;
                    pivot = Matched[i];
                }
            }
        }

        Fill();

        // LOCAL FUNCTION
        void Fill()
        {
            // if removed by special item, don't create any special item
            if (pivot == null) return;

            var h = pivot.HorizontallyRemovedCount;
            var v = pivot.VerticallyRemovedCount;

            if (h == 5 || v == 5)
            {
                pivot.Item.Data = BoardItemDataStorage.Get(BoardItemType.RainbowCandy, BoardItemColor.None);
                Matched.Remove(pivot);
            }
            else if (h >= 3 && v >= 3)
            {
                pivot.Item.Data = BoardItemDataStorage.Get(BoardItemType.WrappedCandy, (pivot.Item.Data as BoardItemDataCandy).Color);
                Matched.Remove(pivot);
            }
            else if (h == 4)
            {
                pivot.Item.Data = BoardItemDataStorage.Get(BoardItemType.StripedVCandy, (pivot.Item.Data as BoardItemDataCandy).Color);
                Matched.Remove(pivot);
            }
            else if (v == 4)
            {
                pivot.Item.Data = BoardItemDataStorage.Get(BoardItemType.StripedHCandy, (pivot.Item.Data as BoardItemDataCandy).Color);
                Matched.Remove(pivot);
            }
            else if (h >= 2 && v >= 2)
            {
                pivot.Item.Data = BoardItemDataStorage.Get(BoardItemType.Ball, BoardItemColor.None);
                Matched.Remove(pivot);
            }
        }
    }

    /// <summary>
    /// return all removing items in the pool
    /// </summary>
    public void Return()
    {
        if (Matched == null) return;

        for (int i = 0; i < Matched.Count; i++)
        {
            var slot = Matched[i];

            if (slot.Item != null)
            {
                var item = slot.Item;
                item.Data = null;
                slot.Item = null;
                _items.Enqueue(item);
            }
        }
    }

    /// <summary>
    /// reset board's and all the board's slots' state
    /// </summary>
    public void Reset()
    {
        Matched.Clear();
        //_matchables.Clear();

        for (int r = 0; r < Row; r++)
        {
            for (int c = 0; c < Column; c++)
            {
                Slots[r, c].Reset();
            }
        }
    }

    /// <summary>
    /// drop all the floating items and refill new items
    /// </summary>
    /// <param name="directly"></param>
    /// <returns></returns>
    public IEnumerator CoRefill(bool directly)
    {
        // wait for all items are still
        yield return new WaitUntil(IsBoardStill);

        for (int column = 0; column < Column; column++)
        {
            int blank = 0;

            Drop(column, ref blank);
            Refill(column, blank);
        }

        // wait for all items are dropped
        yield return new WaitUntil(IsBoardStill);

        // LOCAL FUNCTION
        void Drop(int column, ref int blank)
        {
            for (int row = 0; row < Row; row++)
            {
                var slot = Slots[row, column];

                if (slot.Item == null)
                {
                    blank++;
                }
                else if (blank > 0)
                {
                    var item = slot.Item;
                    var targetSlot = Slots[row - blank, column];

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

        // LOCAL FUNCTION
        void Refill(int column, int blank)
        {
            for (int i = 0; i < blank; i++)
            {
                var item = _items.Dequeue();
                var slot = Slots[Row - blank + i, column];
                var tmpSlot = Slots[Row + i, column];

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
                item.Data = BoardItemDataStorage.Get(BoardItemType.Candy, (BoardItemColor)Random.Range(1, System.Enum.GetValues(typeof(BoardItemColor)).Length));

                slot.Item = item;
                slot.Refreshed = true;
            }
        }
    }

    /// <summary>
    /// swap adjacent slot's items
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <param name="direction"></param>
    /// <param name="position"></param>
    /// <param name="directly"></param>
    /// <returns></returns>
    public bool Swap(int row, int column, int direction, bool position, bool directly)
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
            var tmp = Slots[row, column].Item;
            Slots[row, column].Item = Slots[nrow, ncolumn].Item;
            Slots[nrow, ncolumn].Item = tmp;

            if (position)
            {
                var slot = Slots[row, column];
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

                slot = Slots[nrow, ncolumn];
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

    /// <summary>
    /// check stage's clear condition
    /// </summary>
    /// <param name="data"></param>
    public void Clear(BoardItemData data)
    {
        _stageEventChannel.RemoveBoardItem(data);
    }

    /// <summary>
    /// check all the item's are still
    /// </summary>
    /// <returns></returns>
    public bool IsBoardStill()
    {
        for (int r = 0; r < Row; r++)
        {
            for (int c = 0; c < Column; c++)
            {
                if (Slots[r, c].Item != null && (Slots[r, c].Item.Move ||
                    (Slots[r, c].Position - Slots[r, c].Item.Position).sqrMagnitude > 0.1f))
                {
                    return false;
                }
            }
        }
        return true;
    }

    // called when screen touched
    private void OnTouched(Vector2 position)
    {
        for (int row = 0; row < Row; row++)
        {
            for (int column = 0; column < Column; column++)
            {
                if (Slots[row, column].Item.Bounds.Contains(position))
                {
                    _selected = Slots[row, column];
                }
            }
        }
    }

    // called when screen scrolled
    private void OnScrolled(Vector2 delta)
    {
        if (_selected != null && delta.sqrMagnitude > 1000)
        {
            _selected.OnSwiped(this, delta.Snap4());
            _selected = null;
        }
    }

    // called when input released
    private void OnReleased()
    {
        _selected = null;
    }

    //public bool Matchable()
    //{
    //    _matchables.Clear();

    //    for (int row = 0; row < Row; row++)
    //    {
    //        for (int column = 0; column < Column; column++)
    //        {
    //            IsMatchableWhenSwapRight(row, column);
    //            IsMatchableWhenSwapUp(row, column);
    //        }
    //    }

    //    return _matchables.Count > 0;

    //    // check matchable when swap rightward
    //    void IsMatchableWhenSwapRight(int row, int column)
    //    {
    //        if (Swap(row, column, 0, false, true))
    //        {
    //            if (IsMatchableHorizontally(row) || IsMatchableVertically(column) || IsMatchableVertically(column + 1))
    //            {
    //                _matchables.Add((row, column, 0));
    //                _matchables.Add((row, column + 1, 2));
    //            }

    //            Swap(row, column, 0, false, true);
    //        }
    //    }

    //    // check matchable when swap upward
    //    void IsMatchableWhenSwapUp(int row, int column)
    //    {
    //        if (Swap(row, column, 1, false, true))
    //        {
    //            if (IsMatchableHorizontally(row) || IsMatchableHorizontally(row + 1) || IsMatchableVertically(column))
    //            {
    //                _matchables.Add((row, column, 1));
    //                _matchables.Add((row + 1, column, 3));
    //            }

    //            Swap(row, column, 1, false, true);
    //        }
    //    }

    //    // check matchable specific row horizontally
    //    bool IsMatchableHorizontally(int row)
    //    {
    //        var matchCount = 1;
    //        var current = (BoardItemType)(-1);

    //        for (int column = 0; column < Column; column++)
    //        {
    //            if (Slots[row, column].Item.Data.Type == current)
    //            {
    //                matchCount++;

    //                if (column == Column - 1 && matchCount >= 3) return true;
    //            }
    //            else
    //            {
    //                if (matchCount >= 3) return true;

    //                matchCount = 1;
    //                current = Slots[row, column].Item.Data.Type;
    //            }
    //        }

    //        return false;
    //    }

    //    // check matchable specific column vertically
    //    bool IsMatchableVertically(int column)
    //    {
    //        var matchCount = 1;
    //        var current = (BoardItemType)(-1);

    //        for (int row = 0; row < Row; row++)
    //        {
    //            if (Slots[row, column].Item.Data.Type == current)
    //            {
    //                matchCount++;

    //                if (row == Row - 1 && matchCount >= 3) return true;
    //            }
    //            else
    //            {
    //                if (matchCount >= 3) return true;

    //                matchCount = 1;
    //                current = Slots[row, column].Item.Data.Type;
    //            }
    //        }

    //        return false;
    //    }
    //}
}

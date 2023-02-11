using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private static readonly Vector2Int[] DIRECTIONS = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

    [SerializeField] private BoardItem _itemPrefab;
    [SerializeField] private BoardItemInfo[] _itemInfos;
    [SerializeField] private Layout _layout;
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

            for (int row = 0; row < _layout.row; row++)
            {
                for (int column = 0; column < _layout.column; column++)
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

                StartCoroutine(CoSwap(row, column, direction));
                _selected = null;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _selected = null;
        }
    }

    private void OnDrawGizmos()
    {
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

        if (_selected != null)
        {
            Gizmos.DrawWireRect(_selected.Position, _layout.size * 1.2f);
        }

        if (_matched != null)
        {
            foreach (var slot in _matched)
            {
                Gizmos.DrawWireRect(slot.Position, _layout.size * 1.2f, Color.cyan);
            }
        }
    }

    IEnumerator CoSwap(int row, int column, int direction)
    {
        var wait = new WaitUntil(Move);

        if (!Swap(row, column, direction, true, false)) yield break;

        yield return wait;

        if (!Match(_matched))
        {
            Swap(row, column, direction, true, false);
            yield return wait;
            yield break;
        }
        Remove(_matched);
        Refill(false);
        yield return wait;

        do
        {
            this.Match(_matched);
            Remove(_matched);
            Refill(false);
            yield return wait;
        }
        while (_matched.Count > 0);

        while (!Matchable(_matchables))
        {
            Shuffle();
            do
            {
                this.Match(_matched);
                Remove(_matched);
                Refill(true);
            }
            while (_matched.Count > 0);
        }

        bool Match(List<BoardSlot> matched)
        {
            matched.Clear();

            switch (direction)
            {
                // right
                case 0:
                    MatchHorizontally(row, matched);
                    MatchVertically(column, matched);
                    MatchVertically(column + 1, matched);
                    break;

                // up
                case 1:
                    MatchHorizontally(row, matched);
                    MatchHorizontally(row + 1, matched);
                    MatchVertically(column, matched);
                    break;

                // left
                case 2:
                    MatchHorizontally(row, matched);
                    MatchVertically(column, matched);
                    MatchVertically(column - 1, matched);
                    break;

                // down
                case 3:
                    MatchHorizontally(row, matched);
                    MatchHorizontally(row - 1, matched);
                    MatchVertically(column, matched);
                    break;
            }

            return matched.Count > 0;
        }

        bool Move()
        {
            for (int row = 0; row < _layout.row; row++)
            {
                for (int column = 0; column < _layout.column; column++)
                {
                    if (_slots[row, column].Item.Move) return false;
                }
            }
            return true;
        }
    }

    private void Generate()
    {
        GenerateSlots();
        GenerateItems();
        GenerateBoard();

        // generate slots
        void GenerateSlots()
        {
            var width = _layout.size.x + _layout.spacing.x;
            var height = _layout.size.y + _layout.spacing.y;
            var minX = _layout.column / 2 * width * -1 + (_layout.column % 2 == 0 ? width / 2 : 0);
            var minY = _layout.row / 2 * height * -1 + (_layout.row % 2 == 0 ? height / 2 : 0);

            _slots = new BoardSlot[_layout.row * 2, _layout.column];

            for (int row = 0; row < _layout.row * 2; row++)
            {
                for (int column = 0; column < _layout.column; column++)
                {
                    _slots[row, column] = new BoardSlot(row, column, new Vector2(minX + column * width, minY + row * height));
                }
            }
        }

        // generate items
        void GenerateItems()
        {
            for (int row = 0; row < _layout.row; row++)
            {
                for (int column = 0; column < _layout.column; column++)
                {
                    var slot = _slots[row, column];
                    var item = Instantiate(_itemPrefab);

                    item.transform.localScale = new Vector2(_layout.size.x, _layout.size.y);
                    item.transform.SetParent(transform);
                    item.Position = slot.Position;
                    item.Info = _itemInfos[Random.Range(0, _itemInfos.Length)];

                    slot.Item = item;
                }
            }
        }

        // generate complete board that is matcahble and there's no more current matched items
        void GenerateBoard()
        {
            List<BoardSlot> matched = new List<BoardSlot>();

            do
            {
                Match(matched);
                Remove(matched);
                Refill(true);
            }
            while (matched.Count > 0);

            if (!Matchable(_matchables))
            {
                Shuffle();
                GenerateBoard();
            }
        }
    }

    private void Match(List<BoardSlot> matched)
    {
        if (matched == null) return;

        matched.Clear();

        for (int row = 0; row < _layout.row; row++)
        {
            MatchHorizontally(row, matched);
        }

        for (int column = 0; column < _layout.column; column++)
        {
            MatchVertically(column, matched);
        }
    }

    private void MatchHorizontally(int row, List<BoardSlot> matched)
    {
        if (matched == null) return;

        var matchCount = 1;
        var current = (BoardItemType)(-1);

        for (int column = 0; column < _layout.column; column++)
        {
            if (_slots[row, column].Item.Info.Type == current)
            {
                matchCount++;

                if (column == _layout.column - 1 && matchCount >= 3)
                {
                    for (int i = 0; i < matchCount; i++)
                    {
                        matched.Add(_slots[row, column - i]);
                    }
                }
            }
            else
            {
                if (matchCount >= 3)
                {
                    for (int i = 1; i <= matchCount; i++)
                    {
                        matched.Add(_slots[row, column - i]);
                    }
                }

                matchCount = 1;
                current = _slots[row, column].Item.Info.Type;
            }
        }
    }

    private void MatchVertically(int column, List<BoardSlot> matched)
    {
        if (matched == null) return;

        var matchCount = 1;
        var current = (BoardItemType)(-1);

        for (int row = 0; row < _layout.row; row++)
        {
            if (_slots[row, column].Item.Info.Type == current)
            {
                matchCount++;

                if (row == _layout.row - 1 && matchCount >= 3)
                {
                    for (int i = 0; i < matchCount; i++)
                    {
                        matched.Add(_slots[row - i, column]);
                    }
                }
            }
            else
            {
                if (matchCount >= 3)
                {
                    for (int i = 1; i <= matchCount; i++)
                    {
                        matched.Add(_slots[row - i, column]);
                    }
                }

                matchCount = 1;
                current = _slots[row, column].Item.Info.Type;
            }
        }
    }

    private void Remove(List<BoardSlot> matched)
    {
        if (matched == null) return;

        foreach (var slot in matched)
        {
            if (slot.Item == null) return;

            var item = slot.Item;
            item.Info = null;
            _items.Enqueue(item);

            slot.Item = null;
        }
    }

    private void Refill(bool directly)
    {
        for (int column = 0; column < _layout.column; column++)
        {
            int blank = 0;

            Drop(column, ref blank);
            Refill(column, blank);
        }

        // drop current remain items
        void Drop(int column, ref int blank)
        {
            for (int row = 0; row < _layout.row; row++)
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
                }
            }
        }

        // refill items as needed
        void Refill(int column, int blank)
        {
            for (int i = 0; i < blank; i++)
            {
                var item = _items.Dequeue();
                var slot = _slots[_layout.row - blank + i, column];
                var tmpSlot = _slots[_layout.row + i, column];

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
                item.Info = _itemInfos[Random.Range(0, _itemInfos.Length)];

                slot.Item = item;
            }
        }
    }

    private bool Matchable(HashSet<(int, int, int)> matchables)
    {
        matchables.Clear();

        for (int row = 0; row < _layout.row; row++)
        {
            for (int column = 0; column < _layout.column; column++)
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

            for (int column = 0; column < _layout.column; column++)
            {
                if (_slots[row, column].Item.Info.Type == current)
                {
                    matchCount++;

                    if (column == _layout.column - 1 && matchCount >= 3) return true;
                }
                else
                {
                    if (matchCount >= 3) return true;

                    matchCount = 1;
                    current = _slots[row, column].Item.Info.Type;
                }
            }

            return false;
        }

        // check matchable specific column vertically
        bool IsMatchableVertically(int column)
        {
            var matchCount = 1;
            var current = (BoardItemType)(-1);

            for (int row = 0; row < _layout.row; row++)
            {
                if (_slots[row, column].Item.Info.Type == current)
                {
                    matchCount++;

                    if (row == _layout.row - 1 && matchCount >= 3) return true;
                }
                else
                {
                    if (matchCount >= 3) return true;

                    matchCount = 1;
                    current = _slots[row, column].Item.Info.Type;
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

        var swapable = 0 <= nrow && nrow < _layout.row && 0 <= ncolumn && ncolumn < _layout.column;

        if (swapable)
        {
            Swap(row, column, nrow, ncolumn, position, directly);
        }

        return swapable;
    }

    private void Swap(int row, int column, int nrow, int ncolumn, bool position, bool directly)
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
        }
    }

    private void Score(int count)
    {
        Debug.Log($"{count * 10}");
    }

    private void Shuffle()
    {
        Debug.Log("Shuffle!");

        for (int row = 0; row < _layout.row; row++)
        {
            for (int column = 0; column < _layout.column; column++)
            {
                _slots[row, column].Item.Info = _itemInfos[Random.Range(0, _itemInfos.Length)];
            }
        }
    }

    [System.Serializable]
    private struct Layout
    {
        public int row;
        public int column;
        public Vector2 size;
        public Vector2 spacing;
    }
}

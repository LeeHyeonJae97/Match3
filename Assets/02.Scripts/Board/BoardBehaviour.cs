using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class BoardBehaviour : MonoBehaviour
{
    [SerializeField] private ItemBehaviour _itemPrefab;
    [SerializeField] private Board _board;
    [SerializeField] private BoardLayout _boardLayout;
    [SerializeField] private InputManager _inputManager;
    private ItemBehaviour _selected;
    private List<ItemBehaviour> _matched;
    private float _matchCalledTime;

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

    public bool Matchable()
    {
        return MatchRow() || MatchColumn();

        // LOCAL FUNCTION
        bool MatchRow()
        {
            for (int r = 0; r < _boardLayout.Row; r++)
            {
                var item = default(ItemBehaviour);
                var score = 1;

                for (int c = 0; c < _boardLayout.Column; c++)
                {
                    var current = _board.GetItemBehaviour(r, c);

                    var same = item != null && current != null && current.IsSame(item);

                    if (same)
                    {
                        score++;

                        if (c == _boardLayout.Column - 1 && score >= 3) return true;
                    }
                    else
                    {
                        if (score >= 3) return true;

                        item = current;
                        score = 1;
                    }
                }
            }

            return false;
        }

        // LOCAL FUNCTION
        bool MatchColumn()
        {
            for (int c = 0; c < _boardLayout.Column; c++)
            {
                var item = default(ItemBehaviour);
                var score = 1;

                for (int r = 0; r < _boardLayout.Row; r++)
                {
                    var current = _board.GetItemBehaviour(r, c);

                    var same = item != null && current != null && current.IsSame(item);

                    if (same)
                    {
                        score++;

                        if (r == _boardLayout.Row - 1 && score >= 3) return true;
                    }
                    else
                    {
                        if (score >= 3) return true;

                        item = current;
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
            if (_matchCalledTime == Time.frameCount) return false;

            _matchCalledTime = Time.frameCount;

            return true;
        }

        // LOCAL FUNCTION
        IEnumerator CoMatch()
        {
            yield return new WaitForEndOfFrame();

            Match(_matched);

            if (_matched.Count > 0)
            {
                CoRefill(_matched);
                Drop();
            }
            Reset();
        }
    }

    private void Initialize()
    {
        Validate();
        Initialize();
        Shuffle();

        // LOCAL FUNCTION
        void Validate()
        {
            Assert.IsNotNull(_itemPrefab);
            Assert.IsNotNull(_board);
            Assert.IsNotNull(_boardLayout);
            Assert.IsNotNull(_inputManager);
        }

        // LOCAL FUNCTION
        void Initialize()
        {
            _matched = new List<ItemBehaviour>();

            var row = _boardLayout.Row;
            var column = _boardLayout.Column;
            var size = _boardLayout.Size;
            var spacing = _boardLayout.Spacing;

            var width = size + spacing;
            var height = size + spacing;
            var minX = column / 2 * width * -1 + (column % 2 == 0 ? width / 2 : 0);
            var minY = row / 2 * height * -1 + (row % 2 == 0 ? height / 2 : 0);

            var itemBehaviours = new List<ItemBehaviour>();

            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < column; c++)
                {
                    var item = GameObject.Instantiate(_itemPrefab, transform);

                    item.transform.position = new Vector2(minX + c * width, minY + r * height);
                    item.transform.localScale = Vector2.one * size;

                    item.Initialize(this, _board, _boardLayout, _board.GetItem(ItemType.Candy));

                    itemBehaviours.Add(item);
                }
            }

            _board.Initialize(itemBehaviours);
        }

        // LOCAL FUNCTION
        void Shuffle()
        {
            do
            {
                for (int i = 0; i < _matched.Count; i++)
                {
                    _matched[i].Initialize(_board.GetItem(ItemType.Candy));
                }

                _matched.Clear();

                Match(_matched);
            }
            while (_matched.Count > 0);

            Reset();
        }
    }

    private void Match(List<ItemBehaviour> matched)
    {
        var matchGroup = 0;

        var skip = new int[_boardLayout.Column];

        for (int i = 0; i < skip.Length; i++)
        {
            skip[i] = _boardLayout.Row;
        }

        MatchColumn();
        MatchRow();

        // LOCAL FUNCTION
        void MatchColumn()
        {
            for (int c = 0; c < _boardLayout.Column; c++)
            {
                var item = default(ItemBehaviour);
                var score = 1;

                for (int r = 0; r < _boardLayout.Row; r++)
                {
                    var current = _board.GetItemBehaviour(r, c);

                    // to prevent dropping items are matched
                    if (current == null)
                    {
                        skip[c] = r;
                        break;
                    }

                    var same = item != null && current != null && current.IsSame(item);

                    if (same)
                    {
                        score++;

                        var match = r == _boardLayout.Row - 1 && score >= 3;

                        if (match)
                        {
                            Update(r, c, 0, score, score, GetMatchGroup(r, c, score));
                        }
                    }
                    else
                    {
                        var match = score >= 3;

                        if (match)
                        {
                            Update(r, c, 1, score + 1, score, GetMatchGroup(r, c, score));
                        }

                        item = current;
                        score = 1;
                    }
                }
            }

            // LOCAL FUNCTION
            int GetMatchGroup(int r, int c, int score)
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

                return mg;
            }

            // LOCAL FUNCTION
            void Update(int r, int c, int min, int max, int score, int matchGroup)
            {
                for (int i = min; i < max; i++)
                {
                    var slot = _board.GetSlot(r - i, c);

                    slot.ColumnScore = Mathf.Max(slot.ColumnScore, score);
                    slot.MatchGroup = matchGroup;

                    _board.GetItemBehaviour(r - i, c).OnRemoved(matched, null);
                }
            }
        }

        // LOCAL FUNCTION
        void MatchRow()
        {
            for (int r = 0; r < _boardLayout.Row; r++)
            {
                var item = default(ItemBehaviour);
                var score = 1;

                for (int c = 0; c < _boardLayout.Column; c++)
                {
                    var current = r < skip[c] ? _board.GetItemBehaviour(r, c) : null;

                    var same = item != null && current != null && current.IsSame(item);

                    if (same)
                    {
                        score++;

                        var match = c == _boardLayout.Column - 1 && score >= 3;

                        if (match)
                        {
                            Update(r, c, 0, score, score, GetMatchGrouop(r, c, score));
                        }
                    }
                    else
                    {
                        var match = score >= 3;

                        if (match)
                        {
                            Update(r, c, 1, score + 1, score, GetMatchGrouop(r, c, score));
                        }

                        item = current;
                        score = 1;
                    }
                }
            }

            // LOCAL FUNCTION
            int GetMatchGrouop(int r, int c, int score)
            {
                var mg = 0;

                for (int i = 0; i < score; i++)
                {
                    var tmp = _board.GetSlot(r, c - i).MatchGroup;

                    if (tmp > 0)
                    {
                        mg = tmp;
                    }
                }
                if (mg == 0) mg = ++matchGroup;

                return mg;
            }

            // LOCAL FUNCTION
            void Update(int r, int c, int min, int max, int score, int matchGroup)
            {
                for (int i = min; i < max; i++)
                {
                    var slot = _board.GetSlot(r, c - i);

                    slot.RowScore = Mathf.Max(slot.RowScore, score);
                    slot.MatchGroup = matchGroup;

                    _board.GetItemBehaviour(r, c - i).OnRemoved(matched, null);
                }
            }
        }
    }

    private void CoRefill(List<ItemBehaviour> matched)
    {
        matched.Sort((l, r) =>
        {
            _boardLayout.GetRowColumn(l, out var lr, out var lc);
            _boardLayout.GetRowColumn(r, out var rr, out var rc);

            var ls = _board.GetSlot(lr, lc);
            var rs = _board.GetSlot(rr, rc);

            var comp = ls.MatchGroup.CompareTo(rs.MatchGroup);

            if (comp == 0)
            {
                comp = ls.Refreshed ? -1 : 1;
            }

            return comp;
        });

        var pivot = default(ItemBehaviour);
        var matchGroup = 0;

        var slots = new List<Slot>();
        var cors = new List<Coroutine>();

        var counts = new int[_boardLayout.Column];

        for (int column = 0; column < _boardLayout.Column; column++)
        {
            for (int row = 0; row < _boardLayout.Row; row++)
            {
                if (_board.GetItemBehaviour(row, column) == null) counts[column]++;
            }
        }

        foreach (var item in matched)
        {
            _boardLayout.GetRowColumn(item, out var row, out var column);

            var slot = _board.GetSlot(row, column);

            if (NeedSpecialItem(slot))
            {
                if (slot.Refreshed && slot.MatchGroup != matchGroup)
                {
                    pivot = item;
                    matchGroup = slot.MatchGroup;

                    item.Initialize(_board.GetItem(GetSpecialItemType(slot)));

                    // TODO :
                    // 스페셜 아이템이 그 자리에 추가되었기 때문에 Slot의 Refreshed를 true로 해주어야한다.
                }
                else
                {
                    counts[column]++;

                    var pivotPosition = pivot.transform.position;
                    var refillPosition = _boardLayout.GetPosition(_boardLayout.Row - 1 + counts[column], column);

                    //var cor = StartCoroutine(CoMove(item, pivotPosition, refillPosition));

                    //cors.Add(cor);

                    item.transform.position = refillPosition;
                    item.Initialize(_board.GetItem(ItemType.Candy));
                }
            }
            else
            {
                counts[column]++;

                item.transform.position = _boardLayout.GetPosition(_boardLayout.Row - 1 + counts[column], column);

                item.Initialize(_board.GetItem(ItemType.Candy));
            }
        }

        //foreach (var cor in cors)
        //{
        //    yield return cor;
        //}

        // LOCAL FUNCTIOn
        bool NeedSpecialItem(Slot slot)
        {
            return (slot.MatchGroup != 0 && slot.MatchGroup == matchGroup) || (slot.RowScore >= 3 && slot.ColumnScore >= 3) || slot.RowScore > 3 || slot.ColumnScore > 3;
        }

        // LOCAL FUNCTION
        ItemType GetSpecialItemType(Slot slot)
        {
            if (slot.RowScore >= 3 && slot.ColumnScore >= 3)
            {
                return ItemType.Candy;
            }
            else if (slot.RowScore == 4)
            {
                return ItemType.VSCandy;
            }
            else if (slot.RowScore == 5)
            {
                return ItemType.VSCandy;
            }
            else if (slot.ColumnScore == 4)
            {
                return ItemType.HSCandy;
            }
            else if (slot.ColumnScore == 5)
            {
                return ItemType.HSCandy;
            }
            else
            {
                return ItemType.Candy;
            }
        }

        // LOCAL FUNCTION
        IEnumerator CoMove(ItemBehaviour item, Vector3 pivot, Vector3 refill)
        {
            yield return StartCoroutine(item.CoMove(pivot));

            item.transform.position = refill;

            item.Initialize(_board.GetItem(ItemType.Candy));
        }
    }

    private void Drop()
    {
        for (int c = 0; c < _boardLayout.Column; c++)
        {
            int count = 0;

            for (int r = 0; r < _boardLayout.Row + count; r++)
            {
                var item = _board.GetItemBehaviour(r, c);

                if (item == null && r < _boardLayout.Row)
                {
                    count++;
                }
                else if (item != null && (count > 0 || (_board.TryGetSlot(item, out var slot) && slot.Refreshed)))
                {
                    item.Drop(count);
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
        _boardLayout.GetRowColumn(position, out var row, out var column);

        _selected = _board.GetItemBehaviour(row, column);
    }

    private void OnScrolled(Vector2 delta)
    {
        if (_selected == null || delta.sqrMagnitude < 1000) return;

        var direction = Mathf.Abs(delta.x) > Mathf.Abs(delta.y) ? new Vector2Int(delta.x > 0 ? 1 : -1, 0) : new Vector2Int(0, delta.y > 0 ? 1 : -1);

        _selected.OnSwapped(direction);
        _selected = null;
    }

    private void OnReleased()
    {
        _selected = null;
    }

    private void OnDrawGizmos()
    {
        _board.OnDrawGizmos();
        
        HighlightSelectedItem();

        // LOCAL FUNCTION
        void HighlightSelectedItem()
        {
            if (_selected == null) return;

            Gizmos.DrawWireRect(_selected.transform.position, _selected.transform.localScale * 1.2f, Color.red);
        }
    }
}

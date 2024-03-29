using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public class BoardBehaviour : MonoBehaviour
{
    [SerializeField] private ItemBehaviour _itemPrefab;
    [SerializeField] private Board _board;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private bool _drawItem;
    [SerializeField] private bool _drawSlot;
    private ItemBehaviour _selected;
    private List<ItemBehaviour> _matched;
    private float _matchCalledTime;

    [System.Obsolete]
    private ItemBehaviour _nearMouse;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void OnEnable()
    {
        _inputManager.Handler.OnTouched += OnTouched;
        _inputManager.Handler.OnScrolled += OnScrolled;
        _inputManager.Handler.OnReleased += OnReleased;
    }

    private void OnDisable()
    {
        _inputManager.Handler.OnTouched -= OnTouched;
        _inputManager.Handler.OnScrolled -= OnScrolled;
        _inputManager.Handler.OnReleased -= OnReleased;
    }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        TempSaveLoad();
        FindNearMouse();

        [System.Obsolete]
        void TempSaveLoad()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                _board.Save();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                _board.Load();
            }
        }

        [System.Obsolete]
        void FindNearMouse()
        {
            var mouse = Input.mousePosition;
            mouse.z -= Camera.main.transform.position.z;
            var pos = Camera.main.ScreenToWorldPoint(mouse);

            _nearMouse = _board.GetItemBehaviourApproximately(pos);
        }
    }

    public bool Matchable()
    {
        return MatchRow() || MatchColumn();

        // LOCAL FUNCTION
        bool MatchRow()
        {
            for (int r = 0; r < _board.Layout.Row; r++)
            {
                var item = default(ItemBehaviour);
                var score = 1;

                for (int c = 0; c < _board.Layout.Column; c++)
                {
                    var current = _board.GetItemBehaviour(r, c);

                    var same = item != null && current != null && current.IsSame(item);

                    if (same)
                    {
                        score++;

                        if (c == _board.Layout.Column - 1 && score >= 3) return true;
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
            for (int c = 0; c < _board.Layout.Column; c++)
            {
                var item = default(ItemBehaviour);
                var score = 1;

                for (int r = 0; r < _board.Layout.Row; r++)
                {
                    var current = _board.GetItemBehaviour(r, c);

                    var same = item != null && current != null && current.IsSame(item);

                    if (same)
                    {
                        score++;

                        if (r == _board.Layout.Row - 1 && score >= 3) return true;
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
                yield return StartCoroutine(CoUpdate(_matched));
                Drop();
            }
            Reset();
        }
    }

    public void Match(ItemBehaviour itemBehaviour, ItemBehaviour target)
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

            itemBehaviour.OnRemoved(_matched, target);
            target.OnRemoved(_matched, null);

            yield return StartCoroutine(CoUpdate(_matched));
            Drop();
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
            Assert.IsNotNull(_inputManager);
        }

        // LOCAL FUNCTION
        void Initialize()
        {
            _matched = new List<ItemBehaviour>();

            var row = _board.Layout.Row;
            var column = _board.Layout.Column;
            var size = _board.Layout.Size;
            var spacing = _board.Layout.Spacing;

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

                    item.Initialize(this, _board, _board.GetItem(ItemType.Candy));

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

        var skip = new int[_board.Layout.Column];

        for (int i = 0; i < skip.Length; i++)
        {
            skip[i] = _board.Layout.Row;
        }

        MatchColumn();
        MatchRow();

        // LOCAL FUNCTION
        void MatchColumn()
        {
            for (int c = 0; c < _board.Layout.Column; c++)
            {
                var item = default(ItemBehaviour);
                var score = 1;

                for (int r = 0; r < _board.Layout.Row; r++)
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

                        var match = r == _board.Layout.Row - 1 && score >= 3;

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
            for (int r = 0; r < _board.Layout.Row; r++)
            {
                var item = default(ItemBehaviour);
                var score = 1;

                for (int c = 0; c < _board.Layout.Column; c++)
                {
                    var current = r < skip[c] ? _board.GetItemBehaviour(r, c) : null;

                    var same = item != null && current != null && current.IsSame(item);

                    if (same)
                    {
                        score++;

                        var match = c == _board.Layout.Column - 1 && score >= 3;

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

    private IEnumerator CoUpdate(List<ItemBehaviour> matched)
    {
        Sort();
        yield return StartCoroutine(CoRearrange());

        // LOCAL FUNCTION
        void Sort()
        {
            matched.Sort((l, r) =>
            {
                _board.Layout.GetRowColumn(l, out var lr, out var lc);
                _board.Layout.GetRowColumn(r, out var rr, out var rc);

                var ls = _board.GetSlot(lr, lc);
                var rs = _board.GetSlot(rr, rc);

                var comp = ls.MatchGroup.CompareTo(rs.MatchGroup);

                if (comp == 0)
                {
                    comp = ls.Refreshed ? -1 : 1;
                }

                return comp;
            });
        }

        // LOCAL FUNCTION
        IEnumerator CoRearrange()
        {
            var pivot = default(ItemBehaviour);
            var matchGroup = 0;

            var slots = new List<Slot>();
            var cors = new List<Coroutine>();

            var counts = new int[_board.Layout.Column];

            for (int column = 0; column < _board.Layout.Column; column++)
            {
                for (int row = 0; row < _board.Layout.Row; row++)
                {
                    if (_board.GetItemBehaviour(row, column) == null) counts[column]++;
                }
            }

            foreach (var item in matched)
            {
                _board.Layout.GetRowColumn(item, out var row, out var column);

                var slot = _board.GetSlot(row, column);

                if (NeedSpecialItem(slot))
                {
                    if (slot.Refreshed && slot.MatchGroup != matchGroup)
                    {
                        pivot = item;
                        matchGroup = slot.MatchGroup;

                        item.Initialize(GetSpecialItemType(item, slot));
                    }
                    else
                    {
                        counts[column]++;

                        var pivotPosition = pivot.transform.position;
                        var refillPosition = _board.Layout.GetPosition(_board.Layout.Row - 1 + counts[column], column);

                        var cor = StartCoroutine(CoMove(item, pivotPosition, refillPosition));
                        cors.Add(cor);
                    }
                }
                else
                {
                    counts[column]++;

                    item.transform.position = _board.Layout.GetPosition(_board.Layout.Row - 1 + counts[column], column);

                    item.Initialize(_board.GetItem(ItemType.Candy));
                }
            }

            foreach (var cor in cors)
            {
                yield return cor;
            }

            // LOCAL FUNCTION
            bool NeedSpecialItem(Slot slot)
            {
                return (slot.MatchGroup != 0 && slot.MatchGroup == matchGroup) || slot.Special;
            }

            // LOCAL FUNCTION
            Item GetSpecialItemType(ItemBehaviour itemBehaviour, Slot slot)
            {
                if (slot.RowScore >= 3 && slot.ColumnScore >= 3)
                {
                    return _board.GetItem(ItemType.WCandy, itemBehaviour.Item.Color);
                }
                else if (slot.RowScore == 4)
                {
                    return _board.GetItem(ItemType.VSCandy, itemBehaviour.Item.Color);
                }
                else if (slot.RowScore == 5)
                {
                    return _board.GetItem(ItemType.Rainbow, ItemColor.None);
                }
                else if (slot.ColumnScore == 4)
                {
                    return _board.GetItem(ItemType.HSCandy, itemBehaviour.Item.Color);
                }
                else if (slot.ColumnScore == 5)
                {
                    return _board.GetItem(ItemType.Rainbow, ItemColor.None);
                }
                else
                {
                    return null;
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
    }

    private void Drop()
    {
        for (int c = 0; c < _board.Layout.Column; c++)
        {
            int count = 0;

            for (int r = 0; r < _board.Layout.Row + count; r++)
            {
                var item = _board.GetItemBehaviourApproximately(r, c);

                if (item == null && r < _board.Layout.Row)
                {
                    count++;
                }
                else if (item != null && (count > 0 || (_board.TryGetSlot(item, out var slot) && slot.Refreshed && slot.Special)))
                {
                    item.Drop(count);
                }
            }
        }
    }

    private void Reset()
    {
        _matched.Clear();
        _board.Reset();
    }

    private void OnDrawGizmos()
    {
        DrawItems();
        DrawSlots();
        HighlightSelectedItem();
        HighlightNearMouseItem();

        // LOCAL FUNCTION
        void DrawItems()
        {
            if (!Application.isPlaying || !_drawItem) return;

            for (int row = 0; row < _board.Layout.Row; row++)
            {
                for (int column = 0; column < _board.Layout.Column; column++)
                {
                    Handles.color = Color.black;
                    Handles.Label(_board.Layout.GetPosition(row, column), $"({column},{row})");
                }
            }
        }

        // LOCAL FUNCTION
        void DrawSlots()
        {
            if (!Application.isPlaying || !_drawSlot) return;

            for (int row = 0; row < _board.Layout.Row; row++)
            {
                for (int column = 0; column < _board.Layout.Column; column++)
                {
                    var slot = _board.GetSlot(row, column);
                    var rs = slot.RowScore;
                    var cs = slot.ColumnScore;
                    var mg = slot.MatchGroup;
                    var refreshed = slot.Refreshed;

                    var position = _board.Layout.GetPosition(row, column);

                    Handles.color = Color.black;
                    Handles.Label(position, $"({rs},{cs}) / {mg} / {refreshed}");
                }
            }
        }

        // LOCAL FUNCTION
        void HighlightSelectedItem()
        {
            if (_selected == null) return;

            Gizmos.DrawWireRect(_selected.transform.position, _selected.transform.localScale * 1.2f, Color.red);
        }

        // LOCAL FUNCTION
        void HighlightNearMouseItem()
        {
            if (_nearMouse == null) return;

            Gizmos.DrawWireRect(_nearMouse.transform.position, _nearMouse.transform.localScale * 1.2f, Color.green);
        }
    }

    private void OnTouched(Vector2 position)
    {
        if (_selected != null || IsMoving()) return;

        _board.Layout.GetRowColumn(position, out var row, out var column);

        _selected = _board.GetItemBehaviour(row, column);

        // LOCAL FUNCTION
        bool IsMoving()
        {
            for (int row = 0; row < _board.Layout.Row; row++)
            {
                for (int column = 0; column < _board.Layout.Column; column++)
                {
                    if (_board.GetItemBehaviour(row, column) == null) return true;
                }
            }
            return false;
        }
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
}

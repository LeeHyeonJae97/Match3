using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NewItemColor { Red, Green, Blue, None = -1 }

public class NewItem : MonoBehaviour
{
    [System.Obsolete] public NewItemStorage ItemStorage { set { _itemStorage = value; } }
    [System.Obsolete] public NewBoardLayout Layout { set { _layout = value; } }
    [System.Obsolete] public NewItemColor Color { get; private set; }

    [SerializeField] private NewBoardLayout _layout;
    [SerializeField] private NewItemStorage _itemStorage;

    private void Awake()
    {
        Initialize();

        // LOCAL FUNCTION
        [System.Obsolete]
        void Initialize()
        {
            var colors = new Color[] { UnityEngine.Color.red, UnityEngine.Color.green, UnityEngine.Color.blue };

            var index = Random.Range(0, 3);

            Color = (NewItemColor)index;
            GetComponentInChildren<SpriteRenderer>().color = colors[index];
        }
    }

    public void Swap(Vector2Int direction)
    {
        StartCoroutine(CoSwap());

        // LOCAL FUNCTION
        IEnumerator CoSwap()
        {
            _layout.GetRowColumn(transform.position, out var row, out var column);

            var nrow = row + direction.y;
            var ncolumn = column + direction.x;

            if (!_layout.IsValid(nrow, ncolumn)) yield break;

            var swapped = _itemStorage.GetItem(nrow, ncolumn);

            if (swapped == null) yield break;

            yield return StartCoroutine(CoSwap());

            var matched = new List<NewItem>();

            Match(matched);
            swapped.Match(matched);

            if (matched.Count == 0)
            {
                yield return StartCoroutine(CoSwap());
            }
            else
            {
                Remove();
                StartCoroutine(CoDrop());
            }

            // LOCAL FUNCTION
            IEnumerator CoSwap()
            {
                var cors = new Coroutine[]
                {
                    StartCoroutine(swapped.CoMove(transform.position)),
                    StartCoroutine(CoMove(swapped.transform.position))
                };

                foreach (var cor in cors)
                {
                    yield return cor;
                }
            }

            // LOCAL FUNCTION
            void Remove()
            {
                foreach (var item in matched)
                {
                    item.transform.position = Vector2.one * 1000;
                }
            }

            // LOCAL FUNCTION
            IEnumerator CoDrop()
            {
                for (int r = 0; r < _layout.Row; r++)
                {
                    for (int c = 0; c < _layout.Column; c++)
                    {
                        yield return null;

                        _itemStorage.GetItem(r, c)?.Drop();
                    }
                }
            }
        }
    }

    private void Drop()
    {
        StartCoroutine(CoDrop());

        // LOCAL FUNCTION
        IEnumerator CoDrop()
        {
            while (true)
            {
                _layout.GetRowColumn(transform.position, out var row, out var column);

                bool droppable = _layout.IsValid(row - 1, column) && _itemStorage.GetItem(row - 1, column) == null;

                if (!droppable) break;

                yield return StartCoroutine(CoMove(_layout.GetPosition(row - 1, column)));
            }
        }
    }

    private IEnumerator CoMove(Vector2 position)
    {
        transform.DOKill(false);

        yield return transform.DOMove(position, .1f).WaitForCompletion();
    }

    private void Match(List<NewItem> matched)
    {
        RowMatchable();
        ColumnMatchable();
        SquareMatchable();

        // LOCAL FUNCTION
        void RowMatchable()
        {
            _layout.GetRowColumn(transform.position, out var row, out var column);

            var color = NewItemColor.None;
            var count = 1;

            var start = column - 2;
            var end = column + 2;

            for (int c = start; c <= end; c++)
            {
                var item = _itemStorage.GetItem(row, c);

                if (color != NewItemColor.None && item != null && item.Color == color)
                {
                    count++;

                    if (c == end && count >= 3)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            matched.Add(_itemStorage.GetItem(row, c - i));
                        }
                    }
                }
                else
                {
                    if (count >= 3)
                    {
                        for (int i = 1; i <= count; i++)
                        {
                            matched.Add(_itemStorage.GetItem(row, c - i));
                        }
                    }

                    color = item == null ? NewItemColor.None : item.Color;
                    count = 1;
                }

                //Debug.Log($"RowMatchable : {c} {row} {color} {item?.Color} {count}");
            }
        }

        // LOCAL FUNCTION
        void ColumnMatchable()
        {
            _layout.GetRowColumn(transform.position, out var row, out var column);

            var color = NewItemColor.None;
            var count = 1;

            var start = row - 2;
            var end = row + 2;

            for (int r = start; r <= end; r++)
            {
                var item = _itemStorage.GetItem(r, column);

                if (color != NewItemColor.None && item != null && item.Color == color)
                {
                    count++;

                    if (r == end && count >= 3)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            matched.Add(_itemStorage.GetItem(r - i, column));
                        }
                    }
                }
                else
                {
                    if (count >= 3)
                    {
                        for (int i = 1; i <= count; i++)
                        {
                            matched.Add(_itemStorage.GetItem(r - i, column));
                        }
                    }

                    color = item == null ? NewItemColor.None : item.Color;
                    count = 1;
                }

                //Debug.Log($"ColumnMatchable : {column} {r} {color} {item?.Color} {count}");
            }
        }

        // LOCAL FUNCTION
        void SquareMatchable()
        {

        }
    }
}

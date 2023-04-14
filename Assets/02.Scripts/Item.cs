using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemColor { Red, Green, Blue, White, Black, Gray, None = -1 }

public class Item : MonoBehaviour
{
    [SerializeField] private BoardBehaviour _boardBehaviour;
    [SerializeField] private BoardLayout _layout;
    [SerializeField] private Board _board;

    [System.Obsolete] public ItemColor Color { get; private set; }

    public void Swap(Vector2Int direction)
    {
        if (GetSwappedItem(out var swapped))
        {
            StartCoroutine(CoSwap());
        }

        // LOCAL FUNCTION
        bool GetSwappedItem(out Item swapped)
        {
            _layout.GetRowColumn(transform.position, out var row, out var column);

            var nrow = row + direction.y;
            var ncolumn = column + direction.x;

            swapped = _layout.IsValid(nrow, ncolumn) ? _board.GetItem(nrow, ncolumn) : null;

            return swapped != null;
        }

        // LOCAL FUNCTION
        IEnumerator CoSwap()
        {
            Coroutine[] cors =
            {
                StartCoroutine(CoMove(swapped.transform.position)),
                StartCoroutine(swapped.CoMove(transform.position))
            };

            foreach (var cor in cors)
            {
                yield return cor;
            }

            _board.GetSlot(transform.position).Refreshed = true;
            _board.GetSlot(swapped.transform.position).Refreshed = true;

            if (!_boardBehaviour.Matchable())
            {
                cors[0] = StartCoroutine(CoMove(swapped.transform.position));
                cors[1] = StartCoroutine(swapped.CoMove(transform.position));

                foreach (var cor in cors)
                {
                    yield return cor;
                }
            }
            else
            {
                _boardBehaviour.Match();
            }
        }
    }

    public IEnumerator CoDrop(int count)
    {
        yield return StartCoroutine(CoDrop());
        SetRefreshed();
        Match();

        // LOCAL FUNTION
        IEnumerator CoDrop()
        {
            for (int i = 0; i < count; i++)
            {
                _layout.GetRowColumn(transform.position, out var row, out var column);

                yield return StartCoroutine(CoMove(_layout.GetPosition(row - 1, column)));
            }
        }

        // LOCAL FUNCTION
        void SetRefreshed()
        {
            _board.GetSlot(transform.position).Refreshed = true;
        }

        // LOCAL FUNCTION
        void Match()
        {
            _boardBehaviour.Match();
        }
    }

    public IEnumerator CoMove(Vector2 position, float speed)
    {
        transform.DOKill(false);

        yield return transform.DOMove(position, 1 / speed).SetEase(Ease.Linear).WaitForCompletion();
    }

    private IEnumerator CoMove(Vector2 position)
    {
        transform.DOKill(false);

        yield return transform.DOMove(position, .2f).SetEase(Ease.Linear).WaitForCompletion();
    }

    [System.Obsolete]
    public void Initialize(BoardBehaviour boardBehaviour, Board board, BoardLayout layout)
    {
        _boardBehaviour = boardBehaviour;
        _board = board;
        _layout = layout;

        SetColor();
    }

    [System.Obsolete]
    public void SetColor()
    {
        var colors = new Color[]
        {
            UnityEngine.Color.red,
            UnityEngine.Color.green,
            UnityEngine.Color.blue,
            UnityEngine.Color.white,
            UnityEngine.Color.black,
            UnityEngine.Color.gray
        };

        var color = Random.Range(0, colors.Length);

        SetColor(color);
    }

    [System.Obsolete]
    public void SetColor(int color)
    {
        var colors = new Color[]
        {
            UnityEngine.Color.red,
            UnityEngine.Color.green,
            UnityEngine.Color.blue,
            UnityEngine.Color.white,
            UnityEngine.Color.black,
            UnityEngine.Color.gray
        };

        Color = (ItemColor)color;
        GetComponentInChildren<SpriteRenderer>().color = colors[color];
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NewItemColor { Red, Green, Blue, None = -1 }

public class Item : MonoBehaviour
{
    [SerializeField] private BoardBehaviour _boardBehaviour;
    [SerializeField] private BoardLayout _layout;
    [SerializeField] private Board _board;

    [System.Obsolete] public NewItemColor Color { get; private set; }

    private void Start()
    {
        SetColor();
    }

    public void Swap(Vector2Int direction)
    {
        if (GetSwapped(out var swapped))
        {
            StartCoroutine(CoSwap());
        }

        // LOCAL FUNCTION
        bool GetSwapped(out Item swapped)
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

            bool matched = _boardBehaviour.Match();

            if (!matched)
            {
                cors[0] = StartCoroutine(CoMove(swapped.transform.position));
                cors[1] = StartCoroutine(swapped.CoMove(transform.position));

                foreach (var cor in cors)
                {
                    yield return cor;
                }
            }
        }
    }

    public IEnumerator CoDrop(int count)
    {
        for (int i = 0; i < count; i++)
        {
            _layout.GetRowColumn(transform.position, out var row, out var column);

            yield return StartCoroutine(CoMove(_layout.GetPosition(row - 1, column)));
        }

        _board.GetSlot(transform.position).Refreshed = true;
    }

    private IEnumerator CoMove(Vector2 position)
    {
        transform.DOKill(false);

        yield return transform.DOMove(position, .5f).SetEase(Ease.Linear).WaitForCompletion();
    }

    [System.Obsolete]
    public void Initialize(BoardBehaviour boardBehaviour, Board board, BoardLayout layout)
    {
        _boardBehaviour = boardBehaviour;
        _board = board;
        _layout = layout;
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

        var index = Random.Range(0, colors.Length);

        Color = (NewItemColor)index;
        GetComponentInChildren<SpriteRenderer>().color = colors[index];
    }

    [System.Obsolete]
    public void SetColorSpecial()
    {
        Color = NewItemColor.None;

        var sr = GetComponentInChildren<SpriteRenderer>();
        var color = sr.color;
        color.a = .5f;
        sr.color = color;
    }
}

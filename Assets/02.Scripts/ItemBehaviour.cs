using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    public Item Data => _data;

    private Item _data;
    private BoardBehaviour _boardBehaviour;
    private Board _board;
    private BoardLayout _boardLayout;
    private SpriteRenderer _sr;

    private void Awake()
    {
        _sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void Swap(Vector2Int direction)
    {
        if (GetSwappedItem(out var swapped))
        {
            StartCoroutine(CoSwap());
        }

        // LOCAL FUNCTION
        bool GetSwappedItem(out ItemBehaviour swapped)
        {
            _boardLayout.GetRowColumn(transform.position, out var row, out var column);

            var nrow = row + direction.y;
            var ncolumn = column + direction.x;

            swapped = _boardLayout.IsValid(nrow, ncolumn) ? _board.GetItemBehaviour(nrow, ncolumn) : null;

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
                _boardLayout.GetRowColumn(transform.position, out var row, out var column);

                yield return StartCoroutine(CoMove(_boardLayout.GetPosition(row - 1, column)));
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

    public IEnumerator CoMove(Vector2 position)
    {
        yield return StartCoroutine(CoMove(position, 5f));
    }

    public IEnumerator CoMove(Vector2 position, float speed)
    {
        transform.DOKill(false);

        yield return transform.DOMove(position, 1 / speed).SetEase(Ease.Linear).WaitForCompletion();
    }

    public void Initialize(BoardBehaviour boardBehaviour, Board board, BoardLayout boardLayout, Item item)
    {
        _boardBehaviour = boardBehaviour;
        _board = board;
        _boardLayout = boardLayout;

        Initialize(item);
    }

    public void Initialize(Item item)
    {
        _data = item;

        _sr.sprite = item.Sprite;
    }
}

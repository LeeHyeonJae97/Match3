using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    private Item _item;
    private BoardBehaviour _boardBehaviour;
    private Board _board;
    private BoardLayout _boardLayout;
    private SpriteRenderer _sr;

    private void Awake()
    {
        _sr = GetComponentInChildren<SpriteRenderer>();
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
        _item = item;

        _sr.sprite = item.Sprite;
    }

    public void OnSwapped(Vector2Int direction)
    {
        _item.SwappedStrategy.OnSwapped(direction, this, _boardBehaviour, _board, _boardLayout);
    }

    public void OnRemoved(List<ItemBehaviour> matched, ItemBehaviour remover)
    {
        _item.RemovedStrategy.OnRemoved(matched, remover, _board, _boardLayout, this);
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

    public bool IsSame(ItemBehaviour itemBehaviour)
    {
        return _item.Color == itemBehaviour._item.Color;
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    public Item Item => _item;

    [SerializeField] private float _speed;
    private Item _item;
    private BoardBehaviour _boardBehaviour;
    private Board _board;
    private SpriteRenderer _sr;

    private void Awake()
    {
        _sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void Initialize(BoardBehaviour boardBehaviour, Board board, Item item)
    {
        _boardBehaviour = boardBehaviour;
        _board = board;

        Initialize(item);
    }

    public void Initialize(Item item)
    {
        _item = item;

        _sr.sprite = item.Sprite;
    }

    public void OnSwapped(Vector2Int direction)
    {
        _item.SwappedStrategy.OnSwapped(direction, _boardBehaviour, _board, this);
    }

    public void OnRemoved(List<ItemBehaviour> matched, ItemBehaviour remover)
    {
        _item.RemovedStrategy.OnRemoved(matched, remover, _board, this);
    }

    public void Drop(int count)
    {
        StopAllCoroutines();
        StartCoroutine(CoDrop());

        IEnumerator CoDrop()
        {
            yield return count > 0 ? StartCoroutine(CoDrop()) : null;
            SetRefreshed();
            Match();

            // LOCAL FUNTION
            IEnumerator CoDrop()
            {
                for (int i = 0; i < count; i++)
                {
                    _board.Layout.GetRowColumn(this, out var row, out var column);

                    yield return StartCoroutine(CoMove(_board.Layout.GetPosition(row - 1, column)));
                }
            }

            // LOCAL FUNCTION
            void SetRefreshed()
            {
                if (_board.TryGetSlot(this, out var slot))
                {
                    slot.Refreshed = true;
                }
            }

            // LOCAL FUNCTION
            void Match()
            {
                _boardBehaviour.Match();
            }
        }
    }

    public IEnumerator CoMove(Vector2 position)
    {
        transform.DOKill(false);

        yield return transform.DOMove(position, 1 / _speed).SetEase(Ease.Linear).WaitForCompletion();
    }

    public bool IsSame(ItemBehaviour itemBehaviour)
    {
        return _item.Color == itemBehaviour._item.Color;
    }
}

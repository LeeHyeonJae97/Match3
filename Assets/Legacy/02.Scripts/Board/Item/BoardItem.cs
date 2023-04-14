using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// item in slot
/// </summary>
public class BoardItem : MonoBehaviour
{
    
    public Vector2 Position                            // item's current position
    {
        get { return transform.position; }

        set { transform.position = value; }
    }
    public Vector2 TargetPosition { get; set; }
   
    public BoardItemData Data                          // item's data (candy, striped candy, ball, ...)
    {
        get { return _data; }

        set
        {
            _data = value;

            if (_data != null)
            {
                _sr.sprite = _data.Sprite;

                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
    
    public bool Move { get; set; }                      // is item moving toward new slot
    public Bounds Bounds { get { return _sr.bounds; } } // for selection check by touch input

    [SerializeField] private LegBoardData _boardData;
    [SerializeField] private float _speed;
    private SpriteRenderer _sr;
    private BoardItemData _data;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // move toward new slot
        if (Move)
        {
            var offset = TargetPosition - Position;
            var speed = _speed + Mathf.Abs(Mathf.FloorToInt(offset.x + offset.y)) * 0.7f;

            Position = Vector2.MoveTowards(Position, TargetPosition, speed * GameManager.Instance.deltaTime);

            if ((Position - TargetPosition).sqrMagnitude < 0.01f)
            {
                Position = TargetPosition;
                Move = false;
            }
        }
    }

    /// <summary>
    /// called when screen swiped
    /// </summary>
    /// <param name="board"></param>
    /// <param name="direction"></param>
    /// <param name="slot"></param>    
    public void OnSwiped(LegBoard board, int direction, BoardSlot slot)
    {
        _data.OnSwiped(board, direction, slot);
    }

    /// <summary>
    /// called when removed
    /// </summary>
    /// <param name="board"></param>
    /// <param name="destroyer"></param>
    /// <param name="slot"></param>
    public void OnRemoved(LegBoard board, BoardItemType destroyer, BoardSlot slot)
    {
        _data.OnRemoved(board, destroyer, slot);
    }
}

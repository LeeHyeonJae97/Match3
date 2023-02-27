using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoardItem : MonoBehaviour
{
    public Vector2 Position
    {
        get { return transform.position; }

        set { transform.position = value; }
    }
    public Vector2 TargetPosition { get; set; }
    public BoardItemData Data
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
    public bool Move { get; set; }
    public Bounds Bounds { get { return _sr.bounds; } }

    private SpriteRenderer _sr;
    private BoardItemData _data;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Move)
        {
            Position = Vector2.MoveTowards(Position, TargetPosition, 3 * Time.deltaTime);

            if ((Position - TargetPosition).sqrMagnitude < 0.01f)
            {
                Position = TargetPosition;
                Move = false;
            }
        }
    }
}

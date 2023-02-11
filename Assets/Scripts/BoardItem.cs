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
    public BoardItemInfo Info
    {
        get { return _info; }

        set
        {
            _info = value;

            if (_info != null)
            {
                _sr.sprite = _info.Sprite;
                _sr.color = _info.Color;

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
    private BoardItemInfo _info;

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

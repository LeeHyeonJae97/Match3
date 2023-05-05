using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class InputHandler
{
    public UnityAction<Vector2> OnTouched;
    public UnityAction OnReleased;
    public UnityAction<Vector2> OnScrolled;

    public abstract void Update();
}

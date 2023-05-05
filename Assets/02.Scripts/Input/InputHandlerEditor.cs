using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandlerEditor : InputHandler
{
    protected Vector3 _mouse;

    public override void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _mouse = Input.mousePosition;
            _mouse.z -= Camera.main.transform.position.z;
            var pos = Camera.main.ScreenToWorldPoint(_mouse);

            OnTouched?.Invoke(pos);
        }
        else if (Input.GetMouseButton(0))
        {
            OnScrolled?.Invoke(Input.mousePosition - _mouse);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnReleased?.Invoke();
        }
    }
}

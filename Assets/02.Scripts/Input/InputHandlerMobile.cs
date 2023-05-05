using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandlerMobile : InputHandler
{
    public override void Update()
    {
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    var pos = touch.position;
                    pos = Camera.main.ScreenToWorldPoint(pos);
                    OnTouched?.Invoke(pos);
                    break;

                case TouchPhase.Moved:
                    OnScrolled?.Invoke(touch.deltaPosition);
                    break;

                case TouchPhase.Ended:
                    OnReleased?.Invoke();
                    break;
            }
        }
    }
}

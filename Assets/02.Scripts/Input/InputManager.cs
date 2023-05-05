using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public InputHandler Handler
    {
        get
        {
            if (_handler == null)
            {
#if UNITY_EDITOR
                _handler = new InputHandlerEditor();
#elif UNITY_ANDROID
                _handler = new InputHandlerMobile();
#endif
            }
            return _handler;
        }
    }

    private InputHandler _handler;

    private void Update()
    {
        Handler.Update();
    }
}

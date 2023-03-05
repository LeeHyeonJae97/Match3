using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private GameEventChannel _gameEventChannel;
    private Vector3 _mouse;

    public UnityAction<Vector2> onTouched;
    public UnityAction onReleased;
    public UnityAction<Vector2> onScrolled;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _gameEventChannel.OnPaused += OnPaused;
        _gameEventChannel.OnResumed += OnResumed;
    }

    private void OnDestroy()
    {
        _gameEventChannel.OnPaused -= OnPaused;
        _gameEventChannel.OnResumed -= OnResumed;
    }

    private void OnPaused()
    {
        enabled = false;
    }

    private void OnResumed()
    {
        enabled = true;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            _mouse = Input.mousePosition;
            _mouse.z -= Camera.main.transform.position.z;
            var pos = Camera.main.ScreenToWorldPoint(_mouse);

            onTouched?.Invoke(pos);
        }
        else if (Input.GetMouseButton(0))
        {
            onScrolled?.Invoke(Input.mousePosition - _mouse);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            onReleased?.Invoke();
        }
#elif UNITY_ANDROID
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    var pos = touch.position;
                    pos = Camera.main.ScreenToWorldPoint(pos);
                    onTouched?.Invoke(pos);
                    break;

                case TouchPhase.Moved:
                    onScrolled?.Invoke(touch.deltaPosition);
                    break;

                case TouchPhase.Ended:
                    onReleased?.Invoke();
                    break;
            }
        }
#endif
    }
}

using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputManager : Singleton<InputManager>
{
    public event Action<Vector2> OnTouchPerformed;
    public event Action<Vector2> OnSlidePerformed;
    public event Action OnTouchEnded;

    private TouchControlMap touchControlMap;

    private void Awake() => touchControlMap = new TouchControlMap();
    private void OnEnable() => touchControlMap.Enable();
    private void OnDisable() => touchControlMap.Disable();
    private void Start() => Observer();
    private void Observer()
    {
        touchControlMap.Touch.TouchPos.performed += PerformTouch;
        touchControlMap.Touch.Slide.performed += PerformSlide;
        touchControlMap.Touch.IsTouching.canceled += EndTouch;
    }
    private void PerformSlide(InputAction.CallbackContext context)
    {
        if (IsPointerOutsideTheBorder()) return;

        OnSlidePerformed?.Invoke(touchControlMap.Touch.Slide.ReadValue<Vector2>());
    }
    private void PerformTouch(InputAction.CallbackContext context)
    {
        if (IsPointerOutsideTheBorder()) return;

        OnTouchPerformed?.Invoke(touchControlMap.Touch.TouchPos.ReadValue<Vector2>());
    }
    private void EndTouch(InputAction.CallbackContext context)
    {
        if (IsPointerOutsideTheBorder()) return;

        OnTouchEnded?.Invoke();
    }
    private bool IsPointerOutsideTheBorder()
    {
        return float.IsInfinity(touchControlMap.Touch.TouchPos.ReadValue<Vector2>().x);
    }
}
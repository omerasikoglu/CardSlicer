using System;
using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputManager : Singleton<InputManager>
{
    //public event Action<Vector2> OnTouchActionPerformed;

    public event Action<Vector2> OnTouchPerformed;
    public event Action OnTouchEnded;
    public event Action<Vector2> OnSwervePerformed; //swerve amount

    private TouchControlMap touchControlMap;

    private void Awake()
    {
        touchControlMap = new TouchControlMap();
    }

    private void OnEnable() => touchControlMap.Enable();
    private void OnDisable() => touchControlMap.Disable();
    private void Start()
    {
        Observer();
    }

    private void Observer()
    {
        //touchControlMap.Touch.PrimaryTouchContact.started += ctx => IsTouching(ctx);
        //touchControlMap.Touch.Touch.canceled += EndTouch;

        touchControlMap.Touch.TouchPos.performed += PerformTouch;
        touchControlMap.Touch.Swerve.performed += PerformSwerve;

        touchControlMap.Touch.IsTouching.canceled += EndTouch;

    }

    private void PerformSwerve(InputAction.CallbackContext context)
    {
        if (IsPointerOutsideTheBorder()) return;

        OnSwervePerformed?.Invoke(touchControlMap.Touch.Swerve.ReadValue<Vector2>());
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

//private void PerformTouch(InputAction.CallbackContext context)
//{
//    if (float.IsPositiveInfinity(touchControlMap.Touch.Touch.ReadValue<Vector2>().x) ||
//        float.IsNegativeInfinity(touchControlMap.Touch.Touch.ReadValue<Vector2>().x)) return;
//    OnTouchActionPerformed?.Invoke(touchControlMap.Touch.Touch.ReadValue<Vector2>());
//}
/*, (float)context.startTime*/
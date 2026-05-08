using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CInputManager : MonoBehaviour
{
    public List<Action> onClickHandlers = new();
    bool lClick;

    void Update()
    {
        bool cClick = CheckInput();
        if (!lClick && cClick) foreach (var handler in onClickHandlers) handler.Invoke();
        lClick = cClick;
    }
    
    private bool CheckInput()
    {
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            return true;
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            return true;
        }

        return false;
    }
}
